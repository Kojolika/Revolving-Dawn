using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace utils
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver, IDictionary
    {
        public Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
        public List<TKey> keys = new List<TKey>();
        public List<TValue> values = new List<TValue>();

        public void OnAfterDeserialize()
        {
            dictionary = new Dictionary<TKey, TValue>();

            for (int index = 0; index != Math.Min(keys.Count, values.Count); index++)
            {
                dictionary.Add(keys[index], values[index]);
            }
        }

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach (KeyValuePair<TKey, TValue> keyValuePair in dictionary)
            {
                keys.Add(keyValuePair.Key);
                values.Add(keyValuePair.Value);
            }
        }

        public object this[object key] { get => dictionary; set => dictionary = (Dictionary<TKey, TValue>)value; }

        public bool IsFixedSize => false;

        public bool IsReadOnly => false;

        public ICollection Keys => dictionary.Keys;

        public ICollection Values => dictionary.Values;

        public int Count => dictionary.Count;

        public bool IsSynchronized => throw new NotImplementedException();

        public object SyncRoot => throw new NotImplementedException();

        public void Add(object key, object value)
        {
            Debug.Log("Type of key: "+key.GetType());
            Debug.Log("Type should be: "+typeof(TKey));
            if (key.GetType()!= typeof(TKey))
            {
                throw new Exception("Key is not a valid type");
            }
            if (value.GetType() != typeof(TValue))
            {
                throw new Exception("Value is not a valid type");
            }
            dictionary.Add((TKey)key, (TValue)value);
        }

        public void Clear() => dictionary.Clear();


        public bool Contains(object key)
        {
            if (key.GetType() != typeof(TKey))
            {
                throw new Exception("Key is not a valid type");
            }
            return dictionary.ContainsKey((TKey)key);
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public IDictionaryEnumerator GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

        public void Remove(object key)
        {
            if (key.GetType() != typeof(TKey))
            {
                throw new Exception("Key is not a valid type");
            }
            dictionary.Remove((TKey)key);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return dictionary.GetEnumerator();
        }

    }
}
