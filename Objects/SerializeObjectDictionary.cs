using AVShinin.Serializer.Utils.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVShinin.Serializer.Objects
{
    public class SerializeObjectDictionary : SerializeObject, IDictionary<string, SerializeObject>
    {
        #region Constructors
        public SerializeObjectDictionary() : base(new Dictionary<string, SerializeObject>()) { }

        public SerializeObjectDictionary(Dictionary<string, SerializeObject> dict) : base(dict)
        {
            if (dict == null) throw new ArgumentNullException("dict");
        }
        #endregion

        public new Dictionary<string, SerializeObject> Value { get { return (Dictionary<string, SerializeObject>)base.Value; } }

        public SerializeObject this[string key] { get { return Value.GetOrDefault(key, Null); }set { if (!IsValidFieldName(key)) throw new ArgumentException($"Field '{key}' has an invalid name."); this.Value[key] = value ?? Null; } }
        internal static bool IsValidFieldName(string field)
        {
            if (string.IsNullOrEmpty(field)) return false;
            return true;
        }

        #region Get/Set methods

        public SerializeObject Get(string path)
        {
            // supports parent.child.name
            var names = path.Split('.');

            if (names.Length == 1)
            {
                return this[path];
            }

            var value = this;

            for (var i = 0; i < names.Length - 1; i++)
            {
                var name = names[i];

                if (value[name].IsDictionary)
                {
                    value = value[name].AsDictionary;
                }
                else
                {
                    return Null;
                }
            }

            return value[names.Last()];
        }
        public SerializeObjectDictionary Set(string path, SerializeObject value)
        {
            // supports parent.child.name
            var names = path.Split('.');

            if (names.Length == 1)
            {
                this[path] = value;
                return this;
            }

            var master = this;

            // walk on path creating object when do not exists
            for (var i = 0; i < names.Length - 1; i++)
            {
                var name = names[i];

                if (master[name].IsDictionary)
                {
                    master = master[name].AsDictionary;
                }
                else if (master[name].IsNull)
                {
                    var d = new SerializeObjectDictionary();
                    master[name] = d;
                    master = d;
                }
                else
                {
                    return this;
                }
            }

            master[names.Last()] = value;

            return this;
        }
        public IEnumerable<SerializeObject> GetValues(string path, bool distinct = false, bool singleValue = false)
        {
            if (singleValue)
            {
                yield return this.Get(path);
            }
            else if (path.IndexOf(".") == -1)
            {
                var value = this[path];

                if (value.IsArray)
                {
                    var items = this.GetKeyValues(value, path);

                    foreach (var item in distinct ? items.Distinct() : items)
                    {
                        yield return item;
                    }
                }
                else
                {
                    yield return value;
                }
            }
            else
            {
                var items = this.GetKeyValues(this, path);

                foreach (var item in /*distinct ? items.Distinct() :*/ items)
                {
                    yield return item;
                }
            }
        }
        private IEnumerable<SerializeObject> GetKeyValues(SerializeObject value, string path)
        {
            if (value.IsArray)
            {
                foreach (var item in value.AsArray)
                {
                    foreach (var v in this.GetKeyValues(item, path))
                    {
                        yield return v;
                    }
                }
            }
            else if (value.IsDictionary && path != null)
            {
                var dot = path.IndexOf(".");
                var dictValue = value.AsDictionary[dot == -1 ? path : path.Substring(0, dot)];
                var rpath = dot == -1 ? null : path.Substring(dot + 1);

                foreach (var v in this.GetKeyValues(dictValue, rpath))
                {
                    yield return v;
                }
            }
            else
            {
                yield return value;
            }
        }

        #endregion

        #region CompareTo
        public override int CompareTo(SerializeObject other)
        {
            // if types are different, returns sort type order
            if (other.Type != TypeObject.Dictionary) return this.Type.CompareTo(other.Type);

            var thisKeys = this.Keys.ToArray();
            var thisLength = thisKeys.Length;

            var othermaster = other.AsDictionary;
            var otherKeys = othermaster.Keys.ToArray();
            var otherLength = otherKeys.Length;

            var result = 0;
            var i = 0;
            var stop = Math.Min(thisLength, otherLength);

            for (; 0 == result && i < stop; i++)
                result = this[thisKeys[i]].CompareTo(othermaster[thisKeys[i]]);

            // are different
            if (result != 0) return result;

            // test keys length to check which is bigger
            if (i == thisLength) return i == otherLength ? 0 : -1;
            return 1;
        }
        #endregion

        #region IDictionary
        public ICollection<string> Keys { get { return Value.Keys; } }
        public ICollection<SerializeObject> Values { get { return Value.Values; } }

        public int Count { get { return Value.Count; } }

        public bool IsReadOnly { get { return false; } }

        public void Add(KeyValuePair<string, SerializeObject> item)
        {
            this[item.Key] = item.Value;
        }

        public void Add(string key, SerializeObject value)
        {
            this[key] = value;
        }

        public void Clear()
        {
            Value.Clear();
        }

        public bool Contains(KeyValuePair<string, SerializeObject> item)
        {
            return Value.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return Value.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, SerializeObject>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, SerializeObject>>)this.Value).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, SerializeObject>> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, SerializeObject> item)
        {
            return Value.Remove(item.Key);
        }

        public bool Remove(string key)
        {
            return Value.Remove(key);
        }

        public bool TryGetValue(string key, out SerializeObject value)
        {
            return Value.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Value.GetEnumerator();
        }
        #endregion
    }
}
