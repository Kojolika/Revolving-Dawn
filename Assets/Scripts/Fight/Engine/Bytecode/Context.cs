namespace Fight.Engine.Bytecode
{
    /// <summary>
    /// Defines the current execution context for an <see cref="IInstruction"/>
    /// </summary>
    public class Context
    {
        /// <summary>
        /// Provides methods to access the current stack memory each instruction has access to.
        /// </summary>
        public IWorkingMemory Memory { get; private set; }

        /// <summary>
        /// Provides methods to access the current bytecode input. (Basically the current program being read).
        /// </summary>
        public IInputStream InputStream { get; private set; }

        /// <summary>
        /// Provides methods to access data from the current fight.
        /// </summary>
        public IFightContext Fight { get; private set; }

        /// <summary>
        /// Provides an interface for the current logger.
        /// </summary>
        public ILogger Logger { get; private set; }
    }
}