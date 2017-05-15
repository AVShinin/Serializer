using AVShinin.Serializer.Objects;
using AVShinin.Serializer.Utils;
using System;
using System.Text;

namespace AVShinin.Serializer
{
    public class ObjectReader
    {
        public SerializeObjectDictionary Deserialize(byte[] bson)
        {
            return this.ReadDictionary(new ByteReader(bson));
        }
        public SerializeObjectDictionary ReadDictionary(ByteReader reader)
        {
            var length = reader.ReadInt32();
            var end = reader.Position + length - 5;
            var obj = new SerializeObjectDictionary();

            while (reader.Position < end)
            {
                string name;
                var value = this.ReadElement(reader, out name);
                obj.Value[name] = value;
            }

            reader.ReadByte(); // zero

            return obj;
        }
        public SerializeObjectArray ReadArray(ByteReader reader)
        {
            var length = reader.ReadInt32();
            var end = reader.Position + length - 5;
            var arr = new SerializeObjectArray();

            while (reader.Position < end)
            {
                string name;
                var value = this.ReadElement(reader, out name);
                arr.Add(value);
            }

            reader.ReadByte(); // zero

            return arr;
        }

        private SerializeObject ReadElement(ByteReader reader, out string name)
        {
            var type = reader.ReadByte();
            name = this.ReadCString(reader);

            if (type == (byte)TypeObject.Double) // Double
            {
                return reader.ReadDouble();
            }
            else if (type == (byte)TypeObject.String) // String
            {
                return this.ReadString(reader);
            }
            else if (type == (byte)TypeObject.Dictionary) // Dictionary
            {
                return this.ReadDictionary(reader);
            }
            else if (type == (byte)TypeObject.Array) // Array
            {
                return this.ReadArray(reader);
            }
            else if (type == (byte)TypeObject.Binary) // Binary
            {
                var length = reader.ReadInt32();
                var subType = reader.ReadByte();
                var bytes = reader.ReadBytes(length);

                switch (subType)
                {
                    case 0x00: return bytes;
                    case 0x04: return new Guid(bytes);
                }
            }
            else if (type == (byte)TypeObject.Boolean) // Boolean
            {
                return reader.ReadBoolean();
            }
            else if (type == (byte)TypeObject.DateTime) // DateTime
            {
                var ts = reader.ReadInt64();

                // catch specific values for MaxValue / MinValue #19
                if (ts == 253402300800000) return DateTime.MaxValue;
                if (ts == -62135596800000) return DateTime.MinValue;

                return SerializeObject.UnixEpoch.AddMilliseconds(ts).ToLocalTime();
            }
            else if (type == (byte)TypeObject.Null) // Null
            {
                return SerializeObject.Null;
            }
            else if (type == (byte)TypeObject.Int32) // Int32
            {
                return reader.ReadInt32();
            }
            else if (type == (byte)TypeObject.Int64) // Int64
            {
                return reader.ReadInt64();
            }
            else if (type == (byte)TypeObject.Decimal) // Decimal
            {
                return reader.ReadDecimal();
            }
            throw new NotSupportedException("BSON type not supported");
        }

        private string ReadString(ByteReader reader)
        {
            var length = reader.ReadInt32();
            var bytes = reader.ReadBytes(length - 1);
            reader.ReadByte(); // discard \x00
            return Encoding.UTF8.GetString(bytes, 0, length - 1);
        }

        // use byte array buffer for CString (key-only)
        private byte[] _strBuffer = new byte[1000];

        private string ReadCString(ByteReader reader)
        {
            var pos = 0;

            while (true)
            {
                byte buf = reader.ReadByte();
                if (buf == 0x00) break;
                _strBuffer[pos++] = buf;
            }

            return Encoding.UTF8.GetString(_strBuffer, 0, pos);
        }
    }
}