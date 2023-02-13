// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedParameter.Global

namespace Mediator.Web.Mediation;

public interface IRequestHandler { }

public interface IRequestHandler<in TRequest>
   : IRequestHandler
   where TRequest: IRequest
{
   Task Handle( TRequest request );
}

public interface IRequestHandler<in TRequest, TResponse>
   : IRequestHandler
   where TRequest: IRequest<TResponse>
{
   Task<TResponse> Handle( TRequest request );
}