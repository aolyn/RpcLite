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
		private List<IFormatter> _formaters;
		private readonly object _formatterLock = new object();
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
			if (_formaters == null || _formaters.Count == 0)
			{
				throw new ConfigException("Configuration error: no formatters.");
			}

			if (string.IsNullOrWhiteSpace(contentType))
				return _formaters.First();

			IFormatter formatter;
			_typeToFormatterDictionary.TryGetValue(contentType, out formatter);

			//var formatter = _formaters.FirstOrDefault(it => it.SupportMimes.Contains(contentType));
			return formatter;
		}

		/// <summary>
		/// get formatter by name
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public IFormatter GetFormatterByName(string name)
		{
			if (_formaters == null || _formaters.Count == 0)
			{
				throw new ConfigException("Configuration error: no formatters.");
			}

			if (string.IsNullOrWhiteSpace(name))
				return _formaters.First();

			var formatter = _formaters
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

			lock (_formatterLock)
			{
				var formatters = _formaters == null
					? new List<IFormatter>().ToList()
					: _formaters.ToList();
				formatters.Add(formatter);

				ReLinkFormatters(formatters);
			}
		}

		private void RemoveFormatter(IFormatter formatter)
		{
			if (formatter == null)
				throw new ArgumentNullException(nameof(formatter));

			lock (_formatterLock)
			{
				var formatters = _formaters == null
					? new List<IFormatter>().ToList()
					: _formaters.ToList();
				formatters.Remove(formatter);

				ReLinkFormatters(formatters);
			}
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
			_formaters = formatters;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="config"></param>
		public FormatterManager(RpcConfig config)
		{
			if (config?.Formatter?.RemoveDefault != true)
			{
				//#if NETCORE
				//				AddFormatter(new JsonFormatter());
				//#else
				//AddFormatter(new NetJsonFormater());
				AddFormatter(new JsonFormatter());
				AddFormatter(new XmlFormatter());
				//#endif
			}

			if (config?.Formatter?.Formatters != null)
			{
				foreach (var item in config.Formatter?.Formatters)
				{
					var formatter = TypeCreator.CreateInstanceByIdentifier<IFormatter>(item.Type);
					AddFormatter(formatter);
				}
			}
		}
	}
}
