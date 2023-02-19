namespace Mediator.Web.Mediation.Implementation;

using Messaging.Handlers;

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

      Type? nextStepWrapper = default;
      Type? nextStepOutput = default;

      foreach( ( Type? handlerType, ServiceLifetime lifetime ) in handlers )
      {
         Type handlerInterface = handlerType.GetInterface( PipelineHandlerType.Name )!;
         Type[] genericArguments = handlerInterface.GetGenericArguments();

         // Register the handler
         services.Add( new ServiceDescriptor( handlerType, handlerType, lifetime ) );

         // If there isn't a next step, use the loopback step
         if( nextStepWrapper == default )
         {
            nextStepWrapper ??= LoopbackHandlerType.MakeGenericType( genericArguments[ OUTPUT ] );
            nextStepOutput ??= genericArguments[ OUTPUT ];

            Type nextStepInterface = StepWrapperInterfaceType.MakeGenericType( genericArguments[ OUTPUT ], genericArguments[ OUTPUT ] );

            services.Add( new ServiceDescriptor( nextStepInterface, nextStepWrapper, ServiceLifetime.Singleton ) );
         }

         // Compose the step wrapper type definition
         Type stepWrapperType =
            StepWrapperType.MakeGenericType( genericArguments[ INPUT ],
                                             genericArguments[ OUTPUT ],
                                             nextStepOutput!,
                                             handlerType );

         // Compose the step wrapper interface definition
         Type stepWrapperInterface =
            StepWrapperInterfaceType.MakeGenericType( genericArguments[ INPUT ],
                                                      nextStepOutput! );

         // Register the wrapper
         services.Add( new ServiceDescriptor( stepWrapperInterface, stepWrapperType, lifetime ) );
      }
   }

   private class PipelineStepWrapper<TIn, TOut, TNextOut, TStep>
      : IStepWrapper<TIn, TNextOut>
      where TStep: IPipelineRequestHandler<TIn, TOut>
   {
      private readonly IStepWrapper<TOut, TNextOut> nextStep;
      private readonly TStep step;

      public PipelineStepWrapper( TStep step, IStepWrapper<TOut, TNextOut> nextStep )
      {
         this.step = step;
         this.nextStep = nextStep;
      }

      public async Task<TNextOut> Process( TIn input, CancellationToken cancellationToken ) =>
         await nextStep.Process( await step.Process( input, cancellationToken ), cancellationToken );
   }

   private class LoopbackStepWrapper<TIn>: IStepWrapper<TIn, TIn>
   {
      public Task<TIn> Process( TIn input, CancellationToken cancellationToken ) =>
         Task.FromResult( input );
   }
}