namespace Mediator.Web.Mediation.Implementation;

using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;

[ DebuggerStepThrough ]
public class Mediator: IMediator
{
   public static readonly Type FunctionHandlerType = typeof( IRequestHandler<,> );
   public static readonly Type ActionHandlerType = typeof( IRequestHandler<> );
   public static readonly Type StreamHandlerType = typeof( IStreamHandler<,> );
   public static readonly Type NotificationHandlerType = typeof( INotificationHandler<> );

   private static readonly string Handle = nameof(Handle);
   private readonly ILogger<Mediator> logger;

   private readonly IServiceProvider services;

   public Mediator( IServiceProvider services, ILogger<Mediator> logger )
   {
      this.services = services;
      this.logger = logger;
   }

   public async Task<TOut> Pipeline<TIn, TOut>( TIn input, CancellationToken cancellationToken ) =>
      await services.GetRequiredService<PipelineBuilder.IStepWrapper<TIn, TOut>>()
                    .Process( input, cancellationToken );

   public Task DispatchAsync( IRequest message )
   {
      ( object? handler, MethodInfo? handleMethod ) = GetFirstHandler( ActionHandlerType, message );

      return (Task) handleMethod.Invoke( handler, new object[] { message } )!;
   }

   public Task<TOut> DispatchAsync<TOut>( IRequest<TOut> message )
   {
      ( object? handler, MethodInfo? handleMethod ) = GetFirstHandler( FunctionHandlerType, message, typeof( TOut ) );

      return (Task<TOut>) handleMethod.Invoke( handler, new object[] { message } )!;
   }

   public IAsyncEnumerable<TOut> CreateStream<TOut>( IRequest<TOut> message, CancellationToken cancellationToken )
   {
      ( object? handler, MethodInfo? handleMethod ) = GetFirstHandler( StreamHandlerType, message, typeof( TOut ) );

      return (IAsyncEnumerable<TOut>) handleMethod.Invoke( handler, new object[] { message, cancellationToken } )!;
   }

   public Task PublishAsync<TNotification>( TNotification notification )
   {
      ( IReadOnlyList<object?> handlers, MethodInfo? handleMethod ) = GetHandlers( NotificationHandlerType, notification );

      return Task.WhenAll( handlers.Where( handler => handler is not null )
                                   .Select( handler => (Task) handleMethod.Invoke( handler, new object[] { notification! } )! ) );
   }

   private (object handler, MethodInfo handleMethod) GetFirstHandler<TMessage>( Type openHandlerType,
                                                                                TMessage message,
                                                                                Type? responseType = null )
   {
      ( IReadOnlyList<object?> handlers, MethodInfo? handleMethod ) = GetHandlers( openHandlerType, message, responseType );

      if( handlers.Count > 1 )
         logger.LogWarning( "More than one handler found for {MessageType} when calling {MethodName} - only invoking the first!",
                            message!.GetType(),
                            new StackFrame( 1 ).GetMethod()!.Name );

      return ( handlers.First(), handleMethod )!;
   }

   private (IReadOnlyList<object?> handlers, MethodInfo handleMethod) GetHandlers<TMessage>( Type openHandlerType,
                                                                                             TMessage message,
                                                                                             Type? responseType = null )
   {
      List<Type> genericTypes = new() { message!.GetType() };

      if( responseType is not null )
         genericTypes.Add( responseType );

      Type closedHandlerType = openHandlerType.MakeGenericType( genericTypes.ToArray() );
      MethodInfo? handleMethod = closedHandlerType.GetMethod( Handle, BindingFlags.Public | BindingFlags.Instance );

      if( handleMethod is null )
         throw MediatorHandlerConfigurationException.NoHandlerMethod( closedHandlerType, message.GetType() );

      return ( services.GetServices( closedHandlerType ).ToImmutableList(), handleMethod );
   }
}
