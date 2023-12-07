using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using ExileCore.PoEMemory.Elements;
using ExileCore.PoEMemory.Elements.ExpeditionElements;
using ExileCore.PoEMemory.Elements.Sanctum;
using ExileCore.PoEMemory.MemoryObjects.Ancestor;
using ExileCore.PoEMemory.MemoryObjects.Metamorph;
using ExileCore.Shared.Cache;
using GameOffsets;

namespace ExileCore.PoEMemory.MemoryObjects;

public class IngameUIElements : Element
{
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct QuestListNode
	{
		public long Next;

		public long Prev;

		public long Ptr2_Key;

		public long Ptr1_Unused;

		public char Value;
	}

	private SyndicatePanel _syndicatePanel;

	private readonly CachedValue<IngameUIElementsOffsets> _cachedValue;

	private CraftBenchWindow _CraftBench;

	private Cursor _cursor;

	private IncursionWindow _IncursionWindow;

	private Map _map;

	private Element _SynthesisWindow;

	private Element _UnveilWindow;

	private Element _ZanaMissionChoice;

	private readonly CachedValue<Dictionary<string, KeyValuePair<Quest, QuestState>>> _cachedQuestStates;

	private RitualWindow _ritualWindow;

	public IngameUIElementsOffsets IngameUIElementsStruct => _cachedValue.Value;

	public GameUi GameUI => GetObject<GameUi>(IngameUIElementsStruct.GameUI);

	public SellWindow SellWindow => GetObject<SellWindow>(IngameUIElementsStruct.SellWindow);

	public SellWindowHideout SellWindowHideout => GetObject<SellWindowHideout>(IngameUIElementsStruct.SellWindowHideout);

	public MapDeviceWindow MapDeviceWindow => GetObject<MapDeviceWindow>(IngameUIElementsStruct.MapDeviceWindow);

	public TradeWindow TradeWindow => GetObject<TradeWindow>(IngameUIElementsStruct.TradeWindow);

	public NpcDialog NpcDialog => GetObject<NpcDialog>(IngameUIElementsStruct.NpcDialog);

	public BanditDialog BanditDialog => GetObject<BanditDialog>(IngameUIElementsStruct.BanditDialog);

	public Element SocialPanel => GetObject<Element>(IngameUIElementsStruct.SocialPanel);

	public PurchaseWindow PurchaseWindow => GetObject<PurchaseWindow>(IngameUIElementsStruct.PurchaseWindow);

	public PurchaseWindow PurchaseWindowHideout => GetObject<PurchaseWindow>(IngameUIElementsStruct.PurchaseWindowHideout);

	public SubterraneanChart DelveWindow => GetObject<SubterraneanChart>(IngameUIElementsStruct.DelveWindow);

	public SkillBarElement SkillBar => GetObject<SkillBarElement>(IngameUIElementsStruct.SkillBar);

	public SkillBarElement HiddenSkillBar => GetObject<SkillBarElement>(IngameUIElementsStruct.HiddenSkillBar);

	public ChatPanel ChatPanel => GetObject<ChatPanel>(IngameUIElementsStruct.ChatBox);

	public Element ChatTitlePanel => ChatPanel.ReadObjectAt<Element>(768);

	public PoeChatElement ChatBox => ChatPanel.ReadObjectAt<Element>(816).ReadObjectAt<PoeChatElement>(920);

	public IList<string> ChatMessages => ChatBox.Messages;

	public Element QuestTracker => GetObject<Element>(IngameUIElementsStruct.QuestTracker);

	public QuestRewardWindow QuestRewardWindow => GetObject<QuestRewardWindow>(IngameUIElementsStruct.QuestRewardWindow);

	public Element OpenLeftPanel => GetObject<Element>(IngameUIElementsStruct.OpenLeftPanel);

	public Element OpenRightPanel => GetObject<Element>(IngameUIElementsStruct.OpenRightPanel);

	public StashElement StashElement => GetObject<StashElement>(IngameUIElementsStruct.StashElement);

	public StashElement GuildStashElement => GetObject<StashElement>(IngameUIElementsStruct.GuildStashElement);

	public InventoryElement InventoryPanel => GetObject<InventoryElement>(IngameUIElementsStruct.InventoryPanel);

	public TreePanel TreePanel => GetObject<TreePanel>(IngameUIElementsStruct.TreePanel);

	public TreePanel AtlasTreePanel => GetObject<TreePanel>(IngameUIElementsStruct.AtlasSkillPanel);

	public Element PVPTreePanel => GetChildAtIndex(26);

	public AtlasPanel Atlas => GetObject<AtlasPanel>(IngameUIElementsStruct.AtlasPanel);

