﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RpcLite.Config
{
	/// <summary>
	/// Represents a section of application configuration values.
	/// </summary>
	public interface IConfigurationSection : IConfiguration
	{
		/// <summary>
		///  Gets the key this section occupies in its parent.
		/// </summary>
		string Key { get; }

		/// <summary>
		///  Gets the full path to this section within the Microsoft.Extensions.Configuration.IConfiguration.
		/// </summary>
		string Path { get; }

		/// <summary>
		/// Gets or sets the section value.
		/// </summary>
		string Value { get; set; }
	}
}
