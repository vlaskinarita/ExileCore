using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using ExileCore.RenderQ;
using ExileCore.Shared;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ImGuiNET;
using JM.LinqFaster;
using SharpDX;

namespace ExileCore;

public class MenuWindow : IDisposable
{
	private enum Windows
	{
		MainDebugs,
		NotMainDebugs,
		Plugins,
		Coroutines,
		Caches
	}

	private record MainDebugTableRecord(DebugInformation Current, DebugInformation TotalDebug, int GroupCount)
	{
		public readonly string Name = Current.Name;

		public readonly float Percent = Current.Sum / TotalDebug.Sum * 100f;

		public readonly double Tick = Current.Tick;

		public readonly float Total = Current.Total;

		public readonly float TotalPercent = Current.Total / TotalDebug.Total * 100f;

		public readonly float AllPluginAverage = TotalDebug.Average / (float)GroupCount;

		[CompilerGenerated]
		protected virtual bool PrintMembers(StringBuilder builder)
		{
			RuntimeHelpers.EnsureSufficientExecutionStack();
			builder.Append("Current = ");
			builder.Append(Current);
			builder.Append(", TotalDebug = ");
			builder.Append(TotalDebug);
			builder.Append(", GroupCount = ");
			builder.Append(GroupCount.ToString());
			builder.Append(", Name = ");
			builder.Append((object?)Name);
			builder.Append(", Percent = ");
			float percent = Percent;
			builder.Append(percent.ToString());
			builder.Append(", Tick = ");
			double tick = Tick;
			builder.Append(tick.ToString());
			builder.Append(", Total = ");
			percent = Total;
			builder.Append(percent.ToString());
			builder.Append(", TotalPercent = ");
			percent = TotalPercent;
			builder.Append(percent.ToString());
			builder.Append(", AllPluginAverage = ");
			percent = AllPluginAverage;
			builder.Append(percent.ToString());
			return true;
		}
	}

	private static readonly Stopwatch swStartedProgram = Stopwatch.StartNew();

	private readonly SettingsContainer _settingsContainer;

	private readonly Core core;

	private int _index = -1;

	private DebugInformation AllPlugins;

	private readonly Action CoreSettings = delegate
	{
	};

	private static readonly DebugInformation DebugInformation = new DebugInformation("DebugWindow", main: false);

	private bool demo_window;

	private bool firstTime = true;

	private List<DebugInformation> MainDebugs = new List<DebugInformation>();

	private Action MoreInformation;

	private List<DebugInformation> NotMainDebugs = new List<DebugInformation>();

	private readonly Action OnWindowChange;

	private Windows openWindow;

	private readonly int PluginNameWidth = 200;

	private List<DebugInformation> PluginsDebug = new List<DebugInformation>();

	private Action Selected = delegate
	{
	};

	private string selectedName = "";

	private readonly Stopwatch sw = Stopwatch.StartNew();

	private readonly ThemeEditor themeEditor;

	private readonly Windows[] WindowsName;

	private string _pluginNameFilter = "";

	public static bool IsOpened;

	public CoreSettings _CoreSettings { get; }

	public List<ISettingsHolder> CoreSettingsDrawers { get; }

	private Windows OpenWindow
	{
		get
		{
			return openWindow;
		}
		set
		{
			if (openWindow != value)
			{
				openWindow = value;
				OnWindowChange?.Invoke();
			}
		}
	}

