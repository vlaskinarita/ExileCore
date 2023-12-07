using System;
using System.Numerics;
using ExileCore.PoEMemory.Elements;
using ExileCore.Shared.Cache;
using ExileCore.Shared.Helpers;
using GameOffsets;
using GameOffsets.Native;

namespace ExileCore.PoEMemory.MemoryObjects;

public class IngameState : RemoteMemoryObject
{
	private readonly CachedValue<Camera> _camera;

	private readonly CachedValue<Vector2> _CurrentUIElementPos;

	private readonly CachedValue<Vector2i> _MousePos;

	private readonly CachedValue<EntityLabelMapOffsets> _EntityLabelMap;

	private readonly CachedValue<IngameData> _ingameData;

	private readonly CachedValue<IngameStateOffsets> _ingameState;

	private readonly CachedValue<IngameUIElements> _ingameUi;

	private readonly CachedValue<float> _TimeInGameF;

	private readonly CachedValue<Element> _UIHover;

	private readonly CachedValue<Element> _UIHoverElement;

	private readonly CachedValue<Vector2> _UIHoverPos;

	private readonly CachedValue<Element> _UIRoot;

	private static readonly int WorldDataOffset = Extensions.GetOffset((IngameStateOffsets x) => x.WorldData);

	private static readonly int CameraOffset = Extensions.GetOffset((WorldDataOffsets x) => x.Camera);

	public Camera Camera => _camera.Value;

	public IngameData Data => _ingameData.Value;

	public bool InGame => ServerData.IsInGame;

	public ServerData ServerData => _ingameData.Value.ServerData;

	public IngameUIElements IngameUi => _ingameUi.Value;

	public Element UIRoot => _UIRoot.Value;

	public ShortcutSettings ShortcutSettings => UIRoot?.GetChildAtIndex(0).AsObject<ShortcutSettings>();

	public Element UIHover => _UIHover.Value;

	public float UIHoverX => _UIHoverPos.Value.X;

	public float UIHoverY => _UIHoverPos.Value.Y;

	public Element UIHoverElement => _UIHoverElement.Value;

	public Element UIHoverTooltip => UIHoverElement;

	public float CurentUElementPosX => _CurrentUIElementPos.Value.X;

	public float CurentUElementPosY => _CurrentUIElementPos.Value.Y;

	public int MousePosX => _MousePos.Value.X;

	public int MousePosY => _MousePos.Value.Y;

	public long EntityLabelMap => _EntityLabelMap.Value.EntityLabelMap;

	public TimeSpan TimeInGame => TimeSpan.FromSeconds(_ingameState.Value.TimeInGameF);

	public float TimeInGameF => _TimeInGameF.Value;

	public IngameState()
	{
		_ingameState = new FrameCache<IngameStateOffsets>(() => base.M.Read<IngameStateOffsets>(base.Address));
		_camera = new AreaCache<Camera>(() => GetObject<Camera>(base.M.Read<long>(base.Address + WorldDataOffset) + CameraOffset));
		_ingameData = new AreaCache<IngameData>(() => GetObject<IngameData>(_ingameState.Value.Data));
		_ingameUi = new AreaCache<IngameUIElements>(() => GetObject<IngameUIElements>(_ingameState.Value.IngameUi));
		_UIRoot = new AreaCache<Element>(() => GetObject<Element>(_ingameState.Value.UIRoot));
		_UIHover = new FrameCache<Element>(() => GetObject<Element>(_ingameState.Value.UIHover));
		_UIHoverElement = new FrameCache<Element>(() => GetObject<Element>(_ingameState.Value.UIHoverElement));
		_UIHoverPos = new FrameCache<Vector2>(() => _ingameState.Value.UIHoverPos);
		_CurrentUIElementPos = new FrameCache<Vector2>(() => _ingameState.Value.CurentUIElementPos);
		_MousePos = new FrameCache<Vector2i>(() => _ingameState.Value.MouseGlobal);
		_TimeInGameF = new FrameCache<float>(() => _ingameState.Value.TimeInGameF);
		_EntityLabelMap = new AreaCache<EntityLabelMapOffsets>(() => base.M.Read<EntityLabelMapOffsets>(_ingameState.Value.EntityLabelMap));
	}

	public void UpdateData()
	{
		_ingameData.ForceUpdate();
	}
}