	public Element SettingsPanel => GetObject<Element>(IngameUIElementsStruct.SettingsPanel);

	public Element HelpWindow => GetObject<Element>(IngameUIElementsStruct.HelpWindow);

	public Element SentinelWindow => GetObject<Element>(IngameUIElementsStruct.SentinelWindow);

	public Map Map => _map ?? (_map = GetObject<Map>(IngameUIElementsStruct.Map));

	public ItemsOnGroundLabelElement ItemsOnGroundLabelElement => GetObject<ItemsOnGroundLabelElement>(IngameUIElementsStruct.itemsOnGroundLabelRoot);

	public IList<LabelOnGround> ItemsOnGroundLabels => ItemsOnGroundLabelElement.LabelsOnGround;

	public IList<LabelOnGround> ItemsOnGroundLabelsVisible => ItemsOnGroundLabelElement.LabelsOnGround?.Where((LabelOnGround x) => x.IsVisible).ToList() ?? new List<LabelOnGround>();

	public GemLvlUpPanel GemLvlUpPanel => GetObject<GemLvlUpPanel>(IngameUIElementsStruct.GemLvlUpPanel);

	public Element InvitesPanel => GetObject<Element>(IngameUIElementsStruct.InvitesPanel);

	public ItemOnGroundTooltip ItemOnGroundTooltip => GetObject<ItemOnGroundTooltip>(IngameUIElementsStruct.ItemOnGroundTooltip);

	public MapStashTabElement MapStashTab => ReadObject<MapStashTabElement>(IngameUIElementsStruct.MapTabWindowStartPtr + 2720);

	public Element Sulphit => GetObject<Element>(IngameUIElementsStruct.Map).GetChildAtIndex(3);

	public Element MapSideUI => GetObject<Element>(IngameUIElementsStruct.MapSideUI);

	public Cursor Cursor => _cursor ?? (_cursor = GetObject<Cursor>(IngameUIElementsStruct.Mouse));

	public Element SyndicateTree => GetObject<Element>(base.M.Read<long>(SyndicatePanel.Address + 2640));

	public Element UnveilWindow => _UnveilWindow ?? (_UnveilWindow = GetObject<Element>(IngameUIElementsStruct.UnveilWindow));

	public Element ZanaMissionChoice => _ZanaMissionChoice ?? (_ZanaMissionChoice = GetObject<Element>(IngameUIElementsStruct.ZanaMissionChoice));

	public IncursionWindow IncursionWindow => _IncursionWindow ?? (_IncursionWindow = GetObject<IncursionWindow>(IngameUIElementsStruct.IncursionWindow));

	public Element SynthesisWindow => _SynthesisWindow ?? (_SynthesisWindow = GetObject<Element>(IngameUIElementsStruct.SynthesisWindow));

	public Element AnointingWindow => GetObject<Element>(IngameUIElementsStruct.AnointingWindow);

	public CraftBenchWindow CraftBench => _CraftBench ?? (_CraftBench = GetObject<CraftBenchWindow>(IngameUIElementsStruct.CraftBenchWindow));

	[Obsolete]
	public bool IsDndEnabled => base.M.Read<byte>(base.Address + 3986) == 1;

	[Obsolete]
	public string DndMessage => base.M.ReadStringU(base.M.Read<long>(base.Address + 3992));

	public WorldMapElement AreaInstanceUi => GetObject<WorldMapElement>(IngameUIElementsStruct.AreaInstanceUi);

	public WorldMapElement WorldMap => GetObject<WorldMapElement>(IngameUIElementsStruct.WorldMap);

	public MetamorphWindowElement MetamorphWindow => GetObject<MetamorphWindowElement>(IngameUIElementsStruct.MetamorphWindow);

	public SyndicatePanel SyndicatePanel => _syndicatePanel ?? (_syndicatePanel = GetObject<SyndicatePanel>(IngameUIElementsStruct.BetrayalWindow));

	public InstanceManagerPanel InstanceManagerPanel => GetObject<InstanceManagerPanel>(IngameUIElementsStruct.InstanceManagerPanel);

	public ResurrectPanel ResurrectPanel => GetObject<ResurrectPanel>(IngameUIElementsStruct.ResurrectPanel);

	public MapReceptacleWindow MapReceptacleWindow => GetObject<MapReceptacleWindow>(IngameUIElementsStruct.MapReceptacleWindow);

	public Element PopUpWindow => GetObject<Element>(IngameUIElementsStruct.PopUpWindow);

	public CardTradeWindow CardTradeWindow => GetObject<CardTradeWindow>(IngameUIElementsStruct.CardTradeWindow);

