using XNBVerter.Core;

namespace XNBVerter.Cli
{
    internal sealed class ParseResult(TaskType? task, List<string> filePaths)
    {
        public TaskType? Task { get; } = task;

        public List<string> FilePaths { get; } = filePaths;
    }
}
