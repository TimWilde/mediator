namespace Mediator.Web.Mediation.Implementation;

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

   private readonly IServiceProvider services;

   public Mediator( IServiceProvider services )
   {
      this.services = services;
   }

   public Task<TOut> DispatchAsync<TOut>( IRequest<TOut> message ) =>
      (Task<TOut>) ExecuteHandler( FunctionHandlerType, message, typeof( TOut ) );

   public Task DispatchAsync( IRequest message ) =>
      ExecuteHandler( ActionHandlerType, message );

   public IAsyncEnumerable<TOut> CreateStream<TOut>( IRequest<TOut> request, CancellationToken cancellationToken )
   {
      List<Type> genericTypes = new() { request.GetType(), typeof( TOut ) };

      Type closedHandlerType = StreamHandlerType.MakeGenericType( genericTypes.ToArray() );
      MethodInfo? handleMethod = closedHandlerType.GetMethod( Handle, BindingFlags.Public | BindingFlags.Instance );

      if ( handleMethod is null )
         throw RequestHandlerConfigurationException.NoHandlerMethod( closedHandlerType, request.GetType() );

      object streamHandler = services.GetRequiredService( closedHandlerType );

      return (IAsyncEnumerable<TOut>) handleMethod.Invoke( streamHandler, new object[] { request, cancellationToken } )!;
   }

   public Task PublishAsync( IRequest notification )
   {
      Type closedHandlerType = NotificationHandlerType.MakeGenericType( notification.GetType() );
      MethodInfo? handleMethod = closedHandlerType.GetMethod( Handle, BindingFlags.Public | BindingFlags.Instance );

      if ( handleMethod is null )
         throw RequestHandlerConfigurationException.NoHandlerMethod( closedHandlerType, notification.GetType() );

      IEnumerable<object?> notificationHandlers = services.GetServices( closedHandlerType );

      List<Task> handlerTasks = notificationHandlers.Where( handler => handler is not null )
                                                    .Select( handler => (Task) handleMethod.Invoke( handler, new object[] { notification } )! )
                                                    .ToList();

      return Task.WhenAll( handlerTasks );
   }

   private Task ExecuteHandler( Type openHandlerType, IRequest message, Type? responseType = null )
   {
      List<Type> genericTypes = new() { message.GetType() };

      if ( responseType is not null )
         genericTypes.Add( responseType );

      Type closedHandlerType = openHandlerType.MakeGenericType( genericTypes.ToArray() );
      MethodInfo? handleMethod = closedHandlerType.GetMethod( Handle, BindingFlags.Public | BindingFlags.Instance );

      if ( handleMethod is null )
         throw RequestHandlerConfigurationException.NoHandlerMethod( closedHandlerType, message.GetType() );

      object requestHandler = services.GetRequiredService( closedHandlerType );

      return (Task) handleMethod.Invoke( requestHandler, new object[] { message } )!;
   }
}