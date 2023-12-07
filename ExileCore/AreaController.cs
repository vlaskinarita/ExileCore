using System;
using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore;

public class AreaController
{
	public TheGame TheGameState { get; }

	public AreaInstance CurrentArea { get; private set; }

	public event Action<AreaInstance> OnAreaChange;

	public AreaController(TheGame theGameState)
	{
		TheGameState = theGameState;
	}

	public void ForceRefreshArea(bool areaChangeMultiThread)
	{
		DebugWindow.LogMsg("Force area refresh triggered");
		IngameData data = TheGameState.IngameState.Data;
		AreaTemplate currentArea = data.CurrentArea;
		uint currentAreaHash = TheGameState.CurrentAreaHash;
		CurrentArea = new AreaInstance(currentArea, currentAreaHash, data.CurrentAreaLevel);
		AreaInstance.ForceRefreshCounter++;
		if (CurrentArea.Name.Length != 0)
		{
			ActionAreaChange();
		}
	}

	public bool RefreshState()
	{
		IngameData data = TheGameState.IngameState.Data;
		AreaTemplate currentArea = data.CurrentArea;
		uint currentAreaHash = TheGameState.CurrentAreaHash;
		if (CurrentArea != null && currentAreaHash == CurrentArea.Hash)
		{
			return false;
		}
		CurrentArea = new AreaInstance(currentArea, currentAreaHash, data.CurrentAreaLevel);
		if (CurrentArea.Name.Length == 0)
		{
			return false;
		}
		DebugWindow.LogMsg("Area refresh triggered");
		ActionAreaChange();
		return true;
	}

	private void ActionAreaChange()
	{
		this.OnAreaChange?.Invoke(CurrentArea);
	}
}
