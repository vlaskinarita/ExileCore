using System;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class ActiveAnimationData : StructuredRemoteMemoryObject<ActiveAnimationOffsets>
{
	public int AnimationId => base.Structure.AnimationId;

	public float SlowAnimationSpeed => base.Structure.SlowAnimationSpeed;

	public float NormalAnimationSpeed => base.Structure.NormalAnimationSpeed;

	public float? AnimationSpeed
	{
		get
		{
			if (base.Structure.SlowAnimationStartStagePtr != 0L)
			{
				float normalAnimationSpeed = NormalAnimationSpeed;
				if (normalAnimationSpeed != 0f)
				{
					return normalAnimationSpeed;
				}
			}
			return null;
		}
	}

	public AnimationStage SlowAnimationStartStage => GetObject<AnimationStage>(base.Structure.SlowAnimationStartStagePtr);

	public AnimationStage SlowAnimationEndStage => GetObject<AnimationStage>(base.Structure.SlowAnimationEndStagePtr);

	public Func<float, float> TransformRawProgressFunc
	{
		get
		{
			float num = SlowAnimationSpeed / NormalAnimationSpeed;
			if ((double)Math.Abs(num - 1f) < 0.001)
			{
				return (float f) => f;
			}
			float slowdownStart = SlowAnimationStartStage.StageStart;
			float stageStart = SlowAnimationEndStage.StageStart;
			float slowdownDuration = stageStart - slowdownStart;
			float totalDiff = slowdownDuration * (1f - num) / num;
			return (float progress) => progress + totalDiff * Math.Clamp((progress - slowdownStart) / slowdownDuration, 0f, 1f);
		}
	}
}