	public MenuWindow(Core core, SettingsContainer settingsContainer)
	{
		MenuWindow menuWindow = this;
		this.core = core;
		_settingsContainer = settingsContainer;
		_CoreSettings = settingsContainer.CoreSettings;
		themeEditor = new ThemeEditor(_CoreSettings);
		CoreSettingsDrawers = new List<ISettingsHolder>();
		SettingsParser.Parse(_CoreSettings, CoreSettingsDrawers);
		Selected = CoreSettings;
		CoreSettings = delegate
		{
			foreach (ISettingsHolder coreSettingsDrawer in menuWindow.CoreSettingsDrawers)
			{
				coreSettingsDrawer.Draw();
			}
		};
		_index = -1;
		Selected = CoreSettings;
		Core.DebugInformations.CollectionChanged += OnDebugInformationsOnCollectionChanged;
		OpenWindow = Windows.MainDebugs;
		WindowsName = Enum.GetValues<Windows>();
		OnWindowChange = (Action)Delegate.Combine(OnWindowChange, (Action)delegate
		{
			menuWindow.MoreInformation = null;
			menuWindow.selectedName = "";
		});
		Input.RegisterKey(_CoreSettings.MainMenuKeyToggle);
		HotkeyNode mainMenuKeyToggle = _CoreSettings.MainMenuKeyToggle;
		mainMenuKeyToggle.OnValueChanged = (Action)Delegate.Combine(mainMenuKeyToggle.OnValueChanged, (Action)delegate
		{
			Input.RegisterKey(menuWindow._CoreSettings.MainMenuKeyToggle);
		});
		_CoreSettings.Enable.OnValueChanged += delegate
		{
			if (!menuWindow._CoreSettings.Enable)
			{
				try
				{
					menuWindow._settingsContainer.SaveCoreSettings();
					PluginManager pluginManager = core.pluginManager;
					if (pluginManager != null)
					{
						foreach (PluginWrapper plugin in pluginManager.Plugins)
						{
							try
							{
								menuWindow._settingsContainer.SaveSettings(plugin.Plugin);
							}
							catch (Exception value)
							{
								DebugWindow.LogError($"SaveSettings for plugin error: {value}");
							}
						}
						return;
					}
				}
				catch (Exception value2)
				{
					DebugWindow.LogError($"SaveSettings error: {value2}");
				}
			}
		};
	}

	public void Dispose()
	{
		Core.DebugInformations.CollectionChanged -= OnDebugInformationsOnCollectionChanged;
		_settingsContainer.SaveCoreSettings();
	}

	private void OnDebugInformationsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
	{
		if (args.Action != 0)
		{
			return;
		}
		if (firstTime)
		{
			MainDebugs = Core.DebugInformations.Where((DebugInformation x) => x.Main && !x.Name.EndsWith("[P]") && !x.Name.EndsWith("[R]")).ToList();
			NotMainDebugs = Core.DebugInformations.Where((DebugInformation x) => !x.Main).ToList();
			PluginsDebug = Core.DebugInformations.Where((DebugInformation x) => x.Name.EndsWith("[P]") || x.Name.EndsWith("[R]")).ToList();
			firstTime = false;
			return;
		}
		foreach (DebugInformation newItem in args.NewItems)
		{
			if (newItem.Main && !newItem.Name.EndsWith("[P]") && !newItem.Name.EndsWith("[R]"))
			{
				MainDebugs.Add(newItem);
			}
			else if (newItem.Name.EndsWith("[P]") || newItem.Name.EndsWith("[R]"))
			{
				PluginsDebug.Add(newItem);
			}
			else
			{
				NotMainDebugs.Add(newItem);
			}
		}
	}

