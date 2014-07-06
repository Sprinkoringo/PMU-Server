namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class ListPair<TKey, TValue>
    {
        #region Fields

        List<TKey> mKeys;
        List<TValue> mValues;

        #endregion Fields

        #region Constructors

        public ListPair()
        {
            mKeys = new List<TKey>();
            mValues = new List<TValue>();
        }

        #endregion Constructors

        #region Properties

        public int Count
        {
            get { return mKeys.Count; }
        }

        public List<TKey> Keys
        {
            get { return mKeys; }
        }

        public List<TValue> Values
        {
            get { return mValues; }
        }

        #endregion Properties

        #region Indexers

        public TKey this[TValue val]
        {
            get {
                int index = mValues.IndexOf(val);
                return mKeys[index];
            }
            set {
                int index = mValues.IndexOf(val);
                mKeys[index] = value;
            }
        }

        public TValue this[TKey key]
        {
            get {
                int index = mKeys.IndexOf(key);
                return mValues[index];
            }
            set {
                int index = mKeys.IndexOf(key);
                mValues[index] = value;
            }
        }

        #endregion Indexers

        #region Methods

        public void Add(TKey key, TValue value)
        {
            if (key != null) {
                if (mKeys.Contains(key) == false) {
                    mKeys.Add(key);
                    mValues.Add(value);
                } else {
                    throw (new ArgumentException("A key with the same value has already been added"));
                }
            } else {
                throw (new ArgumentNullException("key"));
            }
        }

        public void Clear()
        {
            mKeys.Clear();
            mValues.Clear();
        }

        public bool ContainsKey(TKey key)
        {
            return mKeys.Contains(key);
        }

        public bool ContainsValue(TValue value)
        {
            return mValues.Contains(value);
        }

        public TKey GetKey(TValue value)
        {
            int index = mValues.IndexOf(value);
            return mKeys[index];
        }

        public TValue GetValue(TKey key)
        {
            int index = mKeys.IndexOf(key);
            return mValues[index];
        }

        public int IndexOfKey(TKey key)
        {
            return mKeys.IndexOf(key);
        }

        public int IndexOfValue(TValue value)
        {
            return mValues.IndexOf(value);
        }

        public TKey KeyByIndex(int index)
        {
            return mKeys[index];
        }

        public void RemoveAt(int index)
        {
            mKeys.RemoveAt(index);
            mValues.RemoveAt(index);
        }

        public void RemoveAtKey(TKey key)
        {
            if (key != null) {
                if (mKeys.Contains(key)) {
                    int index = mKeys.IndexOf(key);
                    mKeys.RemoveAt(index);
                    mValues.RemoveAt(index);
                } else {
                    throw (new KeyNotFoundException());
                }
            } else {
                throw (new ArgumentNullException("key"));
            }
        }

        public void RemoveAtValue(TValue value)
        {
            if (value != null) {
                if (mValues.Contains(value)) {
                    int index = mValues.IndexOf(value);
                    mKeys.RemoveAt(index);
                    mValues.RemoveAt(index);
                } else {
                    throw (new KeyNotFoundException());
                }
            } else {
                throw (new ArgumentNullException("value"));
            }
        }

        public void SetKey(TValue value, TKey newKey)
        {
            int index = mValues.IndexOf(value);
            mKeys[index] = newKey;
        }

        public void SetValue(TKey key, TValue value)
        {
            int index = mKeys.IndexOf(key);
            mValues[index] = value;
        }

        public TValue ValueByIndex(int index)
        {
            return mValues[index];
        }

        #endregion Methods
    }
}