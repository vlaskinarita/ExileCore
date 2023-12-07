using System.Numerics;
using ImGuiNET;

namespace ExileCore;

internal class ImGuiHelpers
{
	public unsafe static bool SetDragDropPayload<T>(string id, T payload) where T : unmanaged
	{
		return ImGui.SetDragDropPayload(id, (nint)(&payload), (uint)sizeof(T));
	}

	public unsafe static T? AcceptDragDropPayload<T>(string id) where T : unmanaged
	{
		ImGuiPayloadPtr imGuiPayloadPtr = ImGui.AcceptDragDropPayload(id);
		if (imGuiPayloadPtr.NativePtr != null)
		{
			return *(T*)imGuiPayloadPtr.Data;
		}
		return null;
	}

	public static void DrawAllColumnsBox(string id, Vector2 start)
	{
		Vector2 cursorPos = ImGui.GetCursorPos();
		ImGui.SetCursorPos(start);
		ImGui.PushStyleColor(ImGuiCol.HeaderHovered, 0u);
		ImGui.PushStyleColor(ImGuiCol.HeaderActive, 0u);
		ImGui.Selectable(id, selected: false, ImGuiSelectableFlags.SpanAllColumns, new Vector2(0f, cursorPos.Y - start.Y));
		ImGui.PopStyleColor(2);
		ImGui.SetCursorPos(cursorPos);
	}
}
