namespace XNBVerter.Core
{
    /// <summary>
    /// Creates XNB files containing song metadata for XNA/MonoGame audio streaming.
    /// </summary>
    /// <remarks>
    /// This creator generates minimal XNB headers that reference external audio files for streaming playback,
    /// rather than embedding the actual audio data within the XNB file itself.
    /// </remarks>
    internal sealed class SongXnbCreator
    {
        /// <summary>
        /// The fixed overhead size in bytes for the XNB song header structure, excluding the variable-length filename.
        /// </summary>
        /// <remarks>
        /// This value accounts for all fixed-size fields in the XNB format:
        /// magic number (3), platform (1), version (1), flags (1), file size (4),
        /// type reader count (1), SongReader string and version (47), Int32Reader string and version (48),
        /// shared resource count (1), primary object type ID (1), duration type ID (1), and duration value (4).
        /// Total: 113 bytes before adding the filename length.
        /// </remarks>
        private const int XnbSongHeaderOverhead = 113;

        /// <summary>
        /// Creates an XNB file that references an audio file for streaming playback.
        /// </summary>
        public static void CreateSongXnb(string inputFile, int durationMs)
        {
            ArgumentNullException.ThrowIfNull(inputFile);

            string outputPath = Path.ChangeExtension(inputFile, ".xnb");

            using FileStream file = File.Create(outputPath);
            using BinaryWriter writer = new(file);

            WriteSongHeader(writer, inputFile, durationMs);

            if (!File.Exists(outputPath))
            {
                throw new IOException($"Failed to create XNB file: {outputPath}");
            }
        }

        /// <summary>
        /// Writes the complete XNB header structure for a streaming song reference.
        /// </summary>
        private static void WriteSongHeader(BinaryWriter writer, string inputFile, int durationMs)
        {
            string streamingFilename = Path.GetFileName(inputFile);

            // "XNB", total: 3 bytes
            writer.Write((byte)'X'); // 1 byte
            writer.Write((byte)'N'); // 1 byte
            writer.Write((byte)'B'); // 1 byte

            // Target platform "w" (Windows)
            writer.Write((byte)'w'); // 1 byte

            // XNB format version 5
            writer.Write((byte)5); // 1 byte

            // Flag (0 = no compression)
            writer.Write((byte)0); // 1 byte

            // File size (header + filename)
            int fileSize = streamingFilename.Length + XnbSongHeaderOverhead;
            writer.Write(fileSize); // UInt32: 4 bytes

            // Type reader count
            writer.Write7BitEncodedInt(2); // 1 byte

            // Type reader 1: SongReader
            writer.Write("Microsoft.Xna.Framework.Content.SongReader"); // 43 bytes (1 length + 42 chars)
            // reader version
            writer.Write(0); // Int32 version: 4 bytes

            // Type reader 2: Int32Reader
            writer.Write("Microsoft.Xna.Framework.Content.Int32Reader"); // 44 bytes (1 length + 43 chars)
            // reader version
            writer.Write(0); // Int32 version: 4 bytes

            // Shared resource count (zero)
            writer.Write((byte)0); // 1 byte

            // Primary object: type ID 1 (SongReader)
            writer.Write((byte)1); // 1 byte

            // From XNB docs:
            // Song
            // Target type: Microsoft.Xna.Framework.Media.Song
            // Type reader name: Microsoft.Xna.Framework.Content.SongReader

            // Streaming filename
            writer.Write(streamingFilename);

            // Duration as polymorphic Int32: type ID 2 (Int32Reader)
            writer.Write((byte)2); // 1 byte

            // Duration in milliseconds
            writer.Write(durationMs); // Int32: 4 bytes
        }
    }
}
