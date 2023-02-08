﻿// ReSharper disable ClassNeverInstantiated.Global

namespace Pipelining.Web.Messaging;

using Mediation;

public class SetSomethingHandler: IRequestHandler<SetSomething>
{
   private readonly ILogger<SetSomethingHandler> logger;

   public SetSomethingHandler( ILogger<SetSomethingHandler> logger )
   {
      this.logger = logger;
   }

   public Task Handle( SetSomething request )
   {
      logger.LogInformation( "Set something handler called!" );
      return Task.CompletedTask;
   }
}
