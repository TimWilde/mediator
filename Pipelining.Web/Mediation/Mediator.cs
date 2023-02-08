namespace Pipelining.Web.Mediation;

using System.Diagnostics;
using System.Reflection;

[ DebuggerStepThrough ]
public class Mediator: IMediator
{
   public static readonly Type FunctionHandlerType = typeof( IRequestHandler<,> );
   public static readonly Type ActionHandlerType = typeof( IRequestHandler<> );

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

   private Task ExecuteHandler( Type openHandlerType, IRequest message, Type? responseType = null )
   {
      List<Type> genericTypes = new() { message.GetType() };

      if( responseType is not null )
         genericTypes.Add( responseType );

      Type closedHandlerType = openHandlerType.MakeGenericType( genericTypes.ToArray() );
      MethodInfo? handleMethod = closedHandlerType.GetMethod( Handle, BindingFlags.Public | BindingFlags.Instance );

      if( handleMethod is null )
         throw RequestHandlerConfigurationException.NoHandlerMethodFor( message.GetType() );

      object requestHandler = services.GetRequiredService( closedHandlerType );

      return (Task) handleMethod.Invoke( requestHandler, new object[] { message } )!;
   }
}
