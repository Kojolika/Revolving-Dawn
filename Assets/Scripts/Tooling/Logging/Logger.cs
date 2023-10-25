using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Tooling.Logging
{
    public static class Logger
    {
        private static readonly StackTrace stackTrace = new StackTrace(true);

        public static void Log(string message, [CallerFilePath] string filePath = "")
            => Debug.Log(
                FormatLog(message, filePath)
            );

        public static void Log(string message, GameObject context, [CallerFilePath] string filePath = "")
            => Debug.Log(
                FormatLog(message, filePath),
                context
            );

        public static void LogWarning(string message, [CallerFilePath] string filePath = "")
            => Debug.LogWarning(
                FormatLog(message, filePath)
            );

        public static void LogWarning(string message, GameObject context, [CallerFilePath] string filePath = "")
            => Debug.LogWarning(
                FormatLog(message, filePath),
                context
            );


        private static string FormatLog(string message, string filePath)
        {
            var currentStackFrame = stackTrace.GetFrame(0);
            var logMessage = "<b><color=" + Options.FilenameColor + "> ["
                             + GetFileNameFromFilePath(filePath)
                             + "]</color></b> "
                             + message;

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
        }
    }
}