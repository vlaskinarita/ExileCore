using ExileCore.PoEMemory.MemoryObjects.Heist;
using ExileCore.PoEMemory.Models;
using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class HeistContract : Component
{
	private readonly CachedValue<HeistContractComponentOffsets> _ContractData;

	private readonly CachedValue<HeistContractObjectiveOffsets> _ObjectivesData;

	private readonly CachedValue<HeistContractRequirementOffsets> _RequirementData;

	private HeistContractObjectiveOffsets Objectives => _ObjectivesData.Value;

	private HeistContractRequirementOffsets Requirements => _RequirementData.Value;

	public BaseItemType TargetItem => base.TheGame.Files.BaseItemTypes.GetFromAddress(Objectives.TargetKey);

	public string Client => base.M.ReadStringU(Objectives.ClientKey);

	public HeistJobRecord RequiredJob => base.TheGame.Files.HeistJobs.GetByAddress(Requirements.JobKey);

	public byte RequiredJobLevel => Requirements.JobLevel;

	public HeistContract()
	{
		_ContractData = new FrameCache<HeistContractComponentOffsets>(() => base.M.Read<HeistContractComponentOffsets>(base.Address));
		_ObjectivesData = new FrameCache<HeistContractObjectiveOffsets>(() => base.M.Read<HeistContractObjectiveOffsets>(_ContractData.Value.ObjectiveKey));
		_RequirementData = new FrameCache<HeistContractRequirementOffsets>(() => base.M.Read<HeistContractRequirementOffsets>(_ContractData.Value.Requirements.First));
	}
}
