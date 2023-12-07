using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExileCore.PoEMemory;
using ExileCore.RenderQ;
using ExileCore.Shared;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Nodes;
using ImGuiNET;
using JM.LinqFaster;
using Serilog;
using SharpDX;
using Vanara.PInvoke;

namespace ExileCore;

public class Core : IDisposable
{
	public static object SyncLocker = new object();

	private static readonly DebugInformation TotalDebugInformation = new DebugInformation("Total", "Total waste time");

	private static readonly DebugInformation CoroutineTickDebugInformation = new DebugInformation("Coroutine Tick");

	private static readonly DebugInformation CoreDebugInformation = new DebugInformation("Core");

	private static readonly DebugInformation MenuDebugInformation = new DebugInformation("Menu+Debug");

	private static readonly DebugInformation GcTickDebugInformation = new DebugInformation("GameController Tick");

	private static readonly DebugInformation AllPluginsDebugInformation = new DebugInformation("All plugins");

	private static readonly DebugInformation InterFrameInformation = new DebugInformation("Inter-frame", "Coroutines + sleep");

	private static readonly DebugInformation DeltaTimeDebugInformation = new DebugInformation("Delta Time", main: false);

	private static readonly DebugInformation FpsCounterDebugInformation = new DebugInformation("Fps counter", main: false);

	private static readonly DebugInformation ParallelCoroutineTickDebugInformation = new DebugInformation("Parallel Coroutine Tick");

	private const int JOB_TIMEOUT_MS = 200;

	private const int TICKS_BEFORE_SLEEP = 4;

	private readonly ActionOverlay _overlay;

	private readonly CoreSettings _coreSettings;

	private readonly DebugWindow _debugWindow;

	private readonly DX11 _dx11;

	private readonly WaitTime _mainControl = new WaitTime(2000);

	private readonly WaitTime _mainControl2 = new WaitTime(250);

	private readonly MenuWindow _mainMenu;

	private readonly SettingsContainer _settings;

	private readonly SoundController _soundController;

	private readonly List<(PluginWrapper plugin, Job job)> WaitingJobs = new List<(PluginWrapper, Job)>(20);

	private Memory _memory;

	private bool _memoryValid = true;

	private double _targetParallelFpsTime;

	private double ForeGroundTime;

	private int frameCounter;

	private System.Drawing.Rectangle lastClientBound;

	private double lastCounterTime;

	private double NextCoroutineTime;

	private double NextRender;

	private readonly TaskCompletionSource _readyToRun = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

	private IDisposable _totalTick;

	public static Core Current;

	public static ObservableCollection<DebugInformation> DebugInformations { get; } = new ObservableCollection<DebugInformation>();


	public static ILogger Logger { get; set; }

	public static Runner MainRunner { get; set; }

	public static Runner ParallelRunner { get; set; }

	public static uint FramesCount { get; private set; }

	public double TargetPcFrameTime { get; private set; }

	public MultiThreadManager MultiThreadManager { get; private set; }

	public PluginManager pluginManager { get; private set; }

	public Runner CoroutineRunner { get; set; }

	public Runner CoroutineRunnerParallel { get; set; }

	public GameController GameController { get; private set; }

	public bool GameStarted { get; private set; }

	public Graphics Graphics { get; }

	public bool IsForeground { get; private set; }

