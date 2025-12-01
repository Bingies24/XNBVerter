namespace XNBVerter.Core
{
    /// <summary>
    /// Specifies the type of operation the CLI should perform,
    /// based on user-supplied arguments.
    /// </summary>
    internal enum TaskType
    {
        /// <summary>
        /// Indicates that no specific task was requested.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates that the input should be
        /// converted into an XNB Song file.
        /// </summary>
        Song = 1
    }
}
