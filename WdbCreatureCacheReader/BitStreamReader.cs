using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;

namespace MS.Internal.Ink
{
    // import from MS.Internal.Ink.BitStreamReader
    public class BitStreamReader
    {
        public byte[]  Buffer { get; private set; }
        public int     Index  { get; private set; }

        private uint countBits;
        private byte partialByte;
        private int  cbitsInPartialByte;

        public bool EndOfStream
        {
            get { return 0u == this.countBits; }
        }

        public BitStreamReader(byte[] buffer)
        {
            this.Buffer = buffer;
            this.countBits = (uint)(buffer.Length * 8);
        }

        public BitStreamReader(byte[] buffer, int startIndex)
        {
            if (startIndex < 0 || startIndex >= buffer.Length)
                throw new ArgumentOutOfRangeException("startIndex");

            this.Buffer = buffer;
            this.Index = startIndex;
            this.countBits = (uint)((buffer.Length - startIndex) * 8);
        }

        public BitStreamReader(byte[] buffer, uint bufferLengthInBits)
            : this(buffer)
        {
            if ((ulong)bufferLengthInBits > (ulong)((long)(buffer.Length * 8)))
                throw new ArgumentOutOfRangeException("bufferLengthInBits", "InvalidBufferLength");

            this.countBits = bufferLengthInBits;
        }

        public long ReadUInt64(int countOfBits)
        {
            if (countOfBits > 64 || countOfBits <= 0)
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "CountOfBitsOutOfRange");

            long num = 0L;
            while (countOfBits > 0)
            {
                int num2 = 8;
                if (countOfBits < 8)
                    num2 = countOfBits;

                num <<= num2;
                byte b = this.ReadByte(num2);
                num |= (long)((ulong)b);
                countOfBits -= num2;
            }
            return num;
        }

        public ushort ReadUInt16(int countOfBits)
        {
            if (countOfBits > 16 || countOfBits <= 0)
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "CountOfBitsOutOfRange");

            ushort num = 0;
            while (countOfBits > 0)
            {
                int num2 = 8;
                if (countOfBits < 8)
                    num2 = countOfBits;

                num = (ushort)(num << num2);
                byte b = this.ReadByte(num2);
                num |= (ushort)b;
                countOfBits -= num2;
            }
            return num;
        }

        public uint ReadUInt16Reverse(int countOfBits)
        {
            if (countOfBits > 16 || countOfBits <= 0)
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "CountOfBitsOutOfRange");

            ushort num = 0;
            int num2 = 0;
            while (countOfBits > 0)
            {
                int num3 = 8;
                if (countOfBits < 8)
                    num3 = countOfBits;

                ushort num4 = (ushort)this.ReadByte(num3);
                num4 = (ushort)(num4 << num2 * 8);
                num |= num4;
                num2++;
                countOfBits -= num3;
            }
            return (uint)num;
        }

        public uint ReadUInt32(int countOfBits)
        {
            if (countOfBits > 32 || countOfBits <= 0)
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "CountOfBitsOutOfRange");

            uint num = 0u;
            while (countOfBits > 0)
            {
                int num2 = 8;
                if (countOfBits < 8)
                    num2 = countOfBits;

                num <<= num2;
                byte b = this.ReadByte(num2);
                num |= (uint)b;
                countOfBits -= num2;
            }
            return num;
        }

        public uint ReadUInt32Reverse(int countOfBits)
        {
            if (countOfBits > 32 || countOfBits <= 0)
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "CountOfBitsOutOfRange");

            uint num = 0u;
            int num2 = 0;
            while (countOfBits > 0)
            {
                int num3 = 8;
                if (countOfBits < 8)
                    num3 = countOfBits;

                uint num4 = (uint)this.ReadByte(num3);
                num4 <<= num2 * 8;
                num |= num4;
                num2++;
                countOfBits -= num3;
            }
            return num;
        }

        public bool ReadBit()
        {
            byte b = this.ReadByte(1);
            return (b & 1) == 1;
        }

        public byte ReadByte(int countOfBits)
        {
            if (this.EndOfStream)
                throw new EndOfStreamException("EndOfStreamReached");

            if (countOfBits > 8 || countOfBits <= 0)
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "CountOfBitsOutOfRange");

            if ((long)countOfBits > (long)((ulong)this.countBits))
                throw new ArgumentOutOfRangeException("countOfBits", countOfBits, "CountOfBitsGreatThanRemainingBits");

            this.countBits -= (uint)countOfBits;
            byte b;

            if (this.cbitsInPartialByte >= countOfBits)
            {
                int num = 8 - countOfBits;
                b = (byte)(this.partialByte >> num);
                this.partialByte = (byte)(this.partialByte << countOfBits);
                this.cbitsInPartialByte -= countOfBits;
            }
            else
            {
                byte b2 = this.Buffer[this.Index];
                this.Index++;
                int num2 = 8 - countOfBits;
                b = (byte)(this.partialByte >> num2);
                int num3 = Math.Abs(countOfBits - this.cbitsInPartialByte - 8);
                b |= (byte)(b2 >> num3);
                this.partialByte = (byte)(b2 << countOfBits - this.cbitsInPartialByte);
                this.cbitsInPartialByte = 8 - (countOfBits - this.cbitsInPartialByte);
            }
            return b;
        }

        #region Extensions

        public string ReadString2(int m_size)
        {
            if (m_size <= 1)
                return string.Empty;

            var str = Encoding.UTF8.GetString(Buffer, Index, m_size - 1);
            Index += (int)m_size;
            return str;
        }
        public byte ReadByte()
        {
            var val = Buffer[Index];
            ++Index;
            return val;
        }

        public short ReadInt16()
        {
            var val = BitConverter.ToInt16(Buffer, Index);
            Index += 2;
            return val;
        }

        public ushort ReadUInt16()
        {
            var val = BitConverter.ToUInt16(Buffer, Index);
            Index += 2;
            return val;
        }

        public int ReadInt32()
        {
            var val = BitConverter.ToInt32(Buffer, Index);
            Index += 4;
            return val;
        }

        public uint ReadUInt32()
        {
            var val = BitConverter.ToUInt32(Buffer, Index);
            Index += 4;
            return val;
        }

        public float ReadFloat()
        {
            var val = BitConverter.ToSingle(Buffer, Index);
            Index += 4;
            return val;
        }

        public string ReadEsqapedSqlString2(int m_size)
        {
            var str = ReadString2(m_size);
            if (string.IsNullOrWhiteSpace(str))
                return "NULL";
            return @"'" + str.Replace(@"'", @"\'").Replace("\"", "\\\"") + @"'";
        }

        #endregion
    }
}