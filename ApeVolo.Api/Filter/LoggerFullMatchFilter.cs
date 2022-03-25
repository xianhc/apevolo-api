using System;
using log4net.Core;
using log4net.Filter;

namespace ApeVolo.Api.Filter;

public class LoggerFullMatchFilter : LoggerMatchFilter
{
    public override FilterDecision Decide(LoggingEvent loggingEvent)
    {
        if (loggingEvent == null)
        {
            throw new ArgumentNullException(nameof(loggingEvent));
        }

        if (!string.IsNullOrEmpty(LoggerToMatch)
            && loggingEvent.LoggerName == LoggerToMatch
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