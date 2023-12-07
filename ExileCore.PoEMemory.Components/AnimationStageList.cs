using System.Collections.Generic;
using System.Linq;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.Components;

public class AnimationStageList : RemoteMemoryObject
{
	private List<AnimationStage> _stages;

	private NativePtrArray StageList => base.M.Read<NativePtrArray>(base.Address);

	public List<AnimationStage> AllStages => _stages ?? (_stages = base.M.ReadStdVector<long>(StageList).Select(base.GetObject<AnimationStage>).ToList());
}
