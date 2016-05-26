using System.Collections.Generic;

namespace RpcLite.Config
{
	/// <summary>
	/// 
	/// </summary>
	public interface IConfiguration
	{
		/// <summary>
		/// Gets the immediate descendant configuration sub-sections.
		/// </summary>
		/// <returns>The configuration sub-sections.</returns>
		IEnumerable<IConfigurationSection> GetChildren();

		/// <summary>
		/// 
		/// </summary>
		IEnumerable<IConfigurationSection> Children { get; }

		/// <summary>
		/// Gets or sets a configuration value.
		/// </summary>
		/// <param name="key">The configuration key.</param>
		/// <returns>The configuration value.</returns>
		string this[string key] { get; set; }

		/// <summary>
		/// Gets a configuration sub-section with the specified key.
		/// </summary>
		/// <param name="key">The key of the configuration section.</param>
		/// <returns>The Microsoft.Extensions.Configuration.IConfigurationSection.</returns>
		/// <remarks>
		///     This method will never return null. If no matching sub-section is found with
		///     the specified key, an empty Microsoft.Extensions.Configuration.IConfigurationSection
		///     will be returned.
		/// </remarks>
		IConfigurationSection GetSection(string key);
	}
}
