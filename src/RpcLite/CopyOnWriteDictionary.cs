using System;
using System.Collections;
using System.Collections.Generic;

namespace RpcLite
{
	/// <inheritdoc />
	/// <summary>
	/// a dictionary read lock free, write will create new dictionary. suit for most ready and very less write
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class CopyOnWriteDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private Dictionary<TKey, TValue> _inner = new Dictionary<TKey, TValue>();
		private readonly object _writeLock = new object();

		/// <inheritdoc />
		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return _inner.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <inheritdoc />
		public void Add(KeyValuePair<TKey, TValue> item)
		{
			lock (_writeLock)
			{
				var dic = new Dictionary<TKey, TValue>(_inner);
				((IDictionary<TKey, TValue>)dic).Add(item);
				_inner = dic;
			}
		}

		/// <inheritdoc />
		public void Clear()
		{
			lock (_writeLock)
			{
				_inner = new Dictionary<TKey, TValue>();
			}

		}

		/// <inheritdoc />
		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return ((IDictionary<TKey, TValue>)_inner).Contains(item);
		}

		/// <inheritdoc />
		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			((IDictionary<TKey, TValue>)_inner).CopyTo(array, arrayIndex);
		}

		/// <inheritdoc />
		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			lock (_writeLock)
			{
				var dic = new Dictionary<TKey, TValue>(_inner);
				var result = ((IDictionary<TKey, TValue>)dic).Remove(item);
				_inner = dic;
				return result;
			}

		}

		/// <inheritdoc />
		public int Count => _inner.Count;

		/// <inheritdoc />
		public bool IsReadOnly => false;

		/// <inheritdoc />
		public void Add(TKey key, TValue value)
		{
			lock (_writeLock)
			{
				var dic = new Dictionary<TKey, TValue>(_inner) { { key, value } };
				_inner = dic;
			}

		}

		/// <inheritdoc />
		public bool ContainsKey(TKey key)
		{
			return _inner.ContainsKey(key);
		}

		/// <inheritdoc />
		public bool Remove(TKey key)
		{
			lock (_writeLock)
			{
				var dic = new Dictionary<TKey, TValue>(_inner);
				var result = dic.Remove(key);
				_inner = dic;
				return result;
			}
		}

		/// <inheritdoc />
		public bool TryGetValue(TKey key, out TValue value)
		{
			lock (_writeLock)
			{
				return _inner.TryGetValue(key, out value);
			}
		}

		/// <inheritdoc />
		public TValue this[TKey key]
		{
			get => _inner[key];
			set
			{
				lock (_writeLock)
				{
					if (_inner.ContainsKey(key))
					{
						_inner[key] = value;
						return;
					}

					var dic = new Dictionary<TKey, TValue>(_inner)
					{
						[key] = value
					};
					_inner = dic;
				}
			}
		}

		/// <inheritdoc />
		public ICollection<TKey> Keys => _inner.Keys;

		/// <inheritdoc />
		public ICollection<TValue> Values => _inner.Values;

		/// <summary>
		/// get value if exist or else add value create by valueFactory
		/// </summary>
		/// <param name="key"></param>
		/// <param name="valueFactory"></param>
		/// <returns></returns>
		public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueFactory)
		{
			if (_inner.TryGetValue(key, out var value))
			{
				return value;
			}

			lock (_writeLock)
			{
				if (_inner.TryGetValue(key, out value))
				{
					return value;
				}

				value = valueFactory(key);

				var dic = new Dictionary<TKey, TValue>(_inner) { { key, value } };
				_inner = dic;

				return value;
			}
		}

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
		[Obsolete("use GetOrAdd(TKey key, Func<TKey, TValue> valueFactory) instead")]
		public TValue GetOrAdd(TKey key, Func<TValue> valueFactory)
		{
			return GetOrAdd(key, k => valueFactory());
		}
	}
}