	public void Render(GameController gameController)
	{
		List<PluginWrapper> plugins = core.pluginManager?.Plugins.OrderBy((PluginWrapper x) => x.Name).ToList() ?? new List<PluginWrapper>();
		if ((bool)_CoreSettings.ShowDemoWindow)
		{
			demo_window = true;
			ImGui.ShowDemoWindow(ref demo_window);
			_CoreSettings.ShowDemoWindow.Value = demo_window;
		}
		if (gameController != null)
		{
			gameController.Memory.BackendMode = (_CoreSettings.UseNewMemoryBackend ? MemoryBackendMode.CacheAndPreload : MemoryBackendMode.AlwaysRead);
		}
		if ((bool)_CoreSettings.ShowDebugWindow)
		{
			DebugInformation.TickAction(DebugWindowRender);
		}
		if (_CoreSettings.MainMenuKeyToggle.PressedOnce())
		{
			_CoreSettings.Enable.Value = !_CoreSettings.Enable;
			if (!_CoreSettings.Enable)
			{
				_settingsContainer.SaveCoreSettings();
				if (gameController != null)
				{
					PluginManager pluginManager = core.pluginManager;
					if (pluginManager != null)
					{
						foreach (PluginWrapper plugin in pluginManager.Plugins)
						{
							_settingsContainer.SaveSettings(plugin.Plugin);
						}
					}
				}
			}
		}
		IsOpened = _CoreSettings.Enable;
		if (!_CoreSettings.Enable)
		{
			return;
		}
		using (core.Graphics.UseCurrentFont())
		{
			ImGui.SetNextWindowSize(new System.Numerics.Vector2(800f, 600f), ImGuiCond.FirstUseEver);
			bool p_open = _CoreSettings.Enable.Value;
			ImGui.Begin("HUD S3ttings", ref p_open);
			_CoreSettings.Enable.Value = p_open;
			ImGui.BeginChild("Left menu window", new System.Numerics.Vector2(PluginNameWidth, ImGui.GetContentRegionAvail().Y), ImGuiChildFlags.Border, ImGuiWindowFlags.None);
			int num = 0;
			if (ImGui.Selectable("Core", _index == --num))
			{
				_index = num;
				Selected = CoreSettings;
			}
			ImGui.Separator();
			if (ImGui.Selectable("ThemeEditor", _index == --num))
			{
				_index = num;
				Selected = themeEditor.DrawSettingsMenu;
			}
			ImGui.Separator();
			if (!PluginCompiler.IsEnabled)
			{
				ImGui.PushStyleColor(ImGuiCol.Text, Color.Red.ToImgui());
				bool num2 = ImGui.Selectable("Plugin compilation", _index == --num);
				ImGui.PopStyleColor();
				if (num2)
				{
					_index = num;
					Selected = DrawCompilationIsDisabledTab;
				}
			}
			if (gameController != null)
			{
				DrawPluginList(plugins, ref num);
			}
			ImGui.EndChild();
			ImGui.SameLine();
			ImGui.BeginChild("Options", ImGui.GetContentRegionAvail(), ImGuiChildFlags.Border);
			Selected?.Invoke();
			ImGui.EndChild();
			ImGui.End();
		}
	}

	private void DrawPluginList(List<PluginWrapper> plugins, ref int freeNegativeIndex)
	{
		ImGui.PushItemWidth(ImGui.GetContentRegionAvail().X);
		ImGui.InputTextWithHint("", "Filter", ref _pluginNameFilter, 200u);
		ImGui.PopItemWidth();
		CorePluginSettings.PluginFolderSettings folderSettings = _CoreSettings.PluginSettings.FolderSettings;
		HashSet<Guid> pluginFolderIds = folderSettings.PluginFolders.Select((CorePluginSettings.PluginFolderSettings.PluginFolder x) => x.Id).ToHashSet();
		ILookup<Guid?, PluginWrapper> lookup = ((IEnumerable<PluginWrapper>)plugins).ToLookup((Func<PluginWrapper, Guid?>)delegate(PluginWrapper x)
		{
			Guid? valueOrDefault = folderSettings.PluginFolderMapping.GetValueOrDefault(x.Plugin.InternalName);
			if (valueOrDefault.HasValue)
			{
				Guid valueOrDefault2 = valueOrDefault.GetValueOrDefault();
				if (pluginFolderIds.Contains(valueOrDefault2))
				{
					return valueOrDefault2;
				}
			}
			return null;
		});
		int num = 0;
		foreach (CorePluginSettings.PluginFolderSettings.PluginFolder item in folderSettings.PluginFolders.Prepend(null))
		{
			System.Numerics.Vector2 cursorPos = ImGui.GetCursorPos();
			if (item == null || ImGui.TreeNodeEx(item.Name, ImGuiTreeNodeFlags.NoTreePushOnOpen | ((!item.CollapsedByDefault) ? ImGuiTreeNodeFlags.DefaultOpen : ImGuiTreeNodeFlags.None)))
			{
				foreach (PluginWrapper item2 in lookup[item?.Id])
				{
					bool isEnable = item2.IsEnable;
					num++;
					if (string.IsNullOrEmpty(_pluginNameFilter) || item2.Name.Contains(_pluginNameFilter, StringComparison.OrdinalIgnoreCase))
					{
						DrawPluginListItem(num, item2, plugins, isEnable);
					}
				}
			}
			if (item == null && !lookup.Contains(null) && ImGui.GetIO().KeyShift)
			{
				ImGui.Text("Default group");
			}
			ImGuiHelpers.DrawAllColumnsBox("##DragTarget", cursorPos);
			if (ImGui.BeginDragDropTarget())
			{
				int? num2 = ImGuiHelpers.AcceptDragDropPayload<int>("PluginIndex");
				if (num2.HasValue && ImGui.IsMouseReleased(ImGuiMouseButton.Left) && num2 >= 0)
				{
					_CoreSettings.PluginSettings.FolderSettings.PluginFolderMapping[plugins[num2.Value].Plugin.InternalName] = item?.Id;
				}
				ImGui.EndDragDropTarget();
			}
		}
		PluginManager pluginManager = core.pluginManager;
		if (pluginManager == null)
		{
			return;
		}
		if (!pluginManager.FailedSourcePlugins.IsEmpty)
		{
			ImGui.Separator();
		}
		foreach (KeyValuePair<string, string> failedSourcePlugin in pluginManager.FailedSourcePlugins)
		{
			failedSourcePlugin.Deconstruct(out var key, out var value);
			string directory = key;
			string errText = value;
			string pluginName = Path.GetFileName(directory);
			ImGui.PushStyleColor(ImGuiCol.Text, Color.Red.ToImgui());
			bool flag = _index == --freeNegativeIndex;
			bool num3 = ImGui.Selectable(pluginName, flag);
			ImGui.PopStyleColor();
			if (num3 || flag)
			{
				_index = freeNegativeIndex;
				Selected = delegate
				{
					DrawFailedPluginMenu(directory, pluginName, errText);
				};
			}
		}
	}

