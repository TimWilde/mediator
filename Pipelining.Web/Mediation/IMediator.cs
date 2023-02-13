namespace Mediator.Web.Mediation;

public interface IMediator
{
   Task<TOut> DispatchAsync<TOut>( IRequest<TOut> message );
   Task DispatchAsync( IRequest message );
   IAsyncEnumerable<TOut> CreateStream<TOut>( IRequest<TOut> request, CancellationToken cancellationToken );
   Task PublishAsync( IRequest notification );
}