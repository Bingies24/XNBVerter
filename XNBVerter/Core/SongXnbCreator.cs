namespace XNBVerter.Core
{
    internal sealed class SongXnbCreator
    {
        private const int XnbSongHeaderOverhead = 114;

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

        private static void WriteSongHeader(BinaryWriter writer, string inputFile, int durationMs)
        {
            string fileNameOnly = Path.GetFileName(inputFile);

            // "XNB"
            writer.Write("XNB".ToCharArray());

            // Target platform "w" (Windows)
            writer.Write("w".ToCharArray());

            // XNB format version
            writer.Write((byte)5);

            // Flags
            writer.Write((byte)0);

            // File size (header + filename)
            int fileSize = fileNameOnly.Length + XnbSongHeaderOverhead;
            writer.Write(fileSize);

            // Type reader count
            writer.Write7BitEncodedInt(2);

            // Type reader 1: SongReader
            writer.Write("Microsoft.Xna.Framework.Content.SongReader");
            writer.Write(0); // reader version

            // Type reader 2: Int32Reader
            writer.Write("Microsoft.Xna.Framework.Content.Int32Reader");
            writer.Write(0); // reader version

            // Shared resources?
            writer.Write((byte)0);
            writer.Write((byte)1);

            // Streaming filename
            writer.Write(fileNameOnly);

            // Int32 Object ID?
            writer.Write((byte)2);

            // Duration in milliseconds
            writer.Write(durationMs);
        }
    }
}