	public RitualWindow RitualWindow => _ritualWindow ?? (_ritualWindow = GetObject<RitualWindow>(IngameUIElementsStruct.RitualWindow));

	public Element TrialPlaquePanel => GetObject<Element>(IngameUIElementsStruct.TrialPlaquePanel);

	public Element LabyrinthSelectPanel => GetObject<Element>(IngameUIElementsStruct.LabyrinthSelectPanel);

	public Element LabyrinthMapPanel => GetObject<Element>(IngameUIElementsStruct.LabyrinthMapPanel);

	public Element AscendancySelectPanel => GetObject<Element>(IngameUIElementsStruct.AscendancySelectPanel);

	public Element LabyrinthDivineFontPanel => GetObject<Element>(IngameUIElementsStruct.LabyrinthDivineFontPanel);

	public Element ChallengesPanel => GetObject<Element>(IngameUIElementsStruct.ChallengePanel);

	public ExpeditionVendorElement HaggleWindow => GetObject<ExpeditionVendorElement>(IngameUIElementsStruct.HaggleWindow);

	public Element ExpeditionNpcDialog => GetObject<Element>(IngameUIElementsStruct.ExpeditionNpcDialog);

	public Element ExpeditionWindow => GetObject<Element>(IngameUIElementsStruct.ExpeditionWindow);

	public Element ExpeditionWindowEmpty => GetObject<Element>(IngameUIElementsStruct.ExpeditionWindowEmpty);

	public Element ExpeditionLockerElement => GetObject<Element>(IngameUIElementsStruct.ExpeditionLockerElement);

	public SanctumFloorWindow SanctumFloorWindow => GetObject<SanctumFloorWindow>(IngameUIElementsStruct.SanctumFloorWindow);

	public Element SanctumRewardWindow => GetObject<Element>(IngameUIElementsStruct.SanctumRewardWindow);

	public Element HeistWindow => GetObject<Element>(IngameUIElementsStruct.HeistWindow);

	public Element BlueprintWindow => GetObject<Element>(IngameUIElementsStruct.BlueprintWindow);

	public Element HeistLockerElement => GetObject<Element>(IngameUIElementsStruct.HeistLockerElement);

	public Element AllyEquipmentWindow => GetObject<Element>(IngameUIElementsStruct.AllyEquipmentWindow);

	public AncestorFightSelectionWindow AncestorFightSelectionWindow => GetObject<AncestorFightSelectionWindow>(IngameUIElementsStruct.AncestorFightSelectionWindow);

	public AncestorMainShopWindow AncestorMainShopWindow => GetObject<AncestorMainShopWindow>(IngameUIElementsStruct.AncestorMainShopWindow);

	public AncestorSideShopPanel AncestorLeftShopPanel => GetObject<AncestorSideShopPanel>(IngameUIElementsStruct.AncestorLeftShopPanel);

	public AncestorSideShopPanel AncestorRightShopPanel => GetObject<AncestorSideShopPanel>(IngameUIElementsStruct.AncestorRightShopPanel);

	public Element GrandHeistWindow => GetObject<Element>(IngameUIElementsStruct.GrandHeistWindow);

	public Element CurrencyShiftClickMenu => GetObject<Element>(IngameUIElementsStruct.CurrencyShiftClickMenu);

	public Element PartyElement => GetObject<Element>(IngameUIElementsStruct.PartyElement);

	public HarvestWindow HorticraftingStationWindow => GetObject<HarvestWindow>(IngameUIElementsStruct.HorticraftingStationWindow);

	public UltimatumPanel UltimatumPanel => GetObject<UltimatumPanel>(IngameUIElementsStruct.UltimatumPanel);

	public KalandraTabletWindow KalandraTabletWindow => GetObject<KalandraTabletWindow>(IngameUIElementsStruct.KalandraTabletWindow);

	public Element HighlightedElement => base.Root?.GetChildFromIndices(1, 6, 1, 0);

	public IList<Tuple<Quest, int>> GetUncompletedQuests => (from q in GetQuestStates
		where q.Value.Value != null && q.Value.Value.QuestStateId != 0
		select q into x
		select new Tuple<Quest, int>(x.Value.Key, x.Value.Value.QuestStateId)).ToList();

	public IList<Tuple<Quest, int>> GetCompletedQuests => (from q in GetQuestStates
		where q.Value.Value != null && q.Value.Value.QuestStateId == 0
		select q into x
		select new Tuple<Quest, int>(x.Value.Key, x.Value.Value.QuestStateId)).ToList();

