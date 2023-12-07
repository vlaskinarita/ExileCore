using ExileCore.Shared.Helpers;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class AnimationStage : StructuredRemoteMemoryObject<ActorAnimationStageOffsets>
{
	private string _stageName;

	private int? _actorAnimationListIndex;

	private float? _stageStart;

	public float StageStart
	{
		get
		{
			float valueOrDefault = _stageStart.GetValueOrDefault();
			if (!_stageStart.HasValue)
			{
				valueOrDefault = base.Structure.StageStart;
				_stageStart = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public int ActorAnimationListIndex
	{
		get
		{
			int valueOrDefault = _actorAnimationListIndex.GetValueOrDefault();
			if (!_actorAnimationListIndex.HasValue)
			{
				valueOrDefault = base.Structure.ActorAnimationListIndex;
				_actorAnimationListIndex = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public string StageName => _stageName ?? (_stageName = base.Structure.StageName.ToString(base.M));
}
