namespace RpcLite.Registry
{
	/// <summary>
	/// 
	/// </summary>
	public struct ServiceIdentifier
	{
		/// <summary>
		/// 
		/// </summary>
		public string Name;

		/// <summary>
		/// 
		/// </summary>
		public string Group;

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			unchecked
			{
				return ((Name?.GetHashCode() ?? 0) * 397) ^ (Group?.GetHashCode() ?? 0);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public static ServiceIdentifier Empty = new ServiceIdentifier();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="group"></param>
		public ServiceIdentifier(string name, string group)
		{
			Name = name;
			Group = group;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="b"></param>
		/// <returns></returns>
		public bool Equals(ServiceIdentifier b)
		{
			return Name == b.Name && Group == b.Group;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (!(obj is ServiceIdentifier)) return false;

			var v = (ServiceIdentifier)obj;
			return Equals(v);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator ==(ServiceIdentifier a, ServiceIdentifier b)
		{
			return a.Name == b.Name && a.Group == b.Group;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static bool operator !=(ServiceIdentifier a, ServiceIdentifier b)
		{
			return !(a == b);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"{Name}, {Group}";
		}
	}
}
