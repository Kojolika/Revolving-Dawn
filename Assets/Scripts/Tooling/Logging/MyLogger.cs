using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Serialization;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Tooling.Logging
{
    public class MyLogger : ILogger, ITraceWriter
    {
        private static readonly StackTrace stackTrace = new StackTrace(true);

        public void Log(LogLevel level, string message, [CallerFilePath] string filePath = "", params object[] args)
        {
            switch (level)
            {
                case LogLevel.Info:
                    Log(message, filePath);
                    break;
                case LogLevel.Warning:
                    LogWarning(message, filePath);
                    break;
                case LogLevel.Error:
                    LogError(message, filePath);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        public static void Log(string message, [CallerFilePath] string filePath = "")
        {
            Debug.Log(
                FormatLog(message, filePath)
            );
        }

        public static void Log(string message, GameObject context, [CallerFilePath] string filePath = "")
        {
            Debug.Log(
                FormatLog(message, filePath),
                context
            );
        }

        public static void LogWarning(string message, [CallerFilePath] string filePath = "")
        {
            Debug.LogWarning(
                FormatLog(message, filePath)
            );
        }

        public static void LogWarning(string message, GameObject context, [CallerFilePath] string filePath = "")
        {
            Debug.LogWarning(
                FormatLog(message, filePath),
                context
            );
        }

        public static void LogError(string message, [CallerFilePath] string filePath = "")
        {
            Debug.LogError(
                FormatLog(message, filePath)
            );
        }

        public static void LogError(string message, GameObject context, [CallerFilePath] string filePath = "")
        {
            Debug.LogError(
                FormatLog(message, filePath),
                context
            );
        }

        private static string FormatLog(string message, string filePath)
        {
            var fileName = GetFileNameFromFilePath(filePath);
            var uniqueColor = Options.GetFilenameColor(fileName);
            var currentStackFrame = stackTrace.GetFrame(0);
            var logMessage = $"<b><color={uniqueColor}><size=25>@</size></color>" +
                             $"<color={Options.FilenameColor}> [{fileName}]</color></b>" +
                             $" {message}";

            if (currentStackFrame.GetMethod() is MethodInfo methodInfo)
            {
                var nameSpace = methodInfo.ReflectedType?.Namespace;

                logMessage = nameSpace == null
                    ? logMessage
                    : "<b><color=" + Options.NameSpaceColor + ">[" + nameSpace + "]</color></b> " + logMessage;
            }

            return logMessage;
        }

        /// <summary>
        /// Formats a file path to just include the file name instead of the entire path.
        /// </summary>
        /// <param name="filePath">File path.</param>
        /// <returns>File name of the file path</returns>
        private static string GetFileNameFromFilePath(string filePath)
        {
            int filePathStringLength = filePath.Length;
            for (int index = filePathStringLength - 1; index > 0; index--)
            {
                if (filePath[index] == '\\')
                {
                    // cut the \ from the string
                    int startIndex = index + 1;

                    return filePath.Substring(startIndex);
                }
            }

            return filePath;
        }

        /// <summary>
        /// Stores default values for preferences of the logger.
        /// </summary>
        private static class Options
        {
            internal const string NameSpaceColor = "white";
            internal const string FilenameColor = "#1fa734ff";

            internal static string GetFilenameColor(string fileName)
                => $"#{Convert.ToString(fileName.GetHashCode(), 16)}";
        }

        #region ITraceWriter

        public void Trace(TraceLevel level, string message, Exception ex)
        {
            switch (level)
            {
                case TraceLevel.Error:
                    LogError(message);
                    break;
                case TraceLevel.Info:
                    Log(message);
                    break;
                case TraceLevel.Verbose:
                    Log(message);
                    break;
                case TraceLevel.Warning:
                    LogWarning(message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        public TraceLevel LevelFilter => TraceLevel.Verbose;

        #endregion
    }

    public interface ILogger
    {
        public void Log(LogLevel level, string message, [CallerFilePath] string filePath = "", params object[] args);
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }
}