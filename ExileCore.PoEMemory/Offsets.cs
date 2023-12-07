using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Interfaces;

namespace ExileCore.PoEMemory;

public class Offsets
{
	public static Offsets Regular = new Offsets
	{
		IgsOffset = 0,
		IgsDelta = 0,
		ExeName = "PathOfExile"
	};

	public static Offsets Korean = new Offsets
	{
		IgsOffset = 0,
		IgsDelta = 0,
		ExeName = "Pathofexile_KG"
	};

	public static Offsets Steam = new Offsets
	{
		IgsOffset = 40,
		IgsDelta = 0,
		ExeName = "PathOfExileSteam"
	};

	private static readonly Pattern basePtrPattern = new Pattern(new byte[33]
	{
		144, 72, 3, 216, 72, 139, 3, 72, 133, 192,
		117, 0, 72, 139, 29, 0, 0, 0, 0, 72,
		139, 5, 0, 0, 0, 0, 72, 133, 192, 116,
		0, 102, 144
	}, "xxxxxxxxxxx?xxx????xxx????xxxx?xx", "BasePtr");

	private static readonly Pattern fileRootPattern = new Pattern(new byte[13]
	{
		72, 139, 8, 72, 141, 61, 0, 0, 0, 0,
		139, 4, 14
	}, "xxxxxx????xxx", "File Root", 19726336);

	private static readonly Pattern areaChangePattern = new Pattern(new byte[23]
	{
		255, 0, 0, 0, 0, 0, 232, 0, 0, 0,
		0, 255, 5, 0, 0, 0, 0, 73, 141, 86,
		64, 72, 139
	}, "x?????x????xx????xxxxxx", "Area change", 6881280);

	private static readonly Pattern isLoadingScreenPattern = new Pattern(new byte[18]
	{
		72, 137, 5, 0, 0, 0, 0, 72, 139, 0,
		0, 0, 72, 137, 0, 72, 139, 198
	}, "xxx????xx???xx?xxx", "Loading");

	private static readonly Pattern GameStatePattern = new Pattern(new byte[23]
	{
		72, 131, 236, 32, 72, 139, 241, 51, 237, 72,
		57, 45, 0, 0, 0, 0, 15, 133, 0, 0,
		0, 0, 65
	}, "xxx?xxxxxxxx????xx????x", "Game State");

	private static readonly Pattern DiagnosticInfoTypePattern = new Pattern(new byte[11]
	{
		69, 51, 0, 68, 137, 0, 36, 64, 72, 139,
		5
	}, "xx?xx?xxxxx", "DiagnosticInfoType", 600000);

	private static readonly StringPattern BlackBarSizePattern = new StringPattern("2B ?? ^ ?? ?? ?? ?? ?? 0F 57 ?? ?? 8B ?? ?? ?? ?? ?? F3 ?? 0F 10", "BlackBarSize");

	private static readonly StringPattern TerrainRotationSelectorPattern = new StringPattern("?? 8D ?? ^ ?? ?? ?? ?? ?? 0F B6 ?? ?? ?? ?? ?? ?? ?? 8B ?? ?? 3B ?? 89 ?? ?? ?? ?? ?? ?? ?? ?? ?? 0F 47 ?? ?? 8D ?? ?? ?? ?? ??", "Terrain Rotation Selector");

	private static readonly StringPattern TerrainRotationHelperPattern = new StringPattern("?? 8D ?? ?? ?? ?? ?? ?? 0F B6 ?? ?? ?? ?? ?? ?? ?? 8B ?? ?? 3B ?? 89 ?? ?? ?? ?? ?? ?? ?? ?? ?? 0F 47 ?? ?? 8D ?? ^ ?? ?? ?? ??", "Terrain Rotator Helper");

	public long AreaChangeCount { get; private set; }

	public long Base { get; private set; }

	public string ExeName { get; private set; }

	public long FileRoot { get; private set; }

	public int IgsDelta { get; private set; }

	public int IgsOffset { get; private set; }

	public int IgsOffsetDelta => IgsOffset + IgsDelta;

	public long isLoadingScreenOffset { get; private set; }

	public long GameStateOffset { get; private set; }

	public long DiagnosticInfoTypeOffset { get; private set; }

	public Dictionary<OffsetsName, long> DoPatternScans(IMemory m)
	{
		IPattern[] array = new IPattern[7] { fileRootPattern, areaChangePattern, GameStatePattern, DiagnosticInfoTypePattern, BlackBarSizePattern, TerrainRotationSelectorPattern, TerrainRotationHelperPattern };
		Dictionary<IPattern, long> patternAddresses = m.FindPatterns(array).Zip(array).ToDictionary(((long First, IPattern Second) x) => x.Second, ((long First, IPattern Second) x) => x.First);
		Dictionary<Pattern, int> patternOffsets = new Dictionary<Pattern, int>
		{
			[fileRootPattern] = 6,
			[areaChangePattern] = 13,
			[GameStatePattern] = 12,
			[DiagnosticInfoTypePattern] = 11
		};
		Dictionary<OffsetsName, long> result = new Dictionary<OffsetsName, long>();
		long baseAddress = m.Process.MainModule.BaseAddress.ToInt64();
		FileRoot = ReadRelativeAddress(fileRootPattern, OffsetsName.FileRoot);
		AreaChangeCount = ReadRelativeAddress(areaChangePattern, OffsetsName.AreaChangeCount);
		GameStateOffset = ReadRelativeAddress(GameStatePattern, OffsetsName.GameStateOffset);
		DiagnosticInfoTypeOffset = ReadRelativeAddress(DiagnosticInfoTypePattern, OffsetsName.DiagnosticInfoTypeOffset);
		ReadRelativeAddress(BlackBarSizePattern, OffsetsName.BlackBarSize);
		ReadRelativeAddress(TerrainRotationSelectorPattern, OffsetsName.TerrainRotationSelector);
		ReadRelativeAddress(TerrainRotationHelperPattern, OffsetsName.TerrainRotationHelper);
		return result;
		long ReadRelativeAddress(IPattern pattern, OffsetsName offsetName)
		{
			long num = patternAddresses[pattern];
			int num2 = default(int);
			if (!(pattern is StringPattern stringPattern))
			{
				if (!(pattern is Pattern pattern2))
				{
					if (pattern != null)
					{
						throw new Exception($"Unknown pattern type {pattern.GetType()} (name: {pattern.Name}");
					}
					global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(pattern);
				}
				else
				{
					if (!patternOffsets.TryGetValue(pattern2, out var value))
					{
						throw new Exception("Pattern " + pattern2.Name + " has no specified offset");
					}
					num2 = value;
				}
			}
			else
			{
				num2 = stringPattern.PatternOffset;
			}
			int num3 = num2;
			long num4 = m.Read<int>(baseAddress + num + num3) + num + num3 + 4;
			result[offsetName] = num4;
			return num4;
		}
	}
}
