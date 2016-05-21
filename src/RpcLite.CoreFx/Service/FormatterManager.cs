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
	public static class FormatterManager
	{
		/// <summary>
		/// 
		/// </summary>
		private static List<IFormatter> _formaters;
		private static readonly object FormatterLock = new object();
		private static Dictionary<string, IFormatter> _typeToFormatterDictionary = new Dictionary<string, IFormatter>();

		/// <summary>
		/// get formatter by content type
		/// </summary>
		/// <param name="contentType"></param>
		/// <returns></returns>
		public static IFormatter GetFormatter(string contentType)
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


		private static void AddFormatter(IFormatter formatter)
		{
			if (formatter == null)
				throw new ArgumentNullException(nameof(formatter));

			lock (FormatterLock)
			{
				var formatters = _formaters == null ? new List<IFormatter>() : _formaters.ToList();
				formatters.Add(formatter);

				var dic = new Dictionary<string, IFormatter>();
				foreach (var item in formatters)
				{
					foreach (var mime in item.SupportMimes)
					{
						dic.Add(mime, item);
					}
				}

				_typeToFormatterDictionary = dic;
				_formaters = formatters;
			}
		}

		static FormatterManager()
		{
#if NETCORE
			AddFormatter(new JsonFormatter());
#else
			//AddFormatter(new NetJsonFormater());
			AddFormatter(new JsonFormatter());
#endif
			//AddFormatter(new XmlFormatter());
		}
	}
}
