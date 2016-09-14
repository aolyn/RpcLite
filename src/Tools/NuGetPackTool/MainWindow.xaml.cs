using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml;

namespace NuGetPackTool
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
			txtNuspec.AllowDrop = true;
			txtNuspec.DragEnter += TxtNuspec_DragEnter;
			txtNuspec.Drop += TxtNuspec_Drop;
			txtNuspec.PreviewDragOver += TxtNuspec_PreviewDragOver;

			if (File.Exists(txtNuspec.Text))
			{
				ReadNuspec();
			}
		}

		private void TxtNuspec_PreviewDragOver(object sender, DragEventArgs e)
		{
			e.Handled = true;
		}

		private void TxtNuspec_DragEnter(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				e.Effects = DragDropEffects.Copy;
			}
			else
			{
				e.Effects = DragDropEffects.None;
			}
		}

		private void TxtNuspec_Drop(object sender, DragEventArgs e)
		{
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				// Note that you can have more than one file.
				string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

				// Assuming you have one file that you care about, pass it off to whatever
				// handling code you have defined.
				//HandleFileOpen(files[0]);
			}
		}

		private void btnSetVersion_Click(object sender, RoutedEventArgs e)
		{
			var file = txtNuspec.Text;

			var xml = new XmlDocument();
			xml.Load(file);

			var xmlnsm = new XmlNamespaceManager(xml.NameTable);
			xmlnsm.AddNamespace("def", xml.DocumentElement?.NamespaceURI);
			//xmlnsm.DefaultNamespace = xml.DocumentElement?.Name;

			var versionNode = xml.DocumentElement?.SelectSingleNode("/def:package/def:metadata/def:version", xmlnsm);

			if (versionNode != null)
			{
				versionNode.InnerText = txtVersion.Text.Trim();
			}

			xml.Save(file);
		}

		private void btnRead_Click(object sender, RoutedEventArgs e)
		{
			ReadNuspec();
		}

		private void ReadNuspec()
		{
			var file = txtNuspec.Text;
			var dir = System.IO.Path.GetDirectoryName(file);
			Environment.CurrentDirectory = dir;

			var xml = new XmlDocument();
			xml.Load(file);

			var xmlnsm = new XmlNamespaceManager(xml.NameTable);
			xmlnsm.AddNamespace("def", xml.DocumentElement?.NamespaceURI);
			//xmlnsm.DefaultNamespace = xml.DocumentElement?.Name;

			var versionNode = xml.DocumentElement?.SelectSingleNode("/def:package/def:metadata/def:version", xmlnsm);

			if (versionNode != null)
			{
				txtVersion.Text = versionNode.InnerText;
			}
		}

		private void btnPack_Click(object sender, RoutedEventArgs e)
		{
			Process.Start("pack-package.cmd");
		}

		private void btnPublish_Click(object sender, RoutedEventArgs e)
		{
			var args = $"nuget push RpcLite.{txtVersion.Text}.nupkg -Source https://www.nuget.org";
			args += Environment.NewLine + "pause";

			RunCommand(args);

			//var result = RunCommand("nuget.exe", args);
			//if (!string.IsNullOrWhiteSpace(result))
			//	MessageBox.Show(result);
		}

		private static void RunCommand(string args)
		{
			var shellFile = Guid.NewGuid().ToString() + ".cmd";
			File.WriteAllText(shellFile, args);
			var proc = Process.Start(shellFile);
			proc.WaitForExit();
			File.Delete(shellFile);
		}

		private static string RunCommand(string file, string cmd)
		{
			var startInfo = new ProcessStartInfo
			{
				//FileName = "cmd.exe",
				//Arguments = "/C " + cmd,
				FileName = file,
				Arguments = cmd,
				UseShellExecute = false,
				RedirectStandardInput = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			};

			var proc = new Process
			{
				StartInfo = startInfo,
			};

			proc.Start();
			proc.WaitForExit();
			var output = proc.StandardOutput.ReadToEnd();//读取进程的输出  
			var output2 = proc.StandardError.ReadToEnd();//读取进程的输出  
			return output + output2;
		}

		public string Execute(string dosCommand, int milliseconds)
		{
			string output = "";     //输出字符串  
			if (!string.IsNullOrEmpty(dosCommand))
			{
				Process process = new Process();     //创建进程对象  
				ProcessStartInfo startInfo = new ProcessStartInfo
				{
					FileName = "cmd.exe",
					Arguments = "/C " + dosCommand,
					UseShellExecute = false,
					RedirectStandardInput = false,
					RedirectStandardOutput = true,
					CreateNoWindow = true
				};
				//设定需要执行的命令  
				//设定参数，其中的“/C”表示执行完命令后马上退出  
				//不使用系统外壳程序启动  
				//不重定向输入  
				//重定向输出  
				//不创建窗口  
				process.StartInfo = startInfo;
				try
				{
					if (process.Start())       //开始进程  
					{
						if (milliseconds == 0)
							process.WaitForExit();     //这里无限等待进程结束  
						else
							process.WaitForExit(milliseconds);  //当超过这个毫秒后，不管执行结果如何，都不再执行这个DOS命令  
						output = process.StandardOutput.ReadToEnd();//读取进程的输出  
					}
				}
				catch
				{
				}
				finally
				{
					if (process != null)
						process.Close();
				}
			}
			return output;
		}

		private void btnCopyToLocalIIS_Click(object sender, RoutedEventArgs e)
		{
			var args = $@"copy RpcLite.{txtVersion.Text}.nupkg E:\Users\Chris\Desktop\Topic\C#\NuGet\NuGet.Server\src\NuGet.Server\Packages\";
			args += Environment.NewLine + "pause";
			RunCommand(args);
		}
	}
}