namespace RpcLite.Config
{
	/// <summary>
	/// ServiceLifeCycle
	/// </summary>
	public enum ServiceLifeCycle
	{
		/// <summary>
		/// Singleton
		/// </summary>
		Singleton = 0,

		/// <summary>
		/// Scoped
		/// </summary>
		Scoped = 1,

		/// <summary>
		/// Transit
		/// </summary>
		Transient = 2,
	}
}
