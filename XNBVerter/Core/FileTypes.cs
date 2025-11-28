namespace XNBVerter.Core
{
    /// <summary>
    /// Provides collections of supported input file extensions
    /// used by XNBVerter during parsing and task evaluation.
    /// </summary>
    internal static class FileTypes
    {
        public static readonly HashSet<string> AllFileTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            ".wav", ".mp3", ".ogg", ".wma", ".xnb"
        };

        public static readonly HashSet<string> SongFileTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            ".wav", ".mp3", ".ogg", ".wma"
        };
    }
}
