using log4net.Core;
using log4net.Filter;
using System;

namespace ApeVolo.Api.Filter
{
    public class LoggerFullMatchFilter : LoggerMatchFilter
    {
        public override FilterDecision Decide(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                throw new ArgumentNullException(nameof(loggingEvent));
            }

            if (!string.IsNullOrEmpty(this.LoggerToMatch)
                && loggingEvent.LoggerName == this.LoggerToMatch
            )
            {
                if (AcceptOnMatch)
                {
                    return FilterDecision.Accept;
                }

                return FilterDecision.Deny;
            }

            return FilterDecision.Neutral;
        }
    }
}