using System;
using System.Collections.Generic;

namespace RpcLite
{
	/// <summary>
	/// Quick read concurrent dictionary only GetOrAdd is thread safe
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class QuickReadConcurrentDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	{
		private readonly Dictionary<TKey, TValue> _innerDictionary = new Dictionary<TKey, TValue>();
		private readonly object _allLocker = new object();
		private readonly Dictionary<TKey, object> _itemLocks = new Dictionary<TKey, object>();

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
				throw new ArgumentNullException("key");
			}

			return GetOrAdd(key, () => value);
		}

		/// <summary>
		/// [thread safe] Adds a key/value pair if the key does not already exist.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="valueFactory"></param>
		/// <returns></returns>
		public TValue GetOrAdd(TKey key, Func<TValue> valueFactory)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}

			TValue value;
			if (_innerDictionary.TryGetValue(key, out value))
			{
				return value;
			}

			object locker;
			lock (_allLocker)
			{
				if (!_itemLocks.TryGetValue(key, out locker))
				{
					locker = new object();
					_itemLocks.Add(key, locker);
				}
			}

			lock (locker)
			{
				if (_innerDictionary.TryGetValue(key, out value))
				{
					_itemLocks.Remove(key);
					return value;
				}

				value = valueFactory();
				_innerDictionary.Add(key, value);
				_itemLocks.Remove(key);
				return value;
			}
		}
	}
}
