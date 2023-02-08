namespace Pipelining.Web.Mediation;

public class RequestHandlerConfigurationException: Exception
{
   private RequestHandlerConfigurationException( string message ): base( message ) { }

   public static RequestHandlerConfigurationException NoHandlerMethodFor( Type messageType ) =>
      new( $"The request handler does not handle messages of type {messageType.FullName}" );

   public static RequestHandlerConfigurationException InvalidRequestHandler<THandler>() =>
      new( $"{typeof( THandler ).FullName} does not implement either the mandatory {Mediator.FunctionHandlerType.FullName} " +
           $"or {Mediator.ActionHandlerType.FullName} interface" );
}
