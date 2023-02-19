// ReSharper disable UnusedTypeParameter

namespace Mediator.Web.Mediation;

public interface IRequest { }

public interface IRequest<TResponse>: IRequest { }