	private void DrawPluginListItem(int index, PluginWrapper plugin, List<PluginWrapper> plugins, bool isEnabled)
	{
		ImGui.PushID(index.ToString());
		if (ImGui.Checkbox("###checkbox", ref isEnabled))
		{
			plugin.TurnOnOffPlugin(isEnabled);
		}
		ImGui.SameLine();
		if (ImGui.Selectable(plugin.Name + "###select", _index == index))
		{
			_index = index;
			Selected = plugin.DrawSettings;
		}
		if (ImGui.GetIO().KeyShift && ImGui.BeginDragDropSource())
		{
			ImGuiHelpers.SetDragDropPayload("PluginIndex", plugins.IndexOf(plugin));
			ImGui.Text(plugin.Name);
			ImGui.EndDragDropSource();
		}
		if (ImGui.GetIO().KeyShift && ImGui.BeginPopupContextItem($"Actions for {plugin.Name}##{index}", ImGuiPopupFlags.MouseButtonRight))
		{
			if (core.pluginManager != null && plugin.Kind == PluginKind.Source && ImGui.Button("Reload"))
			{
				core.pluginManager.ReloadSourcePlugin(plugin.PathOnDisk);
				ImGui.CloseCurrentPopup();
			}
			ImGui.EndPopup();
		}
		ImGui.PopID();
	}

	private void DrawCompilationIsDisabledTab()
	{
		ImGui.TextWrapped("Plugin compilation is disabled. This means there's an issue with the .NET SDK or you've created disable_plugin_compilation.txt in HUD root to disable loading the plugin compiler. To enable plugin compilation, fix the issues and/or delete the file");
		if (ImGui.Button("Open wiki to learn about common .NET SDK issues"))
		{
			Process.Start(new ProcessStartInfo
			{
				FileName = "https://github.com/exApiTools/exApiWiki/wiki/Common-.NET-SDK-installation-issues",
				UseShellExecute = true
			});
		}
	}

	private void DrawFailedPluginMenu(string directory, string pluginName, string errText)
	{
		if (ImGui.Button("Try to rebuild"))
		{
			core.pluginManager?.LoadFailedSourcePlugin(directory);
		}
		if (ImGui.Button("Open plugin folder"))
		{
			Process.Start("explorer.exe", new string[1] { directory });
		}
		ImGui.Text("Plugin " + pluginName + " failed to compile with errors:");
		System.Numerics.Vector2 contentRegionAvail = ImGui.GetContentRegionAvail();
		float x = ImGui.CalcTextSize("A").X;
		int value = (int)(contentRegionAvail.X / x);
		string input = new Regex($"([^\\n]{{1,{value}}}(?=\\s|$)|[^\\n]{{{value}}})\\s*\\n?").Replace(errText.ReplaceLineEndings("\n"), "$1\n");
		ImGui.InputTextMultiline("##err", ref input, 10000u, contentRegionAvail, ImGuiInputTextFlags.ReadOnly);
	}

