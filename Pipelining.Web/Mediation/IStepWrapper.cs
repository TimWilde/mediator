namespace Mediator.Web.Mediation;

public interface IStepWrapper<in TIn, TOut>
{
   Task<TOut> Process( TIn input, CancellationToken cancellationToken );
}
