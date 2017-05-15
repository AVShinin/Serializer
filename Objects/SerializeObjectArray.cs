using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVShinin.Serializer.Objects
{
    public class SerializeObjectArray : SerializeObject, IList<SerializeObject>
    {
        #region Constructors
        public SerializeObjectArray() : base(new List<SerializeObject>()) { }
        public SerializeObjectArray(List<SerializeObject> array) : base(array)
        {
            if (array == null) throw new ArgumentNullException("array");
        }
        public SerializeObjectArray(SerializeObject[] array) : base(new List<SerializeObject>(array))
        {
            if (array == null) throw new ArgumentNullException("array");
        }
        public SerializeObjectArray(IEnumerable<SerializeObject> items) : this()
        {
            this.AddRange<SerializeObject>(items);
        }
        public SerializeObjectArray(IEnumerable<SerializeObjectArray> items) : this()
        {
            this.AddRange<SerializeObjectArray>(items);
        }
        public SerializeObjectArray(IEnumerable<SerializeObjectDictionary> items) : this()
        {
            this.AddRange<SerializeObjectDictionary>(items);
        }
        #endregion

        public new List<SerializeObject> Value { get { return (List<SerializeObject>)base.Value; } }
        public SerializeObject this[int index] { get { return Value.ElementAt(index); } set { Value[index] = value ?? Null; } }

        #region IList
        public int Count { get { return Value.Count; } }

        public bool IsReadOnly { get { return false; } }

        public void Add(SerializeObject item)
        {
            Value.Add(item ?? Null);
        }
        public virtual void AddRange<T>(IEnumerable<T> array) where T : SerializeObject
        {
            if (array == null) throw new ArgumentNullException("array");

            foreach (var item in array)
            {
                this.Add(item ?? Null);
            }
        }

        public void Clear()
        {
            Value.Clear();
        }

        public bool Contains(SerializeObject item)
        {
            return Value.Contains(item);
        }

        public void CopyTo(SerializeObject[] array, int arrayIndex)
        {
            Value.CopyTo(array, arrayIndex);
        }

        public IEnumerator<SerializeObject> GetEnumerator()
        {
            return Value.GetEnumerator();
        }

        public int IndexOf(SerializeObject item)
        {
            return Value.IndexOf(item);
        }

        public void Insert(int index, SerializeObject item)
        {
            Value.Insert(index, item);
        }

        public bool Remove(SerializeObject item)
        {
            return Value.Remove(item);
        }

        public void RemoveAt(int index)
        {
            Value.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var value in this.Value)
            {
                yield return new SerializeObject(value);
            }
        }
        #endregion

        #region CompareTo
        public override int CompareTo(SerializeObject other)
        {
            // if types are different, returns sort type order
            if (other.Type != TypeObject.Dictionary) return this.Type.CompareTo(other.Type);

            var otherArray = other.AsArray;

            var result = 0;
            var i = 0;
            var stop = Math.Min(this.Count, otherArray.Count);

            // compare each element
            for (; 0 == result && i < stop; i++)
                result = this[i].CompareTo(otherArray[i]);

            if (result != 0) return result;
            if (i == this.Count) return i == otherArray.Count ? 0 : -1;
            return 1;
        }
        #endregion
    }
}
