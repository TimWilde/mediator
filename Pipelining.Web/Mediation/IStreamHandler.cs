// ReSharper disable UnusedParameter.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Mediator.Web.Mediation;

public interface IStreamHandler { }

public interface IStreamHandler<in TRequest, out TResponse>: IStreamHandler
   where TRequest: IRequest<TResponse>
{
   IAsyncEnumerable<TResponse> Handle( TRequest request, CancellationToken cancellationToken );
}
