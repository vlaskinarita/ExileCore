using System;
using System.Collections.Generic;
using System.Numerics;
using ExileCore.Shared.Interfaces;
using ImGuiNET;

namespace ExileCore;

public class SettingsHolder : ISettingsHolder
{
	public string Name { get; set; } = "";


	public string Tooltip { get; set; }

	public string Unique => $"{Name}##{ID}";

	public int ID { get; set; } = -1;


	public Action DrawDelegate { get; set; }

	public IList<ISettingsHolder> Children { get; } = new List<ISettingsHolder>();


	public Func<bool> DisplayCondition { get; set; }

	public bool CollapsedByDefault { get; set; }

	public bool EnableSelfDrawCollapsing { get; set; }

	public SettingsHolder()
	{
		Tooltip = "";
	}

	public void Draw()
	{
		Func<bool> displayCondition = DisplayCondition;
		if (displayCondition != null && !displayCondition())
		{
			return;
		}
		if (Children.Count > 0 || EnableSelfDrawCollapsing)
		{
			ImGui.Spacing();
			Vector2 cursorScreenPos = ImGui.GetCursorScreenPos();
			ImGuiTreeNodeFlags flags = (CollapsedByDefault ? ImGuiTreeNodeFlags.AllowOverlap : (ImGuiTreeNodeFlags.AllowOverlap | ImGuiTreeNodeFlags.DefaultOpen));
			bool num = ImGui.TreeNodeEx(Unique + "treeNode", flags);
			string tooltip = Tooltip;
			if (tooltip != null && tooltip.Length > 0)
			{
				ImGui.SameLine();
				ImGui.TextDisabled("(?)");
				if (ImGui.IsItemHovered(ImGuiHoveredFlags.None))
				{
					ImGui.SetTooltip(Tooltip);
				}
			}
			if (num)
			{
				ImGui.Unindent();
				ImGui.Indent(10f);
				ImGui.Spacing();
				foreach (ISettingsHolder child in Children)
				{
					child.Draw();
				}
				if (EnableSelfDrawCollapsing)
				{
					DrawDelegate?.Invoke();
				}
				ImGui.Unindent(10f);
				Vector2 cursorScreenPos2 = ImGui.GetCursorScreenPos();
				ImGui.GetWindowDrawList().AddLine(cursorScreenPos, cursorScreenPos2, ImGui.GetColorU32(ImGuiCol.FrameBgActive));
				ImGui.Spacing();
				ImGui.Spacing();
				ImGui.Indent();
				ImGui.TreePop();
			}
			if (!EnableSelfDrawCollapsing)
			{
				DrawDelegate?.Invoke();
			}
			return;
		}
		DrawDelegate?.Invoke();
		string tooltip2 = Tooltip;
		if (tooltip2 != null && tooltip2.Length > 0)
		{
			ImGui.SameLine();
			ImGui.TextDisabled("(?)");
			if (ImGui.IsItemHovered(ImGuiHoveredFlags.None))
			{
				ImGui.SetTooltip(Tooltip);
			}
		}
	}
}
