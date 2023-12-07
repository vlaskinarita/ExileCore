using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ExileCore.Shared.Attributes;
using ExileCore.Shared.Nodes;
using ImGuiNET;

namespace ExileCore;

[Submenu]
public class CorePluginSettings
{
	[Submenu(RenderMethod = "Render", CollapsedByDefault = true, EnableSelfDrawCollapsing = true)]
	public class PluginFolderSettings
	{
		public class PluginFolder
		{
			public string Name;

			public bool CollapsedByDefault;

			public Guid Id { get; init; } = Guid.NewGuid();

		}

		public List<PluginFolder> PluginFolders = new List<PluginFolder>();

		public Dictionary<string, Guid?> PluginFolderMapping = new Dictionary<string, Guid?>();

		public void Render()
		{
			foreach (var (pluginFolder, num) in PluginFolders.Select((PluginFolder x, int i) => (x, i)).ToList())
			{
				ImGui.PushID(pluginFolder.Id.ToString());
				Vector2 cursorPos = ImGui.GetCursorPos();
				ImGui.PushStyleColor(ImGuiCol.Button, 0u);
				ImGui.Button("=");
				ImGui.PopStyleColor();
				ImGui.SameLine();
				if (ImGui.BeginDragDropSource())
				{
					ImGuiHelpers.SetDragDropPayload("FolderIndex", num);
					ImGui.Text(pluginFolder.Name);
					ImGui.EndDragDropSource();
				}
				else if (ImGui.IsItemHovered())
				{
					ImGui.SetTooltip("Drag me");
				}
				if (ImGui.Button("Delete"))
				{
					if (ImGui.IsKeyDown(ImGuiKey.ModShift))
					{
						PluginFolders.RemoveAt(num);
						ImGui.PopID();
						break;
					}
				}
				else if (ImGui.IsItemHovered())
				{
					ImGui.SetTooltip("Hold Shift");
				}
				ImGui.SameLine();
				ImGui.InputText("Name", ref pluginFolder.Name, 200u);
				ImGui.SameLine();
				ImGui.Checkbox("Collapse by default", ref pluginFolder.CollapsedByDefault);
				ImGuiHelpers.DrawAllColumnsBox("##DragTarget", cursorPos);
				if (ImGui.BeginDragDropTarget())
				{
					int? num2 = ImGuiHelpers.AcceptDragDropPayload<int>("FolderIndex");
					if (num2.HasValue && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
					{
						PluginFolder item = PluginFolders[num2.Value];
						PluginFolders.RemoveAt(num2.Value);
						PluginFolders.Insert(num, item);
					}
					ImGui.EndDragDropTarget();
				}
				ImGui.PopID();
			}
			if (ImGui.Button("Add folder"))
			{
				PluginFolders.Add(new PluginFolder
				{
					Name = "",
					CollapsedByDefault = false
				});
			}
		}
	}

	[Menu("Load source plugins in parallel", "Requires restart to apply. When you use a lot of plugins this option can improve hud load time.")]
	public ToggleNode MultiThreadLoadPlugins { get; set; } = new ToggleNode(value: false);


	[Menu("Avoid locking plugin dlls", "Requires restart to apply. Only enable this if you need to do live dll replacement without restarting the HUD.")]
	public ToggleNode AvoidLockingDllFiles { get; set; } = new ToggleNode(value: false);


	[Menu(null, "Requires restart to apply. Load plugins from source even if there is a compiled plugin with the same name")]
	public ToggleNode PreferSourcePlugins { get; set; } = new ToggleNode(value: false);


	public PluginFolderSettings FolderSettings { get; set; } = new PluginFolderSettings();

}
