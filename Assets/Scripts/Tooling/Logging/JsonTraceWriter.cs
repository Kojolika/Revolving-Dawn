using System;
using System.Diagnostics;
using Newtonsoft.Json.Serialization;

namespace Tooling.Logging
{
    public class JsonTraceWriter : ITraceWriter
    {
        public TraceLevel LevelFilter => TraceLevel.Verbose;

        public void Trace(TraceLevel level, string message, Exception ex)
        {
            switch (level)
            {
                case TraceLevel.Error:
                    MyLogger.LogError(message);
                    break;
                case TraceLevel.Warning:
                    MyLogger.LogWarning(message);
                    break;
                case TraceLevel.Info:
                case TraceLevel.Verbose:
                    MyLogger.Log(message);
                    break;
            }
        }
    }
}