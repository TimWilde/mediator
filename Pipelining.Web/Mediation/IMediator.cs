namespace Mediator.Web.Mediation;

public interface IMediator
{
   /// <summary>
   ///    Delivers <paramref name="message" /> to a single handler and returns the response
   /// </summary>
   /// <typeparam name="TOut">The type of the response from the handler</typeparam>
   /// <param name="message">The instance encapsulating the request</param>
   /// <returns>The response from the handler that conforms to the TOut type</returns>
   /// <exception cref="InvalidOperationException">A handler for the specified type is not registered</exception>
   Task<TOut> DispatchAsync<TOut>( IRequest<TOut> message );

   /// <summary>
   ///    Delivers <paramref name="message" /> to a single handler that does not produce a response
   /// </summary>
   /// <param name="message">The instance encapsulating the request</param>
   /// <returns>An asynchronous void response</returns>
   /// <exception cref="InvalidOperationException">A handler for the specified type is not registered</exception>
   Task DispatchAsync( IRequest message );

   /// <summary>
   ///    Delivers <paramref name="message" /> to a single handler that streams back the response asynchronously
   /// </summary>
   /// <typeparam name="TOut">The type of the response streamed by the handler</typeparam>
   /// <param name="message">The instance encapsulating the request</param>
   /// <param name="cancellationToken">Token propagating cancellation notifications</param>
   /// <returns>An asynchronous stream of TOut instances produced by the handler</returns>
   /// <exception cref="InvalidOperationException">A handler for the specified type is not registered</exception>
   IAsyncEnumerable<TOut> CreateStream<TOut>( IRequest<TOut> message, CancellationToken cancellationToken );

   /// <summary>
   ///    Sends a single <paramref name="notification" /> to all registered handlers for that type
   /// </summary>
   /// <param name="notification">The instance encapsulating the notification</param>
   /// <returns>An asynchronous void response</returns>
   /// <remarks>
   ///    <para>This method will silently complete if there are no handlers registered for the notification type</para>
   ///    <para>Notification handlers are called in parallel and in an indeterminate order</para>
   ///    <para>All notification handlers will complete before the call returns</para>
   /// </remarks>
   /// <typeparam name="TNotification">The type of the message that will be published to all subscribers</typeparam>
   Task PublishAsync<TNotification>( TNotification notification );
}