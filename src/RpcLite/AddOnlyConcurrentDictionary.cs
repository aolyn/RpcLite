using System;
using System.Collections;
using System.Collections.Generic;

namespace RpcLite
{
	/// <summary>
	/// Quick read concurrent dictionary only GetOrAdd is thread safe
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	internal class AddOnlyConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private readonly Dictionary<TKey, TValue> _innerDictionary = new Dictionary<TKey, TValue>();
		private readonly object _allLocker = new object();

		/// <summary>
		/// [thread safe] Adds a key/value pair if the key does not already exist.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public TValue GetOrAdd(TKey key, TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			return GetOrAdd(key, k => value);
		}

		/// <summary>
		/// [thread safe] Adds a key/value pair if the key does not already exist.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="valueFactory"></param>
		/// <returns></returns>
		public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
		{
			if (key == null)
			{
				throw new ArgumentNullException(nameof(key));
			}

			TValue value;
			if (TryGetValue(key, out value))
			{
				return value;
			}

			lock (_allLocker)
			{
				if (TryGetValue(key, out value))
				{
					return value;
				}

				value = valueFactory(key);
				Add(key, value);
				return value;
			}
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			throw new NotImplementedException();
		}

		public int Count { get; }
		public bool IsReadOnly { get; }
		public void Add(TKey key, TValue value)
		{
			_innerDictionary.Add(key, value);
		}

		public bool ContainsKey(TKey key)
		{
			throw new NotImplementedException();
		}

		public bool Remove(TKey key)
		{
			throw new NotImplementedException();
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return _innerDictionary.TryGetValue(key, out value);
		}

		public TValue this[TKey key]
		{
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}

		public ICollection<TKey> Keys => _innerDictionary.Keys;

		public ICollection<TValue> Values => _innerDictionary.Values;
	}

}
