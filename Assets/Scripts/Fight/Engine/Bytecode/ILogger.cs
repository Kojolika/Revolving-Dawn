namespace Fight.Engine.Bytecode
{
    public interface ILogger
    {
        void Log(LogLevel level, string message);
    }

    public enum LogLevel
    {
        Info,
        Debug,
        Warning,
        Error
    }
}