	private void DebugWindowRender()
	{
		bool p_open = _CoreSettings.ShowDebugWindow.Value;
		ImGui.Begin("Debug window", ref p_open);
		_CoreSettings.ShowDebugWindow.Value = p_open;
		if (sw.ElapsedMilliseconds > 1000)
		{
			sw.Restart();
		}
		ImGui.Text("Program work: ");
		ImGui.SameLine();
		ImGui.TextColored(Color.GreenYellow.ToImguiVec4(), swStartedProgram.ElapsedMilliseconds.ToString());
		ImGui.BeginTabBar("Performance tabs");
		Windows[] windowsName = WindowsName;
		foreach (Windows value in windowsName)
		{
			if (ImGui.BeginTabItem($"{value}##WindowName"))
			{
				OpenWindow = value;
				ImGui.EndTabItem();
			}
		}
		ImGui.EndTabBar();
		switch (OpenWindow)
		{
		case Windows.MainDebugs:
			DrawMainDebugInfo(MainDebugs, Core.DebugInformations[0]);
			break;
		case Windows.NotMainDebugs:
			if (!ImGui.BeginTable("Deb", 4, ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit))
			{
				break;
			}
			ImGui.TableSetupColumn("Name");
			ImGui.TableSetupColumn("Tick");
			ImGui.TableSetupColumn("Total");
			ImGui.TableSetupColumn($"Data for {DebugInformation.SizeArray / (int)_CoreSettings.PerformanceSettings.TargetFps} sec.");
			ImGui.TableHeadersRow();
			foreach (DebugInformation notMainDebug in NotMainDebugs)
			{
				ImGui.TableNextRow(ImGuiTableRowFlags.None, 24f);
				DrawInfoForNotMainDebugInformation(notMainDebug);
			}
			ImGui.EndTable();
			break;
		case Windows.Plugins:
			if (AllPlugins == null)
			{
				AllPlugins = Core.DebugInformations.FirstOrDefault((DebugInformation x) => x.Name == "All plugins");
			}
			else
			{
				DrawMainDebugInfo(PluginsDebug, AllPlugins, new MainDebugTableRecord(AllPlugins, Core.DebugInformations[0], MainDebugs.Count));
			}
			break;
		case Windows.Coroutines:
			DrawCoroutineRunnerInfo(core.CoroutineRunner);
			DrawCoroutineRunnerInfo(core.CoroutineRunnerParallel);
			if (!ImGui.CollapsingHeader("Finished coroutines"))
			{
				break;
			}
			foreach (CoroutineDetails finishedCoroutine in Core.MainRunner.FinishedCoroutines)
			{
				ImGui.Text($"{finishedCoroutine.Name} - {finishedCoroutine.Ticks} - {finishedCoroutine.OwnerName} - {finishedCoroutine.Started} - {finishedCoroutine.Finished}");
			}
			foreach (CoroutineDetails finishedCoroutine2 in Core.ParallelRunner.FinishedCoroutines)
			{
				ImGui.Text($"{finishedCoroutine2.Name} - {finishedCoroutine2.Ticks} - {finishedCoroutine2.OwnerName} - {finishedCoroutine2.Started} - {finishedCoroutine2.Finished}");
			}
			break;
		case Windows.Caches:
		{
			Cache cache = core.GameController.Cache;
			if (ImGui.BeginTable("Cache table", 6, ImGuiTableFlags.Borders))
			{
				ImGui.TableSetupColumn("Name");
				ImGui.TableSetupColumn("Count");
				ImGui.TableSetupColumn("Memory read");
				ImGui.TableSetupColumn("Cache read");
				ImGui.TableSetupColumn("Deleted");
				ImGui.TableSetupColumn("% Read from memory");
				ImGui.TableHeadersRow();
				(string, IStaticCache)[] array = new(string, IStaticCache)[1] { ("String cache", cache.StringCache) };
				for (int i = 0; i < array.Length; i++)
				{
					(string, IStaticCache) tuple = array[i];
					ImGui.TableNextColumn();
					ImGui.Text(tuple.Item1);
					ImGui.TableNextColumn();
					ImGui.Text($"{tuple.Item2.Count}");
					ImGui.TableNextColumn();
					ImGui.Text($"{tuple.Item2.ReadMemory}");
					ImGui.TableNextColumn();
					ImGui.Text($"{tuple.Item2.ReadCache}");
					ImGui.TableNextColumn();
					ImGui.Text($"{tuple.Item2.DeletedCache}");
					ImGui.TableNextColumn();
					ImGui.Text($"{tuple.Item2.Coeff} %%");
				}
				ImGui.EndTable();
			}
			if (ImGui.Button("Clear caches"))
			{
				cache.TryClearCache();
			}
			break;
		}
		}
		MoreInformation?.Invoke();
		ImGui.End();
	}

