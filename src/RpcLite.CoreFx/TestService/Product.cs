namespace RpcLite.TestService
{
	/// <summary>
	/// 
	/// </summary>
	public class Product
	{
		/// <summary>
		/// 
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public decimal Price { get; set; }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0}, {1}, {2}", Id, Name, Price);
		}

	}
}