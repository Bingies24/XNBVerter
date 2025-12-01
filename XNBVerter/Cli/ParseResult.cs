using XNBVerter.Core;

namespace XNBVerter.Cli
{
    /// <summary>
    /// Represents the outcome of parsing CLI arguments,
    /// including the resolved task type, validated file paths,
    /// and any error that occurred during parsing.
    /// </summary>
    internal sealed class ParseResult
    {
        public TaskType? Task { get; }
        public List<string> FilePaths { get; }
        public string? Error { get; }

        /// <summary>
        /// True when the parse completed successfully with no errors.
        /// </summary>
        public bool IsSuccess => Error is null;

        /// <summary>
        /// Normal successful result.
        /// </summary>
        public ParseResult(TaskType? task, List<string> filePaths)
        {
            Task = task;
            FilePaths = filePaths;
        }

        /// <summary>
        /// Error result (task + files ignored).
        /// </summary>
        public ParseResult(string errorMessage)
        {
            Error = errorMessage;
            Task = null;
            FilePaths = [];
        }
    }
}
