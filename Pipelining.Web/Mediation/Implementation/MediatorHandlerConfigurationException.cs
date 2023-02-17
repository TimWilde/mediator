namespace Mediator.Web.Mediation.Implementation;

public class MediatorHandlerConfigurationException: Exception
{
   private MediatorHandlerConfigurationException( string message ): base( message ) { }

   public static MediatorHandlerConfigurationException NoHandlerMethod( Type handlerType, Type messageType ) =>
      new( $"The {handlerType.FullName} request handler does not handle messages of type {messageType.FullName}" );

   public static MediatorHandlerConfigurationException InvalidRequestHandler<THandler>() =>
      new( $"{typeof( THandler ).FullName} does not implement either the mandatory {Mediator.FunctionHandlerType.FullName} " +
           $"or {Mediator.ActionHandlerType.FullName} interface" );

   public static MediatorHandlerConfigurationException InvalidStreamHandler<THandler>() =>
      new( $"{typeof( THandler ).FullName} does not implement the mandatory {Mediator.StreamHandlerType.FullName} interface" );

   public static MediatorHandlerConfigurationException InvalidNotificationHandler<THandler>() =>
      new( $"{typeof( THandler ).FullName} does not implement the mandatory {Mediator.NotificationHandlerType.FullName} interface" );
}