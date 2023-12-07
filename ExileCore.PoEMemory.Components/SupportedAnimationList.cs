using System.Collections.Generic;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class SupportedAnimationList : StructuredRemoteMemoryObject<ActorAnimationListOffsets>
{
	private List<AnimationStageList> _animations;

	public List<AnimationStageList> Animations => _animations ?? (_animations = base.M.ReadStructsArray<AnimationStageList>(base.Structure.AnimationList.First, base.Structure.AnimationList.Last, 24, null));
}
