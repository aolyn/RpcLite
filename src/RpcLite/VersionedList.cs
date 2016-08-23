using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace RpcLite
{
	public class VersionedList<T> : IList<T>
	{
		private readonly List<T> _list = new List<T>();
		private long _version;
		public long Version { get { return Interlocked.Read(ref _version); } }

		public T this[int index]
		{
			get { return _list[index]; }
			set
			{
				AddVersion();
				_list[index] = value;
			}
		}

		public int Count { get { return _list.Count; } }

		public bool IsReadOnly { get { return true; } }

		public void Add(T item)
		{
			AddVersion();
			_list.Add(item);
		}

		private void AddVersion()
		{
			Interlocked.Increment(ref _version);
		}

		public void Clear()
		{
			AddVersion();
			_list.Clear();
		}

		public bool Contains(T item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		public int IndexOf(T item)
		{
			return _list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			AddVersion();
			_list.Insert(index, item);
		}

		public bool Remove(T item)
		{
			AddVersion();
			return _list.Remove(item);
		}

		public void RemoveAt(int index)
		{
			AddVersion();
			_list.RemoveAt(index);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _list.GetEnumerator();
		}
	}
}
