using AVShinin.Serializer.Objects;
using AVShinin.Serializer.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace AVShinin.Serializer
{
    public class ObjectWriter
    {
        public byte[] Serialize(SerializeObjectDictionary dict)
        {
            var count = dict.GetBytesCount(true);
            var writer = new ByteWriter(count);

            this.WriteDictionary(writer, dict);

            return writer.Buffer;
        }

        public void WriteDictionary(ByteWriter writer, SerializeObjectDictionary dict)
        {
            writer.Write(dict.GetBytesCount(false));

            foreach (var key in dict.Keys)
            {
                this.WriteElement(writer, key, dict[key] ?? SerializeObject.Null);
            }

            writer.Write((byte)0x00);
        }

        public void WriteArray(ByteWriter writer, SerializeObjectArray array)
        {
            writer.Write(array.GetBytesCount(false));

            for (var i = 0; i < array.Count; i++)
            {
                this.WriteElement(writer, i.ToString(), array[i] ?? SerializeObject.Null);
            }

            writer.Write((byte)0x00);
        }

        private void WriteElement(ByteWriter writer, string key, SerializeObject value)
        {
            // cast RawValue to avoid one if on As<Type>
            switch (value.Type)
            {
                case TypeObject.Double:
                    writer.Write((byte)TypeObject.Double);
                    this.WriteCString(writer, key);
                    writer.Write((Double)value.Value);
                    break;

                case TypeObject.String:
                    writer.Write((byte)TypeObject.String);
                    this.WriteCString(writer, key);
                    this.WriteString(writer, (String)value.Value);
                    break;

                case TypeObject.Dictionary:
                    writer.Write((byte)TypeObject.Dictionary);
                    this.WriteCString(writer, key);
                    this.WriteDictionary(writer, new SerializeObjectDictionary((Dictionary<string, SerializeObject>)value.Value));
                    break;

                case TypeObject.Array:
                    writer.Write((byte)TypeObject.Array);
                    this.WriteCString(writer, key);
                    this.WriteArray(writer, new SerializeObjectArray((List<SerializeObject>)value.Value));
                    break;

                case TypeObject.Binary:
                    writer.Write((byte)TypeObject.Binary);
                    this.WriteCString(writer, key);
                    var bytes = (byte[])value.Value;
                    writer.Write(bytes.Length);
                    writer.Write((byte)0x00); // subtype 00 - Generic binary subtype
                    writer.Write(bytes);
                    break;

                case TypeObject.Guid:
                    writer.Write((byte)TypeObject.Guid);
                    this.WriteCString(writer, key);
                    var guid = ((Guid)value.Value).ToByteArray();
                    writer.Write(guid.Length);
                    writer.Write((byte)0x04); // UUID
                    writer.Write(guid);
                    break;

                case TypeObject.Boolean:
                    writer.Write((byte)TypeObject.Boolean);
                    this.WriteCString(writer, key);
                    writer.Write((byte)(((Boolean)value.Value) ? 0x01 : 0x00));
                    break;

                case TypeObject.DateTime:
                    writer.Write((byte)TypeObject.DateTime);
                    this.WriteCString(writer, key);
                    var date = (DateTime)value.Value;

                    var utc = (date == DateTime.MinValue || date == DateTime.MaxValue) ? date : date.ToUniversalTime();
                    var ts = utc - SerializeObject.UnixEpoch;
                    writer.Write(Convert.ToInt64(ts.TotalMilliseconds));
                    break;

                case TypeObject.Null:
                    writer.Write((byte)TypeObject.Null);
                    this.WriteCString(writer, key);
                    break;

                case TypeObject.Int32:
                    writer.Write((byte)TypeObject.Int32);
                    this.WriteCString(writer, key);
                    writer.Write((Int32)value.Value);
                    break;

                case TypeObject.Int64:
                    writer.Write((byte)TypeObject.Int64);
                    this.WriteCString(writer, key);
                    writer.Write((Int64)value.Value);
                    break;

                case TypeObject.Decimal:
                    writer.Write((byte)TypeObject.Decimal);
                    this.WriteCString(writer, key);
                    writer.Write((Decimal)value.Value);
                    break;
            }
        }

        private void WriteString(ByteWriter writer, string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            writer.Write(bytes.Length + 1);
            writer.Write(bytes);
            writer.Write((byte)0x00);
        }

        private void WriteCString(ByteWriter writer, string s)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            writer.Write(bytes);
            writer.Write((byte)0x00);
        }
    }
}