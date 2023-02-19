// ReSharper disable UnusedParameter.Global

namespace Mediator.Web.Mediation;

public interface IPipelineRequestHandler { }

public interface IPipelineRequestHandler<in TIn, TOut>: IPipelineRequestHandler
{
   Task<TOut> Process( TIn input, CancellationToken cancellationToken );
}