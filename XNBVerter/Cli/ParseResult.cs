using XNBVerter.Core;

namespace XNBVerter.Cli
{
    /// <summary>
    /// Represents the outcome of parsing CLI arguments,
    /// including the resolved task type and any validated file paths.
    /// </summary>
    internal sealed class ParseResult(TaskType? task, List<string> filePaths)
    {
        /// <summary>
        /// Gets the task type determined from the parsed arguments,
        /// or <c>null</c> if no valid task was specified.
        /// </summary>
        public TaskType? Task { get; } = task;

        /// <summary>
        /// Gets the list of absolute file paths that were validated
        /// and accepted as input files during parsing.
        /// </summary>
        public List<string> FilePaths { get; } = filePaths;
    }
}