	public Core(CancellationToken cancellationToken)
	{
		try
		{
			_overlay = new ActionOverlay("ExileApi")
			{
				PostInitializedAction = async delegate
				{
					User32.MONITORINFO windowMonitorInfo = _overlay.WindowMonitorInfo;
					lastClientBound = new System.Drawing.Rectangle(windowMonitorInfo.rcWork.Location, windowMonitorInfo.rcWork.Size);
					await _readyToRun.Task;
				}
			};
			cancellationToken.Register(_overlay.Close);
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "textures\\icon.ico");
			if (File.Exists(text))
			{
				_overlay.SetIcon(text);
			}
			_overlay.Start().Wait();
			_settings = new SettingsContainer();
			_coreSettings = _settings.CoreSettings;
			_coreSettings.PerformanceSettings.Threads = new RangeNode<int>(_coreSettings.PerformanceSettings.Threads.Value, 0, Environment.ProcessorCount);
			CoroutineRunner = new Runner("Main Coroutine");
			CoroutineRunnerParallel = new Runner("Parallel Coroutine");
			using (new PerformanceTimer("DX11 Load"))
			{
				_dx11 = new DX11(_overlay, _coreSettings);
			}
			if (Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor == 1)
			{
				Logger.Information("SoundController init skipped because win7 issue.");
			}
			else
			{
				_soundController = new SoundController("Sounds");
			}
			_coreSettings.Volume.OnValueChanged += delegate(object? _, int i)
			{
				_soundController.SetVolume((float)i / 100f);
			};
			_overlay.VSync = _coreSettings.EnableVSync.Value;
			_coreSettings.EnableVSync.OnValueChanged += delegate
			{
				_overlay.VSync = _coreSettings.EnableVSync.Value;
			};
			Graphics = new Graphics(_dx11, _coreSettings);
			MainRunner = CoroutineRunner;
			ParallelRunner = CoroutineRunnerParallel;
			Thread thread = new Thread(ParallelCoroutineManualThread);
			thread.Name = "Parallel Coroutine";
			thread.IsBackground = true;
			thread.Start();
			_mainMenu = new MenuWindow(this, _settings);
			_debugWindow = new DebugWindow(Graphics, _coreSettings);
			MultiThreadManager = new MultiThreadManager(_coreSettings.PerformanceSettings.Threads);
			CoroutineRunner.MultiThreadManager = MultiThreadManager;
			_coreSettings.PerformanceSettings.Threads.OnValueChanged += delegate(object? _, int i)
			{
				if (MultiThreadManager == null)
				{
					MultiThreadManager = new MultiThreadManager(i);
				}
				else
				{
					Coroutine routine2 = new Coroutine(delegate
					{
						MultiThreadManager.ChangeNumberThreads(_coreSettings.PerformanceSettings.Threads);
					}, new WaitTime(2000), null, "Change Threads Number", infinity: false)
					{
						SyncModWork = true
					};
					ParallelRunner.Run(routine2);
				}
			};
			TargetPcFrameTime = 1000f / (float)(int)_coreSettings.PerformanceSettings.TargetFps;
			_targetParallelFpsTime = 1000f / (float)(int)_coreSettings.PerformanceSettings.TargetParallelCoroutineFps;
			_coreSettings.PerformanceSettings.TargetFps.OnValueChanged += delegate(object? _, int i)
			{
				TargetPcFrameTime = 1000f / (float)i;
			};
			_coreSettings.PerformanceSettings.TargetParallelCoroutineFps.OnValueChanged += delegate(object? _, int i)
			{
				_targetParallelFpsTime = 1000f / (float)i;
			};
			if (_memory == null)
			{
				_memory = FindPoe();
			}
			if (GameController == null && _memory != null)
			{
				Inject();
			}
			Coroutine routine = new Coroutine(MainControl(), null, "Render control")
			{
				Priority = CoroutinePriority.Critical
			};
			CoroutineRunnerParallel.Run(routine);
			NextCoroutineTime = Time.TotalMilliseconds;
			NextRender = Time.TotalMilliseconds;
			PluginManager obj = pluginManager;
			if (obj != null && obj.Plugins.Count == 0)
			{
				_coreSettings.Enable.Value = true;
			}
			Graphics.InitImage("missing_texture.png");
			Current = this;
		}
		catch (Exception value)
		{
			Logger?.Error($"Core constructor -> {value}");
			MessageBox.Show($"Error in Core constructor -> {value}", "Oops... Program failed to launch");
			throw;
		}
	}

	public void Dispose()
	{
		_memory?.Dispose();
		_mainMenu?.Dispose();
		GameController?.Dispose();
		_dx11?.Dispose();
		pluginManager?.CloseAllPlugins();
	}

	private IEnumerator MainControl()
	{
		while (true)
		{
			if (_memory == null)
			{
				_memory = FindPoe();
				if (_memory == null)
				{
					yield return _mainControl;
				}
				continue;
			}
			if (GameController == null && _memory != null)
			{
				Inject();
				if (GameController == null)
				{
					yield return _mainControl;
				}
				continue;
			}
			System.Drawing.Rectangle clientRectangle = WinApi.GetClientRectangle(_memory.Process.MainWindowHandle);
			if (lastClientBound != clientRectangle && clientRectangle.Width > 2 && clientRectangle.Height > 2)
			{
				DebugWindow.LogMsg($"Resize from: {lastClientBound} to {clientRectangle}", 5f, SharpDX.Color.Magenta);
				lastClientBound = clientRectangle;
			}
			_memoryValid = !_memory.IsInvalid();
			if (!_memoryValid)
			{
				pluginManager?.CloseAllPlugins();
				pluginManager = null;
				GameController?.Dispose();
				GameController = null;
				_memory = null;
				goto IL_01ec;
			}
			if (WinApi.IsForegroundWindow(_memory.Process.MainWindowHandle))
			{
				goto IL_01d4;
			}
			IntPtr? windowHandle = _overlay.WindowHandle;
			if (windowHandle.HasValue)
			{
				IntPtr valueOrDefault = windowHandle.GetValueOrDefault();
				if (WinApi.IsForegroundWindow(valueOrDefault))
				{
					goto IL_01d4;
				}
			}
			int num = (((bool)_coreSettings.ForceForeground) ? 1 : 0);
			goto IL_01d5;
			IL_01d5:
			bool flag = (byte)num != 0;
			IsForeground = flag;
			GameController.IsForeGroundCache = flag;
			goto IL_01ec;
			IL_01ec:
			yield return _mainControl2;
			continue;
			IL_01d4:
			num = 1;
			goto IL_01d5;
		}
	}

	public static Memory FindPoe()
	{
		(Process, Offsets)? tuple = FindPoeProcess();
		if (!tuple.HasValue || tuple.Value.Item1.Id == 0)
		{
			DebugWindow.LogMsg("Game not found");
			return null;
		}
		return new Memory(tuple.Value);
	}

	private void Inject()
	{
		try
		{
			if (_memory != null)
			{
				GameController = new GameController(_memory, _soundController, _settings, MultiThreadManager);
				using (new PerformanceTimer("Plugin loader"))
				{
					pluginManager = new PluginManager(GameController, Graphics, MultiThreadManager);
					return;
				}
			}
		}
		catch (Exception value)
		{
			DebugWindow.LogError($"Inject -> {value}");
		}
	}

	private void OnFocusLoss(object sender, EventArgs eventArgs)
	{
		IntPtr? intPtr = _memory?.Process?.MainWindowHandle;
		if (intPtr.HasValue)
		{
			IntPtr valueOrDefault = intPtr.GetValueOrDefault();
			if (valueOrDefault != IntPtr.Zero && !WinApi.IsIconic(valueOrDefault))
			{
				WinApi.SetForegroundWindow(valueOrDefault);
			}
		}
	}

	public void Tick()
	{
		try
		{
			using (CoreDebugInformation.Measure())
			{
				using (_coreSettings.ApplySelectedFontGlobally ? Graphics.UseCurrentFont() : null)
				{
					ImGui.GetStyle().MouseCursorScale = _coreSettings.MouseCursorScale.Value;
					_memory?.NotifyFrame();
					Input.Update(_overlay.WindowHandle);
					using (MenuDebugInformation.Measure())
					{
						FramesCount++;
						NextFrameTask.NextFrameAwaiter.SetNextFrame();
						if (!IsForeground)
						{
							ForeGroundTime += DeltaTimeDebugInformation.Tick;
						}
						else
						{
							ForeGroundTime = 0.0;
						}
						if (ForeGroundTime <= 100.0)
						{
							try
							{
								_debugWindow?.Render();
							}
							catch (Exception value)
							{
								DebugWindow.LogError($"DebugWindow Tick -> {value}");
							}
							try
							{
								_mainMenu?.Render(GameController);
							}
							catch (Exception value2)
							{
								DebugWindow.LogError($"Core Tick Menu -> {value2}");
							}
						}
					}
					if (GameController == null)
					{
						return;
					}
					using (GcTickDebugInformation.Measure())
					{
						GameController.Tick();
					}
					using (AllPluginsDebugInformation.Measure())
					{
						if (!(ForeGroundTime <= 150.0) || pluginManager == null)
						{
							return;
						}
						WaitingJobs.Clear();
						if ((bool)_coreSettings.PerformanceSettings.CollectDebugInformation)
						{
							foreach (PluginWrapper plugin in pluginManager.Plugins)
							{
								if (!plugin.IsEnable || (!GameController.InGame && !plugin.Force))
								{
									continue;
								}
								plugin.CanRender = true;
								Job job = plugin.PerfomanceTick();
								if (job == null)
								{
									continue;
								}
								if (MultiThreadManager.ThreadsCount > 0)
								{
									if (!job.IsStarted)
									{
										MultiThreadManager.AddJob(job);
									}
									WaitingJobs.Add((plugin, job));
								}
								else
								{
									plugin.TickDebugInformation.TickAction(job.Work);
								}
							}
						}
						else
						{
							foreach (PluginWrapper plugin2 in pluginManager.Plugins)
							{
								if (!plugin2.IsEnable || (!GameController.InGame && !plugin2.Force))
								{
									continue;
								}
								plugin2.CanRender = true;
								Job job2 = plugin2.Tick();
								if (job2 == null)
								{
									continue;
								}
								if (MultiThreadManager.ThreadsCount > 0)
								{
									if (!job2.IsStarted)
									{
										MultiThreadManager.AddJob(job2);
									}
									WaitingJobs.Add((plugin2, job2));
								}
								else
								{
									job2.Work();
								}
							}
						}
						if (WaitingJobs.Count > 0)
						{
							MultiThreadManager.Process(this);
							SpinWait.SpinUntil(() => WaitingJobs.AllF(((PluginWrapper plugin, Job job) x) => x.job.IsCompleted), 200);
							if ((bool)_coreSettings.PerformanceSettings.CollectDebugInformation)
							{
								foreach (var waitingJob in WaitingJobs)
								{
									waitingJob.plugin.TickDebugInformation.CorrectAfterTick((float)waitingJob.job.ElapsedMs);
									if (waitingJob.job.IsFailed && waitingJob.job.IsCompleted)
									{
										waitingJob.plugin.CanRender = false;
										DebugWindow.LogMsg($"{waitingJob.plugin.Name} job timeout: {waitingJob.job.ElapsedMs} ms. Thread# {waitingJob.job.WorkingOnThread}");
									}
								}
							}
							else
							{
								foreach (var waitingJob2 in WaitingJobs)
								{
									if (waitingJob2.job.IsFailed)
									{
										waitingJob2.plugin.CanRender = false;
									}
								}
							}
						}
						if ((bool)_coreSettings.PerformanceSettings.CollectDebugInformation)
						{
							foreach (PluginWrapper plugin3 in pluginManager.Plugins)
							{
								if (plugin3.IsEnable && plugin3.CanRender && (GameController.InGame || plugin3.Force))
								{
									plugin3.PerfomanceRender();
								}
							}
							return;
						}
						foreach (PluginWrapper plugin4 in pluginManager.Plugins)
						{
							if (plugin4.IsEnable && (GameController.InGame || plugin4.Force))
							{
								plugin4.Render();
							}
						}
					}
				}
			}
		}
		catch (Exception value3)
		{
			DebugWindow.LogError($"Core tick -> {value3}");
		}
	}

	private static (Process process, Offsets offsets)? FindPoeProcess()
	{
		List<(Process, Offsets)> list = (from x in new Offsets[3]
			{
				Offsets.Regular,
				Offsets.Korean,
				Offsets.Steam
			}.SelectMany((Offsets o) => from p in Process.GetProcessesByName(o.ExeName)
				select (p, o))
			where !x.p.HasExited
			orderby x.p.Id
			select x).ToList();
		if (!list.Any())
		{
			return null;
		}
		int? num = ((list.Count > 1) ? ProcessPicker.ShowDialogBox(list.Select<(Process, Offsets), Process>(((Process p, Offsets o) x) => x.p)) : new int?(0));
		if (num.HasValue)
		{
			if (num.GetValueOrDefault() == -1)
			{
				return null;
			}
			return list[num.Value];
		}
		Environment.Exit(0);
		return null;
	}

	private void ParallelCoroutineManualThread()
	{
		try
		{
			while (true)
			{
				MultiThreadManager?.Process(this);
				if (!CoroutineRunnerParallel.IsRunning)
				{
					Thread.Sleep(10);
					continue;
				}
				DebugInformation.MeasureHolder measureHolder = ParallelCoroutineTickDebugInformation.Measure();
				try
				{
					for (int i = 0; i < CoroutineRunnerParallel.IterationPerFrame; i++)
					{
						CoroutineRunnerParallel.Update();
					}
				}
				catch (Exception ex)
				{
					DebugWindow.LogMsg("Coroutine Parallel error: " + ex.Message, 6f, SharpDX.Color.White);
				}
				measureHolder.Dispose();
				int num = (int)(_targetParallelFpsTime - measureHolder.Elapsed.TotalMilliseconds);
				if (num > 0)
				{
					Thread.Sleep(num);
				}
			}
		}
		catch (Exception ex2)
		{
			DebugWindow.LogMsg("Coroutine Parallel error: " + ex2.Message, 6f, SharpDX.Color.White);
			throw;
		}
	}

	public void TickCoroutines()
	{
		if (!(NextCoroutineTime <= Time.TotalMilliseconds))
		{
			return;
		}
		NextCoroutineTime += _targetParallelFpsTime;
		using (CoroutineTickDebugInformation.Measure())
		{
			if (CoroutineRunner.IsRunning)
			{
				if ((bool)_coreSettings.PerformanceSettings.CoroutineMultiThreading)
				{
					CoroutineRunner.ParallelUpdate();
				}
				else
				{
					CoroutineRunner.Update();
				}
			}
		}
	}

	private void Render()
	{
		_overlay.Position = lastClientBound.Location;
		_overlay.Size = lastClientBound.Size;
		NextRender = Time.TotalMilliseconds + TargetPcFrameTime;
		_dx11.ImGuiRender.BeginBackGroundWindow();
		Tick();
		frameCounter++;
		WaitRender.Frame();
		if (Time.TotalMilliseconds - lastCounterTime > 1000.0)
		{
			FpsCounterDebugInformation.Tick = frameCounter;
			DeltaTimeDebugInformation.Tick = 1000f / (float)frameCounter;
			lastCounterTime = Time.TotalMilliseconds;
			frameCounter = 0;
		}
	}

	public void Run()
	{
		WinMm.timeBeginPeriod(1u);
		_overlay.FocusLost += OnFocusLoss;
		_overlay.RenderAction = Render;
		_overlay.PostFrameAction = delegate
		{
			_totalTick?.Dispose();
			using (InterFrameInformation.Measure())
			{
				int num = 0;
				do
				{
					TickCoroutines();
					if (++num >= 4)
					{
						Thread.Sleep(1);
						num = 0;
					}
				}
				while (Time.TotalMilliseconds < NextRender);
			}
			_totalTick = TotalDebugInformation.Measure();
		};
		_readyToRun.SetResult();
		_overlay.WaitForShutdown().Wait();
	}
}
