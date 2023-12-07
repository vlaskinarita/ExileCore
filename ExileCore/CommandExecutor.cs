using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExileCore.Shared;
using JM.LinqFaster;

namespace ExileCore;

public static class CommandExecutor
{
	public const string Offset = "offset";

	public const string OffsetS = "offsets";

	public const string LoaderOffsets = "loader_offsets";

	public const string CompilePlugins = "compile_plugins";

	public const string GameOffsets = "GameOffsets.dll";

	public static void Execute(string cmd)
	{
		switch (cmd)
		{
		case "offset":
		case "offsets":
			CreateOffsets(force: true);
			return;
		case "compile_plugins":
			CompilePluginsIntoDll();
			return;
		case "loader_offsets":
			CreateOffsets();
			return;
		}
		if (cmd.StartsWith("compile_"))
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine("Plugins", "Source"));
			string plugin = cmd.Replace("compile_", "");
			if (directoryInfo.GetDirectories().FirstOrDefaultF((DirectoryInfo x) => x.Name.Equals(plugin, StringComparison.OrdinalIgnoreCase)) != null)
			{
				CompilePluginIntoDll(plugin);
			}
		}
	}

	private static void CompilePluginIntoDll(string plugin)
	{
		DirectoryInfo directoryInfo = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", "Source")).GetDirectories().FirstOrDefaultF((DirectoryInfo x) => x.Name.Equals(plugin, StringComparison.OrdinalIgnoreCase));
		if (directoryInfo == null)
		{
			DebugWindow.LogError(plugin + " directory not found.");
			return;
		}
		using PluginCompiler compiler = PluginCompiler.CreateOrThrow();
		CompileSourceIntoDll(compiler, directoryInfo);
	}

	private static void CompileSourceIntoDll(PluginCompiler compiler, DirectoryInfo info)
	{
		Stopwatch stopwatch = Stopwatch.StartNew();
		FileInfo fileInfo = info.GetFiles("*.csproj", SearchOption.AllDirectories).FirstOrDefault();
		if (fileInfo == null)
		{
			MessageBox.Show(".csproj for plugin " + info.Name + " not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		string text = info.FullName.Replace("\\Source\\", "\\Compiled\\");
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		try
		{
			compiler.CompilePlugin(fileInfo, text);
			MessageBox.Show($"{info.Name}  >>> Successful <<< (Working time: {stopwatch.ElapsedMilliseconds} ms.)");
		}
		catch (Exception ex)
		{
			MessageBox.Show(info.Name + " -> Failed, look in " + info.FullName + "/Errors.txt", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			File.WriteAllText(Path.Combine(info.FullName, "Errors.txt"), ex.Message);
		}
	}

	private static void CreateOffsets(bool force = false)
	{
		FileInfo fileInfo = new FileInfo("GameOffsets.dll");
		DirectoryInfo directoryInfo = new DirectoryInfo(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "GameOffsets"));
		if (!fileInfo.Exists && !directoryInfo.Exists)
		{
			MessageBox.Show("Offsets dll and folder not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			Environment.Exit(0);
			return;
		}
		if (!directoryInfo.Exists)
		{
			if (force)
			{
				MessageBox.Show("Offsets folder not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			return;
		}
		string[] array = (from x in directoryInfo.GetFiles("*.cs", SearchOption.AllDirectories)
			select x.FullName).ToArray();
		bool flag = force;
		if (!flag)
		{
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				if (new FileInfo(array2[i]).LastWriteTimeUtc > fileInfo.LastWriteTimeUtc)
				{
					flag = true;
					break;
				}
			}
		}
		if (!flag)
		{
			return;
		}
		FileInfo csProj = new FileInfo(Path.Join(directoryInfo.FullName, "GameOffsets.csproj"));
		using PluginCompiler pluginCompiler = PluginCompiler.CreateOrThrow();
		try
		{
			pluginCompiler.CompilePlugin(csProj, fileInfo.Directory.FullName);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			Environment.Exit(1);
		}
		Assembly.Load(File.ReadAllBytes("GameOffsets.dll"));
	}

	private static void CompilePluginsIntoDll()
	{
		List<DirectoryInfo> list = (from x in new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", "Source")).GetDirectories()
			where (x.Attributes & FileAttributes.Hidden) == 0
			select x).ToList();
		if (list.Count == 0)
		{
			MessageBox.Show("Plugins/Source/ is empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		PluginCompiler compiler = PluginCompiler.CreateOrThrow();
		try
		{
			Parallel.ForEach(list, delegate(DirectoryInfo info)
			{
				CompileSourceIntoDll(compiler, info);
			});
		}
		finally
		{
			if (compiler != null)
			{
				((IDisposable)compiler).Dispose();
			}
		}
	}
}
