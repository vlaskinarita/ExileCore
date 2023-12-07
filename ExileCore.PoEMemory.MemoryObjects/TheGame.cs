using System;
using System.Collections.Generic;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Interfaces;
using GameOffsets;
using GameOffsets.Objects;

namespace ExileCore.PoEMemory.MemoryObjects;

public class TheGame : RemoteMemoryObject
{
	private class GameStateHashNode : RemoteMemoryObject
	{
		public GameStateHashNode Previous => ReadObject<GameStateHashNode>(base.Address);

		public GameStateHashNode Root => ReadObject<GameStateHashNode>(base.Address + 8);

		public GameStateHashNode Next => ReadObject<GameStateHashNode>(base.Address + 16);

		public bool IsNull => base.M.Read<byte>(base.Address + 25) != 0;

		public string Key => base.M.ReadNativeString(base.Address + 32);

		public GameState Value1 => ReadObject<GameState>(base.Address + 64);
	}

	private static readonly int DataOffset = Extensions.GetOffset((IngameStateOffsets x) => x.Data);

	private static readonly int CurrentAreaHashOffset = Extensions.GetOffset((IngameDataOffsets x) => x.CurrentAreaHash);

	private static long PreGameStatePtr = -1L;

	private static long LoginStatePtr = -1L;

	private static long SelectCharacterStatePtr = -1L;

	private static long WaitingStatePtr = -1L;

	private static long InGameStatePtr = -1L;

	private static long LoadingStatePtr = -1L;

	private static long EscapeStatePtr = -1L;

	private static CachedValue<DiagnosticInfoType> _DiagnosticInfoType;

	private static TheGame Instance;

	private readonly CachedValue<int> _AreaChangeCount;

	private readonly CachedValue<bool> _inGame;

	private readonly CachedValue<int> _blackBarSize;

	private readonly CachedValue<byte[]> _rotationHelper;

	private readonly CachedValue<byte[]> _rotationSelector;

	public readonly Dictionary<GameStateTypes, long> AllGameStates;

	public FilesContainer Files { get; set; }

	public AreaLoadingState LoadingState { get; }

	public EscapeState EscapeState { get; }

	public IngameState IngameState { get; }

	public IList<GameState> CurrentGameStates => base.M.ReadDoublePtrVectorClasses<GameState>(base.Address + 8, IngameState);

	public IList<GameState> ActiveGameStates => base.M.ReadDoublePtrVectorClasses<GameState>(base.Address + 32, IngameState, noNullPointers: true);

	public bool IsPreGame => GameStateActive(PreGameStatePtr);

	public bool IsLoginState => GameStateActive(LoginStatePtr);

	public bool IsSelectCharacterState => GameStateActive(SelectCharacterStatePtr);

	public bool IsWaitingState => GameStateActive(WaitingStatePtr);

	public bool IsInGameState => GameStateActive(InGameStatePtr);

	public bool IsLoadingState => GameStateActive(LoadingStatePtr);

	public bool IsEscapeState
	{
		get
		{
			if (GameStateActive(EscapeStatePtr))
			{
				return EscapeState.IsActive;
			}
			return false;
		}
	}

	public bool IsLoading => LoadingState.IsLoading;

	public int AreaChangeCount => _AreaChangeCount.Value;

	public bool InGame => _inGame.Value;

	public DiagnosticInfoType DiagnosticInfoType => _DiagnosticInfoType.Value;

	public int BlackBarSize => _blackBarSize.Value;

	public byte[] TerrainRotationSelector => _rotationSelector.Value;

	public byte[] TerrainRotationHelper => _rotationHelper.Value;

	public uint CurrentAreaHash => base.M.Read<uint>(IngameState.Address + DataOffset, new int[1] { CurrentAreaHashOffset });

