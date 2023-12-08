using ExileCore.Shared;
using ExileCore.Shared.Helpers;
using ImGuiNET;
using Color = System.Drawing.Color;
using RectangleF = System.Drawing.RectangleF;

namespace ExileCore;

public class DebugWindow
{
	private static readonly object locker;

	private static readonly Dictionary<string, DebugMsgDescription> Messages;

	private static readonly List<DebugMsgDescription> MessagesList;

	private static readonly Queue<string> toDelete;

	private static readonly Queue<DebugMsgDescription> LogHistory;

	private static readonly CircularBuffer<DebugMsgDescription> History;

	private readonly Graphics graphics;

	private readonly CoreSettings settingsCoreSettings;

	private System.Numerics.Vector2 position;

	static DebugWindow()
	{
		locker = new object();
		Messages = new Dictionary<string, DebugMsgDescription>(24);
		MessagesList = new List<DebugMsgDescription>(24);
		toDelete = new Queue<string>(24);
		LogHistory = new Queue<DebugMsgDescription>(1024);
		History = new CircularBuffer<DebugMsgDescription>(1024);
	}

	public DebugWindow(Graphics graphics, CoreSettings settingsCoreSettings)
	{
		this.graphics = graphics;
		this.settingsCoreSettings = settingsCoreSettings;
		graphics.InitImage("menu-background.png");
	}

	public void Render()
	{
		if ((bool)settingsCoreSettings.HideAllDebugging)
		{
			return;
		}
		try
		{
			if ((bool)settingsCoreSettings.ShowLogWindow)
			{
				using (graphics.UseCurrentFont())
				{
					ImGui.SetNextWindowPos(new System.Numerics.Vector2(10f, 10f), ImGuiCond.Once);
					ImGui.SetNextWindowSize(new System.Numerics.Vector2(600f, 1000f), ImGuiCond.Once);
					bool p_open = settingsCoreSettings.ShowLogWindow.Value;
					ImGui.Begin("Debug log", ref p_open);
					settingsCoreSettings.ShowLogWindow.Value = p_open;
					foreach (DebugMsgDescription item in History)
					{
						if (item != null)
						{
							ImGui.PushStyleColor(ImGuiCol.Text, item.ColorV4);
							ImGui.TextUnformatted(item.Time.ToLongTimeString() + ": " + item.Msg);
							ImGui.PopStyleColor();
						}
					}
					ImGui.End();
				}
			}
			if (MessagesList.Count == 0)
			{
				return;
			}
			position = new System.Numerics.Vector2(10f, 35f);
			for (int i = 0; i < MessagesList.Count; i++)
			{
				DebugMsgDescription debugMsgDescription = MessagesList[i];
				if (debugMsgDescription == null)
				{
					continue;
				}
				if (debugMsgDescription.Time < DateTime.UtcNow)
				{
					toDelete.Enqueue(debugMsgDescription.Msg);
					continue;
				}
				string text = debugMsgDescription.Msg;
				if (debugMsgDescription.Count > 1)
				{
					text = $"({debugMsgDescription.Count}){text}";
				}
				System.Numerics.Vector2 vector = graphics.MeasureText(text);
				graphics.DrawImage("menu-background.png", new RectangleF(position.X - 5f, position.Y, vector.X + 20f, vector.Y));
				graphics.DrawText(text, position, debugMsgDescription.Color);
				position = new System.Numerics.Vector2(position.X, position.Y + vector.Y);
			}
			while (toDelete.Count > 0)
			{
				string key = toDelete.Dequeue();
				if (Messages.TryGetValue(key, out var value))
				{
					MessagesList.Remove(value);
					LogHistory.Enqueue(value);
					History.PushBack(value);
					if (value.Color == Color.Red)
					{
						Core.Logger.Error(value.Msg ?? "");
					}
					else
					{
						Core.Logger.Information(value.Msg ?? "");
					}
				}
				Messages.Remove(key);
				if (LogHistory.Count >= 1024)
				{
					for (int j = 0; j < 24; j++)
					{
						LogHistory.Dequeue();
					}
				}
			}
		}
		catch (Exception ex)
		{
			LogError(ex.ToString());
		}
	}

	public static void LogMsg(string msg)
	{
		LogMsg(msg, 1f, Color.White);
	}

	public static void LogError(string msg, float time = 2f)
	{
		LogMsg(msg, time, Color.Red);
	}

	public static void LogMsg(string msg, float time)
	{
		LogMsg(msg, time, Color.White);
	}

	public static void LogMsg(string msg, float time, Color color)
	{
		try
		{
			if (Messages.TryGetValue(msg, out var value))
			{
				value.Time = DateTime.UtcNow.AddSeconds(time);
				value.Color = color;
				value.Count++;
				return;
			}
			value = new DebugMsgDescription
			{
				Msg = msg,
				Time = DateTime.UtcNow.AddSeconds(time),
				ColorV4 = color.ToImguiVec4(),
				Color = color,
				Count = 1
			};
			lock (locker)
			{
				Messages[msg] = value;
				MessagesList.Add(value);
			}
		}
		catch (Exception value2)
		{
			Core.Logger.Error($"{"DebugWindow"} -> {value2}");
		}
	}
}
