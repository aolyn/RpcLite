using System;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TContract">contract interface</typeparam>
	public class RpcClientBuilder<TContract>
		where TContract : class
	{
		private static readonly Lazy<Func<RpcClientBase<TContract>>> ClientCreateFunc = new Lazy<Func<RpcClientBase<TContract>>>(() =>
		{
			var type = ClientWrapper.WrapInterface<TContract>();
			var func = TypeCreator.GetCreateInstanceFunc(type) as Func<RpcClientBase<TContract>>;
			return func;
		}, true);

		///// <summary>
		///// 
		///// </summary>
		///// <returns></returns>
		//public RpcClientBase<TContract> GetInstance()
		//{
		//	return GetInstance(null);
		//}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="baseUrl"></param>
		/// <returns></returns>
		public RpcClientBase<TContract> GetInstance(string baseUrl)
		{
			if (ClientCreateFunc.Value == null)
				throw new ClientException("GetCreateInstanceFunc Error.");

			var client = ClientCreateFunc.Value();
			return client;
		}

	}
}