	public TheGame(IMemory m, Cache cache, CoreSettings settings)
	{
		TheGame theGame = this;
		RemoteMemoryObject.pM = m;
		RemoteMemoryObject.pCache = cache;
		RemoteMemoryObject.pTheGame = this;
		Instance = this;
		base.Address = m.Read<long>(m.BaseOffsets[OffsetsName.GameStateOffset] + m.AddressOfProcess);
		_AreaChangeCount = new TimeCache<int>(() => theGame.M.Read<int>(theGame.M.AddressOfProcess + theGame.M.BaseOffsets[OffsetsName.AreaChangeCount]), 50L);
		_DiagnosticInfoType = new TimeCache<DiagnosticInfoType>(() => theGame.M.Read<DiagnosticInfoType>(theGame.M.AddressOfProcess + theGame.M.BaseOffsets[OffsetsName.DiagnosticInfoTypeOffset]), 5L);
		_blackBarSize = new TimeCache<int>(() => (!settings.DisableBlackBarAdjustment) ? theGame.M.Read<int>(theGame.M.AddressOfProcess + theGame.M.BaseOffsets[OffsetsName.BlackBarSize]) : 0, 250L);
		_rotationSelector = new StaticValueCache<byte[]>(() => theGame.M.ReadBytes(theGame.M.AddressOfProcess + theGame.M.BaseOffsets[OffsetsName.TerrainRotationSelector], 8));
		_rotationHelper = new StaticValueCache<byte[]>(() => theGame.M.ReadBytes(theGame.M.AddressOfProcess + theGame.M.BaseOffsets[OffsetsName.TerrainRotationHelper], 24));
		AllGameStates = ReadStates(base.Address);
		PreGameStatePtr = AllGameStates[GameStateTypes.PreGameState];
		LoginStatePtr = AllGameStates[GameStateTypes.LoginState];
		SelectCharacterStatePtr = AllGameStates[GameStateTypes.SelectCharacterState];
		WaitingStatePtr = AllGameStates[GameStateTypes.WaitingState];
		InGameStatePtr = AllGameStates[GameStateTypes.InGameState];
		LoadingStatePtr = AllGameStates[GameStateTypes.LoadingState];
		EscapeStatePtr = AllGameStates[GameStateTypes.EscapeState];
		LoadingState = GetObject<AreaLoadingState>(AllGameStates[GameStateTypes.AreaLoadingState]);
		IngameState = GetObject<IngameState>(AllGameStates[GameStateTypes.InGameState]);
		EscapeState = GetObject<EscapeState>(AllGameStates[GameStateTypes.EscapeState]);
		_inGame = new FrameCache<bool>(() => theGame.IngameState.Address != 0L && theGame.IngameState.Data.Address != 0L && theGame.IngameState.ServerData.Address != 0L && !theGame.IsLoading);
		Files = new FilesContainer(m);
	}

	public void Init()
	{
	}

	public void ReloadFiles()
	{
		Files = new FilesContainer(RemoteMemoryObject.pM);
	}

	private static bool GameStateActive(long stateAddress)
	{
		TheGame instance = Instance;
		if (instance == null)
		{
			return false;
		}
		IMemory m = instance.M;
		long num = Instance.Address + 32;
		long num2 = m.Read<long>(num);
		int num3 = (int)(m.Read<long>(num + 16) - num2);
		byte[] value = m.ReadMem(num2, num3);
		for (int i = 0; i < num3; i += 16)
		{
			long num4 = BitConverter.ToInt64(value, i);
			if (stateAddress == num4)
			{
				return true;
			}
		}
		return false;
	}

	private Dictionary<GameStateTypes, long> ReadStates(long pointer)
	{
		Dictionary<GameStateTypes, long> dictionary = new Dictionary<GameStateTypes, long>();
		GameStateOffsets gameStateOffsets = base.M.Read<GameStateOffsets>(pointer);
		dictionary[GameStateTypes.AreaLoadingState] = gameStateOffsets.State0;
		dictionary[GameStateTypes.WaitingState] = gameStateOffsets.State1;
		dictionary[GameStateTypes.CreditsState] = gameStateOffsets.State2;
		dictionary[GameStateTypes.EscapeState] = gameStateOffsets.State3;
		dictionary[GameStateTypes.InGameState] = gameStateOffsets.State4;
		dictionary[GameStateTypes.ChangePasswordState] = gameStateOffsets.State5;
		dictionary[GameStateTypes.LoginState] = gameStateOffsets.State6;
		dictionary[GameStateTypes.PreGameState] = gameStateOffsets.State7;
		dictionary[GameStateTypes.CreateCharacterState] = gameStateOffsets.State8;
		dictionary[GameStateTypes.SelectCharacterState] = gameStateOffsets.State9;
		dictionary[GameStateTypes.DeleteCharacterState] = gameStateOffsets.State10;
		dictionary[GameStateTypes.LoadingState] = gameStateOffsets.State11;
		return dictionary;
	}
}
