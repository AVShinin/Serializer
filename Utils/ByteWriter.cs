using AVShinin.Serializer.Objects;
using System;
using System.Text;

namespace AVShinin.Serializer.Utils
{
    public class ByteWriter
    {
        private byte[] _buffer;
        private int _pos;

        public byte[] Buffer { get { return _buffer; } }

        public int Position { get { return _pos; } }

        public ByteWriter(int length)
        {
            _buffer = new byte[length];
            _pos = 0;
        }

        public ByteWriter(byte[] buffer)
        {
            _buffer = buffer;
            _pos = 0;
        }

        public void Skip(int length)
        {
            _pos += length;
        }

        #region Native data types

        public void Write(Byte value)
        {
            _buffer[_pos] = value;

            _pos++;
        }

        public void Write(Boolean value)
        {
            _buffer[_pos] = value ? (byte)1 : (byte)0;

            _pos++;
        }

        public void Write(UInt16 value)
        {
            var pi = BitConverter.GetBytes(value);

            _buffer[_pos + 0] = pi[0];
            _buffer[_pos + 1] = pi[1];

            _pos += 2;
        }

        public void Write(UInt32 value)
        {
            var pi = BitConverter.GetBytes(value);

            _buffer[_pos + 0] = pi[0];
            _buffer[_pos + 1] = pi[1];
            _buffer[_pos + 2] = pi[2];
            _buffer[_pos + 3] = pi[3];

            _pos += 4;
        }

        public void Write(UInt64 value)
        {
            var pi = BitConverter.GetBytes(value);

            _buffer[_pos + 0] = pi[0];
            _buffer[_pos + 1] = pi[1];
            _buffer[_pos + 2] = pi[2];
            _buffer[_pos + 3] = pi[3];
            _buffer[_pos + 4] = pi[4];
            _buffer[_pos + 5] = pi[5];
            _buffer[_pos + 6] = pi[6];
            _buffer[_pos + 7] = pi[7];

            _pos += 8;
        }

        public void Write(Int16 value)
        {
            var pi = BitConverter.GetBytes(value);

            _buffer[_pos + 0] = pi[0];
            _buffer[_pos + 1] = pi[1];

            _pos += 2;
        }

        public void Write(Int32 value)
        {
            var pi = BitConverter.GetBytes(value);

            _buffer[_pos + 0] = pi[0];
            _buffer[_pos + 1] = pi[1];
            _buffer[_pos + 2] = pi[2];
            _buffer[_pos + 3] = pi[3];

            _pos += 4;
        }

        public void Write(Int64 value)
        {
            var pi = BitConverter.GetBytes(value);

            _buffer[_pos + 0] = pi[0];
            _buffer[_pos + 1] = pi[1];
            _buffer[_pos + 2] = pi[2];
            _buffer[_pos + 3] = pi[3];
            _buffer[_pos + 4] = pi[4];
            _buffer[_pos + 5] = pi[5];
            _buffer[_pos + 6] = pi[6];
            _buffer[_pos + 7] = pi[7];

            _pos += 8;
        }

        public void Write(Single value)
        {
            var pi = BitConverter.GetBytes(value);

            _buffer[_pos + 0] = pi[0];
            _buffer[_pos + 1] = pi[1];
            _buffer[_pos + 2] = pi[2];
            _buffer[_pos + 3] = pi[3];

            _pos += 4;
        }

        public void Write(Double value)
        {
            var pi = BitConverter.GetBytes(value);

            _buffer[_pos + 0] = pi[0];
            _buffer[_pos + 1] = pi[1];
            _buffer[_pos + 2] = pi[2];
            _buffer[_pos + 3] = pi[3];
            _buffer[_pos + 4] = pi[4];
            _buffer[_pos + 5] = pi[5];
            _buffer[_pos + 6] = pi[6];
            _buffer[_pos + 7] = pi[7];

            _pos += 8;
        }

        public void Write(Decimal value)
        {
            var array = Decimal.GetBits(value);

            this.Write(array[0]);
            this.Write(array[1]);
            this.Write(array[2]);
            this.Write(array[3]);
        }

        public void Write(Byte[] value)
        {
            System.Buffer.BlockCopy(value, 0, _buffer, _pos, value.Length);

            _pos += value.Length;
        }

        #endregion Native data types

        #region Extended types

        public void Write(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            this.Write(bytes.Length);
            this.Write(bytes);
        }

        public void Write(string value, int length)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            if (bytes.Length != length) throw new ArgumentException("Invalid string length");
            this.Write(bytes);
        }

        public void Write(DateTime value)
        {
            this.Write(value.ToUniversalTime().Ticks);
        }

        public void Write(Guid value)
        {
            this.Write(value.ToByteArray());
        }

        public void WriteBsonValue(SerializeObject value, ushort length)
        {
            this.Write((byte)value.Type);

            switch (value.Type)
            {
                case TypeObject.Null:
                    break;

                case TypeObject.Int32: this.Write((Int32)value.Value); break;
                case TypeObject.Int64: this.Write((Int64)value.Value); break;
                case TypeObject.Double: this.Write((Double)value.Value); break;

                case TypeObject.String: this.Write((String)value.Value, length); break;

                case TypeObject.Dictionary: new ObjectWriter().WriteDictionary(this, value.AsDictionary); break;
                case TypeObject.Array: new ObjectWriter().WriteArray(this, value.AsArray); break;

                case TypeObject.Binary: this.Write((Byte[])value.Value); break;
                case TypeObject.Guid: this.Write((Guid)value.Value); break;

                case TypeObject.Boolean: this.Write((Boolean)value.Value); break;
                case TypeObject.DateTime: this.Write((DateTime)value.Value); break;

                default: throw new NotImplementedException();
            }
        }

        #endregion Extended types
    }
}