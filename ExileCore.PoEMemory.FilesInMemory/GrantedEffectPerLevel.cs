namespace ExileCore.PoEMemory.FilesInMemory;

public class GrantedEffectPerLevel : RemoteMemoryObject
{
	private GrantedEffect _grantedEffect;

	private int? _level;

	private int? _requiredLevel;

	private int? _costMultiplier;

	private int? _cooldown;

	public GrantedEffect GrantedEffect => _grantedEffect ?? (_grantedEffect = base.TheGame.Files.GrantedEffects.GetByAddress(base.M.Read<long>(base.Address)));

	public int Level
	{
		get
		{
			int valueOrDefault = _level.GetValueOrDefault();
			if (!_level.HasValue)
			{
				valueOrDefault = base.M.Read<int>(base.Address + 16);
				_level = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public int RequiredLevel
	{
		get
		{
			int valueOrDefault = _requiredLevel.GetValueOrDefault();
			if (!_requiredLevel.HasValue)
			{
				valueOrDefault = base.M.Read<int>(base.Address + 20);
				_requiredLevel = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public int CostMultiplier
	{
		get
		{
			int valueOrDefault = _costMultiplier.GetValueOrDefault();
			if (!_costMultiplier.HasValue)
			{
				valueOrDefault = base.M.Read<int>(base.Address + 24);
				_costMultiplier = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public int Cooldown
	{
		get
		{
			int valueOrDefault = _cooldown.GetValueOrDefault();
			if (!_cooldown.HasValue)
			{
				valueOrDefault = base.M.Read<int>(base.Address + 32);
				_cooldown = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}
}
