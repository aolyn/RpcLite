using System;
using System.Collections.Generic;
using System.Linq;
using RpcLite.Config;
using RpcLite.Formatters;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	public class FormatterManager
	{
		private List<IFormatter> _formatters;
		private Dictionary<string, IFormatter> _typeToFormatterDictionary = new Dictionary<string, IFormatter>();

		/// <summary>
		/// 
		/// </summary>
		public static FormatterManager Default = new FormatterManager(null);

		/// <summary>
		/// get formatter by content type
		/// </summary>
		/// <param name="contentType"></param>
		/// <returns></returns>
		public IFormatter GetFormatter(string contentType)
		{
			if (_formatters == null || _formatters.Count == 0)
			{
				throw new ConfigException("Configuration error: no formatters.");
			}

			if (string.IsNullOrWhiteSpace(contentType))
				return _formatters.First();

			_typeToFormatterDictionary.TryGetValue(contentType, out var formatter);
			return formatter;
		}

		/// <summary>
		/// get formatter by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public IFormatter GetFormatterByName(string name)
		{
			if (_formatters == null || _formatters.Count == 0)
			{
				throw new ConfigException("Configuration error: no formatters.");
			}

			if (string.IsNullOrWhiteSpace(name))
				return _formatters.First();

			var formatter = _formatters
				.FirstOrDefault(it => it.Name == name);
			return formatter;
		}

		/// <summary>
		/// 
		/// </summary>
		public IFormatter DefaultFormatter { get; private set; }

		private void AddFormatter(IFormatter formatter)
		{
			if (formatter == null)
				throw new ArgumentNullException(nameof(formatter));

			//lock (_formatterLock)
			//{
			var formatters = _formatters == null
				? new List<IFormatter>().ToList()
				: _formatters.ToList();
			formatters.Add(formatter);

			ReLinkFormatters(formatters);
			//}
		}

		private void ReLinkFormatters(List<IFormatter> formatters)
		{
			var dic = new Dictionary<string, IFormatter>();
			foreach (var item in formatters)
			{
				foreach (var mime in item.SupportMimes)
				{
					dic[mime] = item;
				}
			}

			_typeToFormatterDictionary = dic;
			DefaultFormatter = formatters.FirstOrDefault();
			_formatters = formatters;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		public FormatterManager(FormatterConfig config)
		{
			if (config?.RemoveDefault != true)
			{
				//#if NETCORE
				//	AddFormatter(new JsonFormatter());
				//#else
				//AddFormatter(new NetJsonFormatter());
				AddFormatter(new JsonFormatter());
				AddFormatter(new XmlFormatter());
				//#endif
			}

			if (config?.Formatters != null)
			{
				foreach (var item in config.Formatters)
				{
					var formatter = ReflectHelper.CreateInstanceByIdentifier<IFormatter>(item.Type);
					AddFormatter(formatter);
				}
			}
		}
	}
}
