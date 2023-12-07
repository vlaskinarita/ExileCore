using System.Text.RegularExpressions;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory.Components;

public class HarvestInfrastructureMod
{
	public int ModLevel { get; }

	public string ModName { get; }

	internal HarvestInfrastructureMod(HarvestInfrastructureModUnmanaged data, IMemory m)
	{
		ModLevel = data.ModLevel;
		long addr = m.Read<long>(data.DatEntryPtr + 8);
		string input = m.ReadStringU(addr, 1000);
		ModName = Regex.Replace(input, "\\<(.*?)\\>|\\{|\\}", string.Empty);
	}

	public override string ToString()
	{
		return $"{ModName}. ({ModLevel})";
	}
}
