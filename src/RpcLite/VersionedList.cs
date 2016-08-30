using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace RpcLite
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class VersionedList<T> : IList<T>
	{
		private readonly List<T> _list = new List<T>();
		private long _version;

		/// <summary>
		/// 
		/// </summary>
		public long Version { get { return Interlocked.Read(ref _version); } }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public T this[int index]
		{
			get { return _list[index]; }
			set
			{
				AddVersion();
				_list[index] = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public int Count { get { return _list.Count; } }

		/// <summary>
		/// 
		/// </summary>
		public bool IsReadOnly { get { return false; } }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		public void Add(T item)
		{
			AddVersion();
			_list.Add(item);
		}

		/// <summary>
		/// 
		/// </summary>
		private void AddVersion()
		{
			Interlocked.Increment(ref _version);
		}

		/// <summary>
		/// 
		/// </summary>
		public void Clear()
		{
			AddVersion();
			_list.Clear();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(T item)
		{
			return _list.Contains(item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(T[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public int IndexOf(T item)
		{
			return _list.IndexOf(item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
		/// <param name="item"></param>
		public void Insert(int index, T item)
		{
			AddVersion();
			_list.Insert(index, item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Remove(T item)
		{
			AddVersion();
			return _list.Remove(item);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="index"></param>
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
