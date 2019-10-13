using System;
using System.Collections;
using System.Collections.Generic;

namespace RpcLite
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class ReadOnlyListWraper<T> : IReadOnlyCollection<T>
	{
		private readonly IList<T> _list;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		public ReadOnlyListWraper(IList<T> list)
		{
			if (list == null)
				throw new ArgumentNullException(nameof(list));

			_list = list;
		}

		/// <summary>
		/// 
		/// </summary>
		public int Count => _list.Count;

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
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return _list.GetEnumerator();
		}
	}
}

//#if NET40

//namespace System.Collections.Generic
//{
//	/// <summary>
//	/// 
//	/// </summary>
//	/// <typeparam name="T"></typeparam>
//	public interface IReadOnlyCollection<out T> : IEnumerable<T>, IEnumerable
//	{
//		/// <summary>
//		/// 
//		/// </summary>
//		int Count { get; }
//	}
//}

//#endif
