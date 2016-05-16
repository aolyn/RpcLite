using System;
using System.Collections.Generic;
using System.Threading;

namespace RpcLite
{
	/// <summary>
	/// Quick read concurrent dictionary only GetOrAdd is thread safe
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TValue"></typeparam>
	public class QuickReadConcurrentDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	{
		//private readonly Dictionary<TKey, TValue> _innerDictionary = new Dictionary<TKey, TValue>();
		private readonly object _allLocker = new object();
		private readonly Dictionary<TKey, object> _itemLocks = new Dictionary<TKey, object>();
		private readonly ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

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
				throw new ArgumentNullException(nameof(key));
			}

			TValue value;
			try
			{
				_readWriteLock.EnterReadLock();
				if (TryGetValue(key, out value))
				{
					return value;
				}
			}
			finally
			{
				_readWriteLock.ExitReadLock();
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
				try
				{
					_readWriteLock.EnterReadLock();
					if (TryGetValue(key, out value))
					{
						return value;
					}
				}
				finally
				{
					_readWriteLock.ExitReadLock();
				}

				value = valueFactory();
				try
				{
					_readWriteLock.EnterWriteLock();
					Add(key, value);
				}
				finally
				{
					_readWriteLock.ExitWriteLock();
				}

				lock (_allLocker)
				{
					_itemLocks.Remove(key);
				}
				return value;
			}
		}
	}
}
