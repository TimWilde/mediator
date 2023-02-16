// ReSharper disable UnusedParameter.Global
// ReSharper disable UnusedType.Global

namespace Mediator.Web.Mediation;

public interface INotificationHandler { }

public interface INotificationHandler<in TRequest>
   : INotificationHandler
{
   Task Handle( TRequest request );
}