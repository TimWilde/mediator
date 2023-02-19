namespace Mediator.Web.Mediation.Implementation;

public class PipelineBuilder
{
   private const int INPUT = 0,
                     OUTPUT = 1;

   private static readonly Type PipelineHandlerType = typeof( IPipelineRequestHandler<,> );
   private static readonly Type StepWrapperType = typeof( PipelineStepWrapper<,,,> );
   private static readonly Type StepWrapperInterfaceType = typeof( IStepWrapper<,> );
   private static readonly Type LoopbackHandlerType = typeof( LoopbackStepWrapper<> );

   private readonly List<(Type handlerType, ServiceLifetime lifetime)> handlers = new();

   public PipelineBuilder AddOperation<THandler>( ServiceLifetime lifetime = ServiceLifetime.Transient )
      where THandler: IPipelineRequestHandler
   {
      handlers.Add( ( typeof( THandler ), lifetime ) );
      return this;
   }

   public void Build( IServiceCollection services )
   {
      handlers.Reverse();

      Type? loopbackHandlerType = default;
      Type? finalStepOutput = default;

      foreach( ( Type? handlerType, ServiceLifetime lifetime ) in handlers )
      {
         Type handlerInterface = handlerType.GetInterface( PipelineHandlerType.Name )!;
         Type[] genericArguments = handlerInterface.GetGenericArguments();

         if( loopbackHandlerType == default )
         {
            finalStepOutput = genericArguments[ OUTPUT ];
            loopbackHandlerType ??= LoopbackHandlerType.MakeGenericType( finalStepOutput );

            Type nextStepInterface = StepWrapperInterfaceType.MakeGenericType( finalStepOutput, finalStepOutput );

            services.Add( new ServiceDescriptor( nextStepInterface, loopbackHandlerType, ServiceLifetime.Singleton ) );
         }

         services.Add( new ServiceDescriptor( handlerType, handlerType, lifetime ) );

         Type stepWrapperType =
            StepWrapperType.MakeGenericType( finalStepOutput!,
                                             genericArguments[ INPUT ],
                                             genericArguments[ OUTPUT ],
                                             handlerType );

         Type stepWrapperInterface =
            StepWrapperInterfaceType.MakeGenericType( genericArguments[ INPUT ], finalStepOutput! );

         services.Add( new ServiceDescriptor( stepWrapperInterface, stepWrapperType, lifetime ) );
      }
   }

   public interface IStepWrapper<in TIn, TOut>
   {
      Task<TOut> Process( TIn input, CancellationToken cancellationToken );
   }

   private class PipelineStepWrapper<TPipelineOutput, TStepInput, TStepOutput, TStep>
      : IStepWrapper<TStepInput, TPipelineOutput>
      where TStep: IPipelineRequestHandler<TStepInput, TStepOutput>
   {
      private readonly IStepWrapper<TStepOutput, TPipelineOutput> nextStepWrapper;
      private readonly TStep step;

      public PipelineStepWrapper( TStep step, IStepWrapper<TStepOutput, TPipelineOutput> nextStepWrapper )
      {
         this.step = step;
         this.nextStepWrapper = nextStepWrapper;
      }

      public async Task<TPipelineOutput> Process( TStepInput input, CancellationToken cancellationToken ) =>
         await nextStepWrapper.Process( await step.Process( input, cancellationToken ), cancellationToken );
   }

   private class LoopbackStepWrapper<TIn>: IStepWrapper<TIn, TIn>
   {
      public Task<TIn> Process( TIn input, CancellationToken cancellationToken ) =>
         Task.FromResult( input );
   }
}
