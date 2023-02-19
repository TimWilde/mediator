// ReSharper disable ClassNeverInstantiated.Global

namespace Mediator.Web.Messaging.Handlers;

using Mediation;

public class PipelineHandlers
{
   public class StringToDateTime : IPipelineRequestHandler<string, DateTime>
   {
      private readonly ILogger<StringToDateTime> logger;

      public StringToDateTime(ILogger<StringToDateTime> logger)
      {
         this.logger = logger;
      }

      public Task<DateTime> Process(string input, CancellationToken cancellationToken)
      {
         logger.LogInformation("-- Converting a String to DateTime");
         return Task.FromResult(DateTime.Parse(input));
      }
   }

   public class DateTimeToDateOnly : IPipelineRequestHandler<DateTime, DateOnly>
   {
      private readonly ILogger<DateTimeToDateOnly> logger;

      public DateTimeToDateOnly(ILogger<DateTimeToDateOnly> logger)
      {
         this.logger = logger;
      }

      public Task<DateOnly> Process(DateTime input, CancellationToken cancellationToken)
      {
         logger.LogInformation("-- Converting a DateTime to a DateOnly");
         return Task.FromResult(DateOnly.FromDateTime(input));
      }
   }

   public class DateOnlyToDaysSince : IPipelineRequestHandler<DateOnly, int>
   {
      private readonly ILogger<DateOnlyToDaysSince> logger;

      public DateOnlyToDaysSince(ILogger<DateOnlyToDaysSince> logger)
      {
         this.logger = logger;
      }

      public Task<int> Process(DateOnly input, CancellationToken cancellationToken)
      {
         logger.LogInformation("-- Calculating number of days since the DateOnly date");
         return Task.FromResult((int)Math.Ceiling(DateTime.UtcNow.Subtract(input.ToDateTime(TimeOnly.MinValue)).TotalDays));
      }
   }
}
