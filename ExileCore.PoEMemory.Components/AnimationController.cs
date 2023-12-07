using System;
using System.Linq;
using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class AnimationController : Component
{
	private readonly CachedValue<Func<float, float>> _progressTransformFunc;

	private readonly CachedValue<float> _animationSpeed;

	private readonly CachedValue<SupportedAnimationList> _supportedAnimationList;

	private readonly CachedValue<AnimationControllerOffsets> _cachedValue;

	public AnimationControllerOffsets Structure => _cachedValue.Value;

	private ActiveAnimationData ActiveAnimationData => base.M.ReadStdVector<long>(Structure.ActiveAnimationsArrayPtr).Select(base.ReadObject<ActiveAnimationData>).LastOrDefault((ActiveAnimationData x) => x.AnimationId == CurrentAnimationId);

	public float MaxRawAnimationProgress => Structure.MaxAnimationProgress - Structure.MaxAnimationProgressOffset;

	public float RawNextAnimationPoint => Structure.NextAnimationPoint;

	public float RawAnimationProgress => Structure.AnimationProgress;

	public float RawAnimationSpeed => Structure.AnimationSpeedMultiplier1 * Structure.AnimationSpeedMultiplier2;

	public float TransformedMaxRawAnimationProgress => TransformProgress(MaxRawAnimationProgress);

	public float TransformedRawNextAnimationPoint => TransformProgress(RawNextAnimationPoint);

	public float TransformedRawAnimationProgress => TransformProgress(RawAnimationProgress);

	public float AnimationSpeed => _animationSpeed.Value;

	public SupportedAnimationList SupportedAnimationList => _supportedAnimationList.Value;

	public int CurrentAnimationId => Structure.AnimationInActorId;

	public int CurrentAnimationStage => Structure.CurrentAnimationStage;

	public AnimationStageList CurrentAnimation
	{
		get
		{
			if (CurrentAnimationId < 0 || CurrentAnimationId >= SupportedAnimationList.Animations.Count)
			{
				throw new ArgumentOutOfRangeException("CurrentAnimationId", CurrentAnimationId, $"There's only {SupportedAnimationList.Animations.Count} animations");
			}
			return SupportedAnimationList.Animations[CurrentAnimationId];
		}
	}

	public float NextAnimationPoint => TransformedRawNextAnimationPoint / TransformedMaxRawAnimationProgress;

	public float AnimationProgress => TransformedRawAnimationProgress / TransformedMaxRawAnimationProgress;

	public TimeSpan AnimationCompletesIn
	{
		get
		{
			float num = (TransformedMaxRawAnimationProgress - TransformedRawAnimationProgress) / AnimationSpeed;
			return TimeSpan.FromSeconds((!float.IsNaN(num) && !float.IsInfinity(num)) ? num : 1f);
		}
	}

	public TimeSpan AnimationActiveFor
	{
		get
		{
			float num = TransformedRawAnimationProgress / AnimationSpeed;
			return TimeSpan.FromSeconds((!float.IsNaN(num) && !float.IsInfinity(num)) ? num : 0f);
		}
	}

	public AnimationController()
	{
		_cachedValue = CreateStructFrameCache<AnimationControllerOffsets>();
		_progressTransformFunc = KeyTrackingCache.Create(() => ActiveAnimationData?.TransformRawProgressFunc ?? ((Func<float, float>)((float f) => f)), () => (CurrentAnimationId, CurrentAnimationStage));
		_animationSpeed = KeyTrackingCache.Create(() => ActiveAnimationData?.AnimationSpeed ?? RawAnimationSpeed, () => (CurrentAnimationId, CurrentAnimationStage));
		_supportedAnimationList = new TimeCache<SupportedAnimationList>(() => GetObject<SupportedAnimationList>(Structure.ActorAnimationArrayPtr), 1000L);
	}

	public float TransformProgress(float progress)
	{
		return _progressTransformFunc.Value(progress);
	}
}
