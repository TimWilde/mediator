namespace Pipelining.Web.Mediation;

public interface IMediator
{
   Task<TOut> DispatchAsync<TOut>( IRequest<TOut> message );
   Task DispatchAsync( IRequest message );
}