	private void DrawInfoForNotMainDebugInformation(DebugInformation deb)
	{
		ImGui.TableNextColumn();
		if (selectedName == deb.Name)
		{
			ImGui.PushStyleColor(ImGuiCol.Text, Color.OrangeRed.ToImgui());
		}
		ImGui.Text(deb.Name ?? "");
		if (ImGui.IsItemClicked() && deb.Index > 0)
		{
			MoreInformation = delegate
			{
				AddtionalInfo(deb);
			};
		}
		if (!string.IsNullOrEmpty(deb.Description))
		{
			ImGui.SameLine();
			ImGui.TextDisabled("(?)");
			if (ImGui.IsItemHovered(ImGuiHoveredFlags.None))
			{
				ImGui.SetTooltip(deb.Description);
			}
		}
		ImGui.TableNextColumn();
		ImGui.Text($"{deb.Tick:0.0000}");
		ImGui.TableNextColumn();
		ImGui.TextUnformatted($"{deb.Total:0.000}");
		ImGui.TableNextColumn();
		float num = 0f;
		float value = 0f;
		if (deb.AtLeastOneFullTick)
		{
			num = deb.Ticks.AverageF();
			value = deb.Ticks.MinF();
		}
		else
		{
			float[] array = deb.Ticks.Take(deb.Index).ToArray();
			if (array.Length != 0)
			{
				num = array.Average();
				value = array.Min();
			}
		}
		ImGui.Text($"Min: {value:0.000} Max: {deb.Ticks.MaxF():00.000} Avg: {num:0.000} TAMax: {deb.TotalMaxAverage:00.000}");
		if (num >= (float)_CoreSettings.PerformanceSettings.LimitDrawPlot)
		{
			ImGui.SameLine();
			ImGui.PlotLines("##Plot" + deb.Name, ref deb.Ticks[0], DebugInformation.SizeArray);
		}
		if (selectedName == deb.Name)
		{
			ImGui.PopStyleColor();
		}
	}

	private void AddtionalInfo(DebugInformation deb)
	{
		selectedName = deb.Name;
		if (!deb.AtLeastOneFullTick)
		{
			ImGui.Text($"Info {deb.Name} - {(float)(DebugInformation.SizeArray / (int)_CoreSettings.PerformanceSettings.TargetFps) / 60f:0.00} sec. Index: {deb.Index}/{DebugInformation.SizeArray}");
			float scale_min = deb.Ticks.Min();
			float num = deb.Ticks.Max();
			float windowWidth = ImGui.GetWindowWidth();
			ImGui.PlotHistogram("##Plot" + deb.Name, ref deb.Ticks[0], DebugInformation.SizeArray, 0, $"Avg: {deb.Ticks.Where((float x) => x > 0f).Average():0.0000} Max {num:0.0000}", scale_min, num, new System.Numerics.Vector2(windowWidth - 10f, 150f));
			if (ImGui.Button("Close##" + deb.Name))
			{
				MoreInformation = null;
			}
			return;
		}
		ImGui.Text($"Info {deb.Name} - {(float)(DebugInformation.SizeArray * DebugInformation.SizeArray / (int)_CoreSettings.PerformanceSettings.TargetFps) / 60f:0.00} sec. Index: {deb.Index}/{DebugInformation.SizeArray}");
		float scale_min2 = deb.TicksAverage.MinF();
		float num2 = deb.TicksAverage.MaxF();
		float scale_max = deb.Ticks.MaxF();
		float windowWidth2 = ImGui.GetWindowWidth();
		ImGui.PlotHistogram("##Plot" + deb.Name, ref deb.Ticks[0], DebugInformation.SizeArray, 0, $"{deb.Tick:0.000}", 0f, scale_max, new System.Numerics.Vector2(windowWidth2 - 50f, 150f));
		float[] array = deb.TicksAverage.Where((float x) => x > 0f).ToArray();
		if (array.Length != 0)
		{
			ImGui.Text($"Index: {deb.IndexTickAverage}/{DebugInformation.SizeArray}");
			ImGui.PlotHistogram("##Plot" + deb.Name, ref deb.TicksAverage[0], DebugInformation.SizeArray, 0, $"Avg: {array.Average():0.0000} Max {num2:0.0000}", scale_min2, num2, new System.Numerics.Vector2(windowWidth2 - 50f, 150f));
		}
		else
		{
			ImGui.Text("Dont have information");
		}
		if (ImGui.Button("Close##" + deb.Name))
		{
			MoreInformation = null;
		}
	}

