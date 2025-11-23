namespace XNBVerter.Core
{
    internal static class BinaryWriterExtensions
    {
        public static void Write7BitEncodedInt(this BinaryWriter writer, int value)
        {
            // Standard 7-bit encoded int format used by .NET
            uint v = (uint)value;

            while (v >= 0x80)
            {
                writer.Write((byte)(v | 0x80));
                v >>= 7;
            }

            writer.Write((byte)v);
        }
    }
}