	public Dictionary<Quest, QuestState> GetUncompletedQuests2 => (from q in GetQuestStates
		where q.Value.Value != null && q.Value.Value.QuestStateId > 0 && q.Value.Value.QuestStateId < 255
		select q into x
		select x.Value.Value).ToDictionary((QuestState x) => x.Quest);

	public Dictionary<string, KeyValuePair<Quest, QuestState>> GetQuestStates => _cachedQuestStates.Value;

	public Element LeagueMechanicButtons => GetObject<Element>(IngameUIElementsStruct.LeagueMechanicButtons);

	public ExpeditionDetonator ExpeditionDetonatorElement => GetObject<ExpeditionDetonator>(IngameUIElementsStruct.ExpeditionDetonatorElement);

	public List<Element> FullscreenPanels => new Element[4] { TreePanel, AtlasTreePanel, Atlas, SyndicatePanel }.Where((Element x) => x.IsValid).ToList();

	public List<Element> LargePanels => new Element[17]
	{
		SellWindow, SellWindowHideout, MapDeviceWindow, TradeWindow, PurchaseWindow, PurchaseWindowHideout, DelveWindow, IncursionWindow, UnveilWindow, CraftBench,
		HelpWindow, MetamorphWindow, CardTradeWindow, RitualWindow, SanctumFloorWindow, AncestorFightSelectionWindow, AncestorMainShopWindow
	}.Where((Element x) => x.IsValid).ToList();

	[Obsolete("Removed from the game")]
	public ArchnemesisPanelElement ArchnemesisInventoryPanel => GetObject<ArchnemesisPanelElement>((IntPtr)0);

	[Obsolete("Removed from the game")]
	public ArchnemesisAltarElement ArchnemesisAltarPanel => GetObject<ArchnemesisAltarElement>((IntPtr)0);

	[Obsolete("Removed from the game")]
	public HarvestWindow HarvestWindow => GetObject<HarvestWindow>((IntPtr)0);

	[Obsolete("Use PopUpWindow instead")]
	public Element DestroyConfirmationWindow => PopUpWindow;

	[Obsolete("Use SyndicatePanel instead")]
	public Element BetrayalWindow => SyndicatePanel;

	[Obsolete("Use Atlas instead")]
	public Element AtlasPanel => Atlas;

	public IList<(Quest, int)> GetQuests
	{
		get
		{
			if (IngameUIElementsStruct.GetQuests == 0L)
			{
				return new List<(Quest, int)>();
			}
			return (from x in ReadQuestsList(IngameUIElementsStruct.GetQuests)
				select (base.TheGame.Files.Quests.GetByAddress(x.Item1), x.Item2)).ToList();
		}
	}

	public IngameUIElements()
	{
		_cachedValue = new FrameCache<IngameUIElementsOffsets>(() => base.M.Read<IngameUIElementsOffsets>(base.Address));
		_cachedQuestStates = new TimeCache<Dictionary<string, KeyValuePair<Quest, QuestState>>>(GenerateQuestStates, 1000L);
	}

	private Dictionary<string, KeyValuePair<Quest, QuestState>> GenerateQuestStates()
	{
		if (IngameUIElementsStruct.GetQuests == 0L)
		{
			return new Dictionary<string, KeyValuePair<Quest, QuestState>>();
		}
		Dictionary<string, KeyValuePair<Quest, QuestState>> dictionary = new Dictionary<string, KeyValuePair<Quest, QuestState>>();
		foreach (var getQuest in GetQuests)
		{
			if (getQuest.Item1 != null)
			{
				QuestState questState = base.TheGame.Files.QuestStates.GetQuestState(getQuest.Item1.Id, getQuest.Item2);
				dictionary.TryAdd(getQuest.Item1.Id, new KeyValuePair<Quest, QuestState>(getQuest.Item1, questState));
			}
		}
		return dictionary;
	}

	public IList<(long, int)> ReadQuestsList(long address)
	{
		List<(long, int)> list = new List<(long, int)>();
		long num = base.M.Read<long>(address);
		QuestListNode questListNode = base.M.Read<QuestListNode>(num);
		list.Add((questListNode.Ptr2_Key, questListNode.Value));
		Stopwatch stopwatch = Stopwatch.StartNew();
		while (num != questListNode.Next)
		{
			if (stopwatch.ElapsedMilliseconds > 2000)
			{
				Core.Logger?.Error($"ReadQuestsList error result count: {list.Count}");
				return new List<(long, int)>();
			}
			questListNode = base.M.Read<QuestListNode>(questListNode.Next);
			list.Add((questListNode.Ptr2_Key, questListNode.Value));
		}
		if (list.Count > 0)
		{
			list.RemoveAt(list.Count - 1);
		}
		return list;
	}
}
