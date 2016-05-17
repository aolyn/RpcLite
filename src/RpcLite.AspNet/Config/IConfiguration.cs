using System.Collections.Generic;

namespace RpcLite.Config
{
	public interface IConfiguration
	{
		//
		// Summary:
		//     Gets the immediate descendant configuration sub-sections.
		//
		// Returns:
		//     The configuration sub-sections.
		IEnumerable<IConfigurationSection> GetChildren();

		//
		// Summary:
		//     Gets or sets a configuration value.
		//
		// Parameters:
		//   key:
		//     The configuration key.
		//
		// Returns:
		//     The configuration value.
		string this[string key] { get; set; }

		//
		// Summary:
		//     Gets a configuration sub-section with the specified key.
		//
		// Parameters:
		//   key:
		//     The key of the configuration section.
		//
		// Returns:
		//     The Microsoft.Extensions.Configuration.IConfigurationSection.
		//
		// Remarks:
		//     This method will never return null. If no matching sub-section is found with
		//     the specified key, an empty Microsoft.Extensions.Configuration.IConfigurationSection
		//     will be returned.
		IConfigurationSection GetSection(string key);
	}
}
