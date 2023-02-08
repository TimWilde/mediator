// ReSharper disable UnusedTypeParameter

namespace Pipelining.Web.Mediation;

public interface IRequest { }

public interface IRequest<TResponse>: IRequest { }