	private void DrawMainDebugInfo(List<DebugInformation> listSource, DebugInformation totalSource, MainDebugTableRecord firstLine = null)
	{
		if (!ImGui.BeginTable("Deb", 6, ImGuiTableFlags.Borders | ImGuiTableFlags.Sortable | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.SortTristate))
		{
			return;
		}
		ImGui.TableSetupColumn("Name");
		ImGui.TableSetupColumn("%");
		ImGui.TableSetupColumn("Tick");
		ImGui.TableSetupColumn("Total");
		ImGui.TableSetupColumn("Total %");
		ImGui.TableSetupColumn($"Data for {DebugInformation.SizeArray / (int)_CoreSettings.PerformanceSettings.TargetFps} sec.");
		ImGui.TableHeadersRow();
		IEnumerable<MainDebugTableRecord> source = listSource.Select((DebugInformation x) => new MainDebugTableRecord(x, totalSource, listSource.Count));
		ImGuiTableSortSpecsPtr sortSpecs = ImGui.TableGetSortSpecs();
		if (sortSpecs.SpecsCount > 0)
		{
			if (sortSpecs.Specs.SortDirection == ImGuiSortDirection.Ascending)
			{
				source = source.OrderBy(SortSelector);
			}
			else if (sortSpecs.Specs.SortDirection == ImGuiSortDirection.Descending)
			{
				source = source.OrderByDescending(SortSelector);
			}
		}
		if (firstLine != null)
		{
			source = source.Prepend(firstLine);
		}
		source = source.ToList();
		float maxTotal = source.Max((MainDebugTableRecord x) => x.Total);
		foreach (MainDebugTableRecord item in source)
		{
			ImGui.TableNextRow(ImGuiTableRowFlags.None, 24f);
			DrawMainDebugInfoLine(item, maxTotal);
		}
		ImGui.EndTable();
		object SortSelector(MainDebugTableRecord x)
		{
			short columnIndex = sortSpecs.Specs.ColumnIndex;
			switch (columnIndex)
			{
			case 0:
				return x.Name;
			case 1:
				return x.Percent;
			case 2:
				return x.Tick;
			case 3:
				return x.Total;
			case 4:
				return x.TotalPercent;
			default:
			{
				global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(columnIndex);
				object result = default(object);
				return result;
			}
			}
		}
	}

