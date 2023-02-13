namespace Mediator.Web.Mediation.Implementation;

public class RequestHandlerConfigurationException: Exception
{
   private RequestHandlerConfigurationException( string message ): base( message ) { }

   public static RequestHandlerConfigurationException NoHandlerMethod( Type handlerType, Type messageType ) =>
      new( $"The {handlerType.FullName} request handler does not handle messages of type {messageType.FullName}" );

   public static RequestHandlerConfigurationException InvalidRequestHandler<THandler>() =>
      new( $"{typeof( THandler ).FullName} does not implement either the mandatory {Mediator.FunctionHandlerType.FullName} " +
           $"or {Mediator.ActionHandlerType.FullName} interface" );

   public static RequestHandlerConfigurationException InvalidStreamHandler<THandler>() =>
      new( $"{typeof( THandler ).FullName} does not implement the mandatory {Mediator.StreamHandlerType.FullName} interface" );

   public static RequestHandlerConfigurationException InvalidNotificationHandler<THandler>() =>
      new( $"{typeof( THandler ).FullName} does not implement the mandatory {Mediator.NotificationHandlerType.FullName} interface" );
}