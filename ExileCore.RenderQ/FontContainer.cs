using ImGuiNET;

namespace ExileCore.RenderQ;

public readonly struct FontContainer
{
	public unsafe readonly ImFont* Atlas;

	public readonly string Name;

	public readonly int Size;

	public unsafe FontContainer(ImFont* atlas, string name, int size)
	{
		Atlas = atlas;
		Name = name;
		Size = size;
	}
}
