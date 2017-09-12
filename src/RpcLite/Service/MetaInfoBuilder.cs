using System;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RpcLite.Service
{
	/// <summary>
	/// 
	/// </summary>
	internal class MetaInfoBuilder
	{
		public object GetMetaInfo(RpcService service)
		{
			var type = service.Type;
			var sb = new StringBuilder();
			sb.AppendFormat("<h1>{0}</h1>", type.Name);
			sb.AppendLine();
			sb.Append("<h3>Actions:</h3><p>");

			var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

			var methodString = GetActionString(methods);
			sb.Append(methodString);
			sb.Append("</p>");

			return sb.ToString();
		}

		private static StringBuilder GetActionString(MethodInfo[] methods)
		{
			var sb = new StringBuilder();
			foreach (var method in methods)
			{
				if (method.DeclaringType == typeof(object))
					continue;

				sb.Append("<p>");
				sb.AppendLine();
				sb.Append(GetTypeName(method.ReturnType));
				sb.Append(" ");
				sb.AppendFormat("{0}(", method.Name);

				var isFirstArgument = true;
				foreach (var arg in method.GetParameters())
				{
					if (isFirstArgument)
						isFirstArgument = false;
					else
						sb.Append(", ");

					sb.Append(arg.ParameterType.Name);
					sb.Append(" ");
					sb.Append(arg.Name);
				}

				sb.AppendFormat(");");
			}
			sb.Append("</p>");
			return sb;
		}

		private static string GetTypeName(Type type)
		{
			if (type.IsGenericType && type.BaseType == typeof(Task))
			{
				var gpts = type.GetGenericArguments();
				if (gpts.Length > 0)
				{
					return $"Task&lt;{gpts[0].Name}&gt;";
				}
			}

			return type.Name;
		}
	}
}