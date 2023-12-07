using System.Linq;
using ExileCore.PoEMemory.FilesInMemory;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using GameOffsets;

namespace ExileCore.PoEMemory.Components;

public class SkillGem : Component
{
	private readonly CachedValue<SkillGemOffsets> _cachedValue;

	private readonly FrameCache<GemInformation> _cachedValue2;

	public int Level => (int)_cachedValue.Value.Level;

	public uint TotalExpGained => _cachedValue.Value.TotalExpGained;

	public uint ExperiencePrevLevel => _cachedValue.Value.TotalExpGained;

	public uint ExperienceMaxLevel => _cachedValue.Value.ExperienceMaxLevel;

	public uint ExperienceToNextLevel => ExperienceMaxLevel - ExperiencePrevLevel;

	public int MaxLevel => _cachedValue2.Value.MaxLevel;

	public int SocketColor => _cachedValue2.Value.SocketColor;

	public SkillGemQualityTypeE QualityType => (SkillGemQualityTypeE)_cachedValue.Value.QualityType;

	public GrantedEffect GrantedEffect1 => base.TheGame.Files.GrantedEffects.GetByAddress(_cachedValue2.Value.GrantedEffect1);

	public GrantedEffect GrantedEffect2 => base.TheGame.Files.GrantedEffects.GetByAddress(_cachedValue2.Value.GrantedEffect2);

	public GrantedEffect GrantedEffect1HardMode => base.TheGame.Files.GrantedEffects.GetByAddress(_cachedValue2.Value.GrantedEffect1HardMode);

	public GrantedEffect GrantedEffect2HardMode => base.TheGame.Files.GrantedEffects.GetByAddress(_cachedValue2.Value.GrantedEffect2HardMode);

	public int RequiredLevel => GetRequiredLevel(Level);

	public SkillGem()
	{
		_cachedValue = new FrameCache<SkillGemOffsets>(() => base.M.Read<SkillGemOffsets>(base.Address));
		_cachedValue2 = new FrameCache<GemInformation>(() => base.M.Read<GemInformation>(_cachedValue.Value.AdvanceInformation));
	}

	public int GetRequiredLevel(int gemLevel)
	{
		return GrantedEffect1.PerLevelEffects.FirstOrDefault((GrantedEffectPerLevel x) => x.Level == gemLevel)?.RequiredLevel ?? 0;
	}
}
