using AVShinin.Serializer.Utils.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVShinin.Serializer.Objects
{
    public class SerializeObject : IComparable<SerializeObject>, IEquatable<SerializeObject>
    {
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static SerializeObject Null = new SerializeObject();

        public virtual object Value { get; private set; }
        public TypeObject Type { get; private set; }

        #region Constructors
        public SerializeObject()
        {
            Type = TypeObject.Null;
            Value = null;
        }
        public SerializeObject(Int32 value)
        {
            Type = TypeObject.Int32;
            Value = value;
        }
        public SerializeObject(Int64 value)
        {
            Type = TypeObject.Int64;
            Value = value;
        }
        public SerializeObject(Double value)
        {
            Type = TypeObject.Double;
            Value = value;
        }
        public SerializeObject(Decimal value)
        {
            Type = TypeObject.Decimal;
            Value = value;
        }
        public SerializeObject(String value)
        {
            Type = value == null ? TypeObject.Null : TypeObject.String;
            Value = value;
        }
        public SerializeObject(Byte[] value)
        {
            Type = value == null ? TypeObject.Null : TypeObject.Binary;
            Value = value;
        }
        public SerializeObject(Guid value)
        {
            Type = TypeObject.Guid;
            Value = value;
        }
        public SerializeObject(Boolean value)
        {
            Type = TypeObject.Boolean;
            Value = value;
        }
        public SerializeObject(DateTime value)
        {
            Type = TypeObject.DateTime;
            Value = value;
        }
        public SerializeObject(Dictionary<string, SerializeObject> value)
        {
            Type = value == null ? TypeObject.Null : TypeObject.Dictionary;
            Value = value;
        }
        public SerializeObject(List<SerializeObject> value)
        {
            Type = value == null ? TypeObject.Null : TypeObject.Array;
            Value = value;
        }
        public SerializeObject(SerializeObject value)
        {
            Type = value == null ? TypeObject.Null : value.Type;
            Value = value.Value;
        }
        public SerializeObject(Object value)
        {
            Value = value;

            if (value == null) this.Type = TypeObject.Null;
            else if (value is Int32) this.Type = TypeObject.Int32;
            else if (value is Int64) this.Type = TypeObject.Int64;
            else if (value is Double) this.Type = TypeObject.Double;
            else if (value is Decimal) this.Type = TypeObject.Decimal;
            else if (value is String) this.Type = TypeObject.String;
            else if (value is Dictionary<string, SerializeObject>) this.Type = TypeObject.Dictionary;
            else if (value is List<SerializeObject>) this.Type = TypeObject.Array;
            else if (value is Byte[]) this.Type = TypeObject.Binary;
            else if (value is Guid) this.Type = TypeObject.Guid;
            else if (value is Boolean) this.Type = TypeObject.Boolean;
            else if (value is DateTime) this.Type = TypeObject.DateTime;
            else if (value is SerializeObject)
            {
                var v = (SerializeObject)value;
                this.Type = v.Type;
                this.Value = v.Value;
            }
            else
            {
                var enumerable = value as IEnumerable;
                var dictionary = value as IDictionary;

                if (dictionary != null)
                {
                    var dict = new Dictionary<string, SerializeObject>();

                    foreach (var key in dictionary.Keys)
                    {
                        dict.Add(key.ToString(), new SerializeObject(dictionary[key]));
                    }

                    this.Type = TypeObject.Dictionary;
                    this.Value = dict;
                }
                else if (enumerable != null)
                {
                    var list = new List<SerializeObject>();

                    foreach (var x in enumerable)
                    {
                        list.Add(new SerializeObject(x));
                    }

                    this.Type = TypeObject.Array;
                    this.Value = list;
                }
                else
                {
                    throw new InvalidCastException("Не возможно определить тип!");
                }
            }
        }
        #endregion

        #region Convert types

        public SerializeObjectArray AsArray
        {
            get
            {
                if (this.IsArray)
                {
                    var array = new SerializeObjectArray((List<SerializeObject>)this.Value);
                    array.Length = this.Length;

                    return array;
                }
                else
                {
                    return default(SerializeObjectArray);
                }
            }
        }

        public SerializeObjectDictionary AsDictionary
        {
            get
            {
                if (this.IsDictionary)
                {
                    var dict = new SerializeObjectDictionary((Dictionary<string, SerializeObject>)this.Value);
                    dict.Length = this.Length;

                    return dict;
                }
                else
                {
                    return default(SerializeObjectDictionary);
                }
            }
        }

        public Byte[] AsBinary
        {
            get { return this.Type == TypeObject.Binary ? (Byte[])this.Value : default(Byte[]); }
        }

        public bool AsBoolean
        {
            get { return this.Type == TypeObject.Boolean ? (Boolean)this.Value : default(Boolean); }
        }

        public string AsString
        {
            get { return this.Type != TypeObject.Null ? this.Value.ToString() : default(String); }
        }

        public int AsInt32
        {
            get { return this.IsNumber ? Convert.ToInt32(this.Value) : default(Int32); }
        }

        public long AsInt64
        {
            get { return this.IsNumber ? Convert.ToInt64(this.Value) : default(Int64); }
        }

        public double AsDouble
        {
            get { return this.IsNumber ? Convert.ToDouble(this.Value) : default(Double); }
        }

        public decimal AsDecimal
        {
            get { return this.IsNumber ? Convert.ToDecimal(this.Value) : default(Decimal); }
        }

        public DateTime AsDateTime
        {
            get { return this.Type == TypeObject.DateTime ? (DateTime)this.Value : default(DateTime); }
        }

        public Guid AsGuid
        {
            get { return this.Type == TypeObject.Guid ? (Guid)this.Value : default(Guid); }
        }

        #endregion

        #region IsTypes

        public bool IsNull
        {
            get { return this.Type == TypeObject.Null; }
        }

        public bool IsArray
        {
            get { return this.Type == TypeObject.Array; }
        }

        public bool IsDictionary
        {
            get { return this.Type == TypeObject.Dictionary; }
        }

        public bool IsInt32
        {
            get { return this.Type == TypeObject.Int32; }
        }

        public bool IsInt64
        {
            get { return this.Type == TypeObject.Int64; }
        }

        public bool IsDouble
        {
            get { return this.Type == TypeObject.Double; }
        }

        public bool IsDecimal
        {
            get { return this.Type == TypeObject.Decimal; }
        }

        public bool IsNumber
        {
            get { return this.IsInt32 || this.IsInt64 || this.IsDouble || this.IsDecimal; }
        }

        public bool IsBinary
        {
            get { return this.Type == TypeObject.Binary; }
        }

        public bool IsBoolean
        {
            get { return this.Type == TypeObject.Boolean; }
        }

        public bool IsString
        {
            get { return this.Type == TypeObject.String; }
        }

        public bool IsGuid
        {
            get { return this.Type == TypeObject.Guid; }
        }

        public bool IsDateTime
        {
            get { return this.Type == TypeObject.DateTime; }
        }

        #endregion

        #region Implicit Ctor

        // Int32
        public static implicit operator Int32(SerializeObject value)
        {
            return (Int32)value.Value;
        }

        // Int32
        public static implicit operator SerializeObject(Int32 value)
        {
            return new SerializeObject(value);
        }

        // Int64
        public static implicit operator Int64(SerializeObject value)
        {
            return (Int64)value.Value;
        }

        // Int64
        public static implicit operator SerializeObject(Int64 value)
        {
            return new SerializeObject(value);
        }

        // Double
        public static implicit operator Double(SerializeObject value)
        {
            return (Double)value.Value;
        }

        // Double
        public static implicit operator SerializeObject(Double value)
        {
            return new SerializeObject(value);
        }

        // Decimal
        public static implicit operator Decimal(SerializeObject value)
        {
            return (Decimal)value.Value;
        }

        // Decimal
        public static implicit operator SerializeObject(Decimal value)
        {
            return new SerializeObject(value);
        }

        // UInt64 (to avoid ambigous between Double-Decimal)
        public static implicit operator UInt64(SerializeObject value)
        {
            return (UInt64)value.Value;
        }

        // Decimal
        public static implicit operator SerializeObject(UInt64 value)
        {
            return new SerializeObject((Double)value);
        }

        // String
        public static implicit operator String(SerializeObject value)
        {
            return (String)value.Value;
        }

        // String
        public static implicit operator SerializeObject(String value)
        {
            return new SerializeObject(value);
        }

        // Dictionary
        public static implicit operator Dictionary<string, SerializeObject>(SerializeObject value)
        {
            return (Dictionary<string, SerializeObject>)value.Value;
        }

        // Dictionary
        public static implicit operator SerializeObject(Dictionary<string, SerializeObject> value)
        {
            return new SerializeObject(value);
        }

        // Array
        public static implicit operator List<SerializeObject>(SerializeObject value)
        {
            return (List<SerializeObject>)value.Value;
        }

        // Array
        public static implicit operator SerializeObject(List<SerializeObject> value)
        {
            return new SerializeObject(value);
        }

        // Binary
        public static implicit operator Byte[] (SerializeObject value)
        {
            return (Byte[])value.Value;
        }

        // Binary
        public static implicit operator SerializeObject(Byte[] value)
        {
            return new SerializeObject(value);
        }

        // Guid
        public static implicit operator Guid(SerializeObject value)
        {
            return (Guid)value.Value;
        }

        // Guid
        public static implicit operator SerializeObject(Guid value)
        {
            return new SerializeObject(value);
        }

        // Boolean
        public static implicit operator Boolean(SerializeObject value)
        {
            return (Boolean)value.Value;
        }

        // Boolean
        public static implicit operator SerializeObject(Boolean value)
        {
            return new SerializeObject(value);
        }

        // DateTime
        public static implicit operator DateTime(SerializeObject value)
        {
            return (DateTime)value.Value;
        }

        // DateTime
        public static implicit operator SerializeObject(DateTime value)
        {
            return new SerializeObject(value);
        }

        public override string ToString()
        {
            return this.IsNull ? "(null)" : this.Value.ToString();
        }

        #endregion

        #region IComparable<SerializeObject>, IEquatable<SerializeObject>

        public virtual int CompareTo(SerializeObject other)
        {
            if (this.Type != other.Type)
            {
                if (this.IsNumber && other.IsNumber)
                {
                    return Convert.ToDecimal(this.Value).CompareTo(Convert.ToDecimal(other.Value));
                }
                else
                {
                    return this.Type.CompareTo(other.Type);
                }
            }

            switch (this.Type)
            {
                case TypeObject.Null:
                    return 0;

                case TypeObject.Int32: return ((Int32)this.Value).CompareTo((Int32)other.Value);
                case TypeObject.Int64: return ((Int64)this.Value).CompareTo((Int64)other.Value);
                case TypeObject.Double: return ((Double)this.Value).CompareTo((Double)other.Value);
                case TypeObject.Decimal: return ((Decimal)this.Value).CompareTo((Decimal)other.Value);

                case TypeObject.String: return string.Compare((String)this.Value, (String)other.Value);

                case TypeObject.Dictionary: return this.AsDictionary.CompareTo(other);
                case TypeObject.Array: return this.AsArray.CompareTo(other);

                case TypeObject.Binary: return ((Byte[])this.Value).BinaryCompareTo((Byte[])other.Value);
                case TypeObject.Guid: return ((Guid)this.Value).CompareTo((Guid)other.Value);

                case TypeObject.Boolean: return ((Boolean)this.Value).CompareTo((Boolean)other.Value);
                case TypeObject.DateTime:
                    var d0 = (DateTime)this.Value;
                    var d1 = (DateTime)other.Value;
                    if (d0.Kind != DateTimeKind.Utc) d0 = d0.ToUniversalTime();
                    if (d1.Kind != DateTimeKind.Utc) d1 = d1.ToUniversalTime();
                    return d0.CompareTo(d1);

                default: throw new NotImplementedException();
            }
        }

        public bool Equals(SerializeObject other)
        {
            return this.CompareTo(other) == 0;
        }

        #endregion

        #region Operators

        public static bool operator ==(SerializeObject lhs, SerializeObject rhs)
        {
            if (object.ReferenceEquals(lhs, null)) return object.ReferenceEquals(rhs, null);
            if (object.ReferenceEquals(rhs, null)) return false; // don't check type because sometimes different types can be ==

            return lhs.Equals(rhs);
        }

        public static bool operator !=(SerializeObject lhs, SerializeObject rhs)
        {
            return !(lhs == rhs);
        }

        public static bool operator >=(SerializeObject lhs, SerializeObject rhs)
        {
            return lhs.CompareTo(rhs) >= 0;
        }

        public static bool operator >(SerializeObject lhs, SerializeObject rhs)
        {
            return lhs.CompareTo(rhs) > 0;
        }

        public static bool operator <(SerializeObject lhs, SerializeObject rhs)
        {
            return lhs.CompareTo(rhs) < 0;
        }

        public static bool operator <=(SerializeObject lhs, SerializeObject rhs)
        {
            return lhs.CompareTo(rhs) <= 0;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(new SerializeObject(obj));
        }

        public override int GetHashCode()
        {
            var hash = 17;
            hash = 37 * hash + this.Type.GetHashCode();
            hash = 37 * hash + this.Value.GetHashCode();
            return hash;
        }

        #endregion

        #region GetBytesCount

        internal int? Length = null;
        public int GetBytesCount(bool recalc)
        {
            if (recalc == false && this.Length.HasValue) return this.Length.Value;

            switch (this.Type)
            {
                case TypeObject.Null:
                    this.Length = 0; break;

                case TypeObject.Int32: this.Length = 4; break;
                case TypeObject.Int64: this.Length = 8; break;
                case TypeObject.Double: this.Length = 8; break;
                case TypeObject.Decimal: this.Length = 16; break;

                case TypeObject.String: this.Length = Encoding.UTF8.GetByteCount((string)this.Value); break;

                case TypeObject.Binary: this.Length = ((Byte[])this.Value).Length; break;
                case TypeObject.Guid: this.Length = 16; break;

                case TypeObject.Boolean: this.Length = 1; break;
                case TypeObject.DateTime: this.Length = 8; break;

                case TypeObject.Array:
                    var array = (List<SerializeObject>)this.Value;
                    this.Length = 5; // header + footer
                    for (var i = 0; i < array.Count; i++)
                    {
                        this.Length += this.GetBytesCountElement(i.ToString(), array[i] ?? SerializeObject.Null, recalc);
                    }
                    break;

                case TypeObject.Dictionary:
                    var master = (Dictionary<string, SerializeObject>)this.Value;
                    this.Length = 5; // header + footer
                    foreach (var key in master.Keys)
                    {
                        this.Length += this.GetBytesCountElement(key, master[key] ?? SerializeObject.Null, recalc);
                    }
                    break;
            }

            return this.Length.Value;
        }

        private int GetBytesCountElement(string key, SerializeObject value, bool recalc)
        {
            return
                1 + // element type
                Encoding.UTF8.GetByteCount(key) + // CString
                1 + // CString 0x00
                value.GetBytesCount(recalc) +
                (value.Type == TypeObject.String || value.Type == TypeObject.Binary || value.Type == TypeObject.Guid ? 5 : 0); // bytes.Length + 0x??
        }

        #endregion
    }
}