	private void DrawMainDebugInfoLine(MainDebugTableRecord debug, float maxTotal)
	{
		ImGui.TableNextColumn();
		ImGui.PushStyleColor(ImGuiCol.HeaderHovered, Color.Transparent.ToImgui());
		ImGui.PushStyleColor(ImGuiCol.HeaderActive, Color.Transparent.ToImgui());
		if (selectedName == debug.Name)
		{
			ImGui.PushStyleColor(ImGuiCol.Text, Color.OrangeRed.ToImgui());
		}
		ImGui.Selectable(debug.Name ?? "", selected: false, ImGuiSelectableFlags.SpanAllColumns);
		if (ImGui.IsItemClicked())
		{
			MoreInformation = delegate
			{
				AddtionalInfo(debug.Current);
			};
		}
		if (!string.IsNullOrEmpty(debug.Current.Description))
		{
			ImGui.SameLine();
			ImGui.TextDisabled("(?)");
			if (ImGui.IsItemHovered(ImGuiHoveredFlags.None))
			{
				ImGui.SetTooltip(debug.Current.Description);
			}
		}
		ImGui.TableNextColumn();
		float num = debug.Current.Ticks.AverageF();
		float num2 = num / debug.AllPluginAverage;
		Color c = ((num2 <= 0.5f) ? Color.Green : ((num2 >= 4f) ? Color.Red : ((!(num2 >= 1.5f)) ? Color.Yellow : Color.Orange)));
		System.Numerics.Vector4 col = c.ToImguiVec4();
		ImGui.TextColored(col, $"{debug.Percent:0.00} %%".PadLeft(9));
		ImGui.TableNextColumn();
		ImGui.TextColored(col, $"{debug.Tick:0.0000}".PadLeft(8));
		ImGui.TableNextColumn();
		ImGui.TextColored(col, $"{debug.Total:0.000}".PadLeft(Math.Max(0, (int)Math.Floor(Math.Log10(maxTotal)) + 5)));
		ImGui.TableNextColumn();
		ImGui.TextColored(col, $"{debug.TotalPercent:0.00} %%".PadLeft(9));
		ImGui.TableNextColumn();
		ImGui.Text($"Min: {debug.Current.Ticks.Min():0.000} Max: {debug.Current.Ticks.Max():00.000} Avg: {num:0.000} TAMax: {debug.Current.TotalMaxAverage:00.000}");
		if (num >= (float)_CoreSettings.PerformanceSettings.LimitDrawPlot)
		{
			ImGui.SameLine();
			ImGui.PlotLines("##Plot" + debug.Name, ref debug.Current.Ticks[0], DebugInformation.SizeArray);
		}
		if (selectedName == debug.Name)
		{
			ImGui.PopStyleColor();
		}
		ImGui.PopStyleColor(2);
	}

	private void DrawCoroutineRunnerInfo(Runner runner)
	{
		ImGui.Text(runner.Name ?? "");
		if (!ImGui.BeginTable("CoroutineTable", 11, ImGuiTableFlags.Borders))
		{
			return;
		}
		ImGui.TableSetupColumn("Name");
		ImGui.TableSetupColumn("Owner");
		ImGui.TableSetupColumn("Ticks");
		ImGui.TableSetupColumn("Time ms");
		ImGui.TableSetupColumn("Started");
		ImGui.TableSetupColumn("Timeout");
		ImGui.TableSetupColumn("DoWork");
		ImGui.TableSetupColumn("AutoResume");
		ImGui.TableSetupColumn("Done");
		ImGui.TableSetupColumn("Priority");
		ImGui.TableSetupColumn("DO");
		ImGui.TableHeadersRow();
		foreach (Coroutine item in runner.Coroutines.OrderByDescending((Coroutine x) => x.Priority).ToList())
		{
			ImGui.TableNextColumn();
			string text = "";
			if (item.Condition != null)
			{
				text = item.Condition.GetType().Name;
			}
			ImGui.Text(item.Name ?? "");
			ImGui.TableNextColumn();
			ImGui.Text(item.OwnerName ?? "");
			ImGui.TableNextColumn();
			ImGui.Text($"{item.Ticks}");
			ImGui.TableNextColumn();
			ImGui.Text($"{Math.Round(runner.CoroutinePerformance.GetValueOrDefault(item.Name), 2)}");
			ImGui.TableNextColumn();
			ImGui.Text(item.Started.ToLongTimeString() ?? "");
			ImGui.TableNextColumn();
			ImGui.Text(text + ": " + item.TimeoutForAction);
			ImGui.TableNextColumn();
			ImGui.Text($"{item.Running}");
			ImGui.TableNextColumn();
			ImGui.Text($"{item.AutoResume}");
			ImGui.TableNextColumn();
			ImGui.Text($"{item.IsDone}");
			ImGui.TableNextColumn();
			ImGui.Text($"{item.Priority}");
			ImGui.TableNextColumn();
			if (item.Running)
			{
				if (ImGui.Button("Pause##" + item.Name + "##" + runner.Name))
				{
					item.Pause();
				}
			}
			else if (ImGui.Button("Start##" + item.Name + "##" + runner.Name))
			{
				item.Resume();
			}
			ImGui.SameLine();
			if (ImGui.Button("Done##" + item.Name + "##" + runner.Name))
			{
				item.Done();
			}
		}
		ImGui.EndTable();
	}
}
