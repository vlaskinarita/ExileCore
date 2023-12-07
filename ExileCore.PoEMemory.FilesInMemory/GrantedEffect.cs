using System.Collections.Generic;
using System.Linq;

namespace ExileCore.PoEMemory.FilesInMemory;

public class GrantedEffect : RemoteMemoryObject
{
	private string _name;

	private bool? _isSupport;

	private string _supportGemLetter;

	private bool? _supportsGemsOnly;

	private bool? _cannotBeSupported;

	private int? _castTime;

	private List<GrantedEffectPerLevel> _perLevelEffects;

	public string Name => _name ?? (_name = base.M.ReadStringU(base.M.Read<long>(base.Address)));

	public bool IsSupport
	{
		get
		{
			bool valueOrDefault = _isSupport.GetValueOrDefault();
			if (!_isSupport.HasValue)
			{
				valueOrDefault = base.M.Read<bool>(base.Address + 8);
				_isSupport = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public string SupportGemLetter => _supportGemLetter ?? (_supportGemLetter = base.M.ReadStringU(base.M.Read<long>(base.Address + 25)));

	public bool SupportsGemsOnly
	{
		get
		{
			bool valueOrDefault = _supportsGemsOnly.GetValueOrDefault();
			if (!_supportsGemsOnly.HasValue)
			{
				valueOrDefault = base.M.Read<bool>(base.Address + 69);
				_supportsGemsOnly = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public bool CannotBeSupported
	{
		get
		{
			bool valueOrDefault = _cannotBeSupported.GetValueOrDefault();
			if (!_cannotBeSupported.HasValue)
			{
				valueOrDefault = base.M.Read<bool>(base.Address + 90);
				_cannotBeSupported = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public int CastTime
	{
		get
		{
			int valueOrDefault = _castTime.GetValueOrDefault();
			if (!_castTime.HasValue)
			{
				valueOrDefault = base.M.Read<int>(base.Address + 95);
				_castTime = valueOrDefault;
				return valueOrDefault;
			}
			return valueOrDefault;
		}
	}

	public List<GrantedEffectPerLevel> PerLevelEffects => _perLevelEffects ?? (_perLevelEffects = base.TheGame.Files.GrantedEffectsPerLevel.EntriesList.Where((GrantedEffectPerLevel x) => Equals(x.GrantedEffect)).ToList());
}
