using System;

namespace RpcLite.Client
{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TContract">contract interface</typeparam>
	internal class RpcClientBuilder<TContract>
		where TContract : class
	{
		private readonly Lazy<Func<RpcClientBase>> _clientCreateFunc =
			new Lazy<Func<RpcClientBase>>(() =>
		   {
			   var type = ClientWrapper.WrapInterface<TContract>();
			   var func = TypeCreator.GetCreateInstanceFunc(type) as Func<RpcClientBase>;
			   return func;
		   }, true);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public RpcClientBase GetInstance(string address)
		{
			if (_clientCreateFunc.Value == null)
				throw new ClientException("GetCreateInstanceFunc Error.");

			var client = _clientCreateFunc.Value();
			client.Address = address;
			return client;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	internal class RpcClientBuilder
	{
		private readonly Func<object> _clientCreateFunc;

		public RpcClientBuilder(Type contractType)
		{
			var type = ClientWrapper.WrapInterface(contractType);
			_clientCreateFunc = TypeCreator.GetCreateInstanceFunc(type);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="address"></param>
		/// <returns></returns>
		public RpcClientBase GetInstance(string address)
		{
			var client = (RpcClientBase)_clientCreateFunc();
			client.Address = address;
			return client;
		}
	}
}