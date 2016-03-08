using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServiceRegistery
{
	public static class ExtensionFunc
	{
		public static Task GetResponseAsync(this HttpWebRequest obj, object state)
		{
			return Task.Factory.FromAsync(obj.BeginGetResponse, obj.EndGetResponse, state);
		}
	}
}
