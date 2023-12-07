#define TRACE
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExileCore.PoEMemory.FilesInMemory;
using ExileCore.PoEMemory.FilesInMemory.Ancestor;
using ExileCore.PoEMemory.FilesInMemory.Archnemesis;
using ExileCore.PoEMemory.FilesInMemory.Atlas;
using ExileCore.PoEMemory.FilesInMemory.Harvest;
using ExileCore.PoEMemory.FilesInMemory.Labyrinth;
using ExileCore.PoEMemory.FilesInMemory.Metamorph;
using ExileCore.PoEMemory.FilesInMemory.Sanctum;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.PoEMemory.MemoryObjects.Heist;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Static;

namespace ExileCore.PoEMemory;

public class FilesContainer
{
	private UniversalFileWrapper<AncestralTrialUnit> _ancestralTrialUnits;

	private UniversalFileWrapper<AncestralTrialItem> _ancestralTrialItems;

	private UniversalFileWrapper<AncestralTrialTribe> _ancestralTrialTribes;

	private readonly IMemory _memory;

	private BaseItemTypes _baseItemTypes;

	private UniversalFileWrapper<BetrayalChoiceAction> _betrayalChoiceActions;

	private UniversalFileWrapper<BetrayalChoice> _betrayalChoises;

	private UniversalFileWrapper<BetrayalDialogue> _betrayalDialogue;

	private UniversalFileWrapper<ArchnemesisRecipe> _archnemesisRecipes;

	private UniversalFileWrapper<BetrayalJob> _betrayalJobs;

	private UniversalFileWrapper<BetrayalRank> _betrayalRanks;

	private UniversalFileWrapper<BetrayalReward> _betrayalRewards;

	private UniversalFileWrapper<BetrayalTarget> _betrayalTargets;

	private UniversalFileWrapper<HeistJobRecord> _HeistJobs;

	private UniversalFileWrapper<HeistChestRewardTypeRecord> _HeistChestRewardTypes;

	private UniversalFileWrapper<HeistNpcRecord> _HeistNpcs;

	private UniversalFileWrapper<ClientString> _clientString;

	private StatDescriptionWrapper<StatDescription> _statDescriptions;

	private ModsDat _mods;

	private StatsDat _stats;

	private TagsDat _tags;

	private ItemVisualIdentities _itemVisualIdentities;

	private UniqueItemDescriptions _uniqueItemDescriptions;

	private UniversalFileWrapper<WordEntry> _word;

	private UniversalFileWrapper<AtlasNode> atlasNodes;

	public FilesFromMemory FilesFromMemory;

	private LabyrinthTrials labyrinthTrials;

	private MonsterVarieties monsterVarieties;

	private PassiveSkills passiveSkills;

	private PropheciesDat prophecies;

	private Quests quests;

	private QuestStates questStates;

	private UniversalFileWrapper<ArchnemesisMod> _archnemesisMods;

	private UniversalFileWrapper<LakeRoom> _lakeRooms;

	private UniversalFileWrapper<StampChoice> _stampChoices;

	private UniversalFileWrapper<BlightTowerDat> _blightTowers;

	private UniversalFileWrapper<HarvestSeed> _harvestSeeds;

	private UniversalFileWrapper<HeistChestRecord> _heistChests;

	private UniversalFileWrapper<ChestRecord> _chests;

	private UniversalFileWrapper<QuestReward> _questRewards;

	private UniversalFileWrapper<QuestRewardOffer> _questRewardOffers;

	private UniversalFileWrapper<Character> _characters;

	private UniversalFileWrapper<GrantedEffectPerLevel> _grantedEffectsPerLevel;

	private UniversalFileWrapper<GrantedEffect> _grantedEffects;

	private UniversalFileWrapper<BuffVisual> _buffVisuals;

	private UniversalFileWrapper<BuffDefinition> _buffDefinitions;

	private WorldAreas worldAreas;

	private UniversalFileWrapper<MetamorphMetaSkill> _metamorphMetaSkills;

	private UniversalFileWrapper<MetamorphMetaSkillType> _metamorphMetaSkillTypes;

	private UniversalFileWrapper<MetamorphMetaMonster> _metamorphMetaMonsters;

	private UniversalFileWrapper<MetamorphRewardType> _metamorphRewardTypes;

	private UniversalFileWrapper<MetamorphRewardTypeItemsClient> _metamorphRewardTypeItemsClient;

	private AtlasRegions _atlasRegions;

	private BestiaryCapturableMonsters _bestiaryCapturableMonsters;

	private UniversalFileWrapper<BestiaryRecipe> _bestiaryRecipes;

	private UniversalFileWrapper<BestiaryRecipeComponent> _bestiaryRecipeComponents;

	private UniversalFileWrapper<BestiaryGroup> _bestiaryGroups;

	private UniversalFileWrapper<BestiaryFamily> _bestiaryFamilies;

	private UniversalFileWrapper<BestiaryGenus> _bestiaryGenuses;

	private UniversalFileWrapper<LabyrinthArea> _labyrinthAreas;

	private UniversalFileWrapper<LabyrinthSectionDat> _labyrinthSections;

	private UniversalFileWrapper<LabyrinthSectionLayout> _labyrinthSectionLayouts;

	private UniversalFileWrapper<LabyrinthSecret> _labyrinthSecrets;

	private UniversalFileWrapper<LabyrinthNodeOverride> _labyrinthNodeOverrides;

	private UniversalFileWrapper<SanctumRoom> _sanctumRooms;

	private UniversalFileWrapper<SanctumRoomType> _sanctumRoomTypes;

	private UniversalFileWrapper<SanctumPersistentEffect> _sanctumPersistentEffects;

	private UniversalFileWrapper<SanctumDeferredRewardCategory> _sanctumDeferredRewardCategories;

	private UniversalFileWrapper<SanctumDeferredReward> _sanctumDeferredRewards;

	public UniversalFileWrapper<AncestralTrialUnit> AncestralTrialUnits
	{
		get
		{
			UniversalFileWrapper<AncestralTrialUnit> universalFileWrapper = _ancestralTrialUnits;
			if (universalFileWrapper == null)
			{
				UniversalFileWrapper<AncestralTrialUnit> obj = new UniversalFileWrapper<AncestralTrialUnit>(_memory, () => FindFile("Data/AncestralTrialUnits.dat"))
				{
					ExcludeZeroAddresses = true
				};
				UniversalFileWrapper<AncestralTrialUnit> universalFileWrapper2 = obj;
				_ancestralTrialUnits = obj;
				universalFileWrapper = universalFileWrapper2;
			}
			return universalFileWrapper;
		}
	}

	public UniversalFileWrapper<AncestralTrialItem> AncestralTrialItems
	{
		get
		{
			UniversalFileWrapper<AncestralTrialItem> universalFileWrapper = _ancestralTrialItems;
			if (universalFileWrapper == null)
			{
				UniversalFileWrapper<AncestralTrialItem> obj = new UniversalFileWrapper<AncestralTrialItem>(_memory, () => FindFile("Data/AncestralTrialItems.dat"))
				{
					ExcludeZeroAddresses = true
				};
				UniversalFileWrapper<AncestralTrialItem> universalFileWrapper2 = obj;
				_ancestralTrialItems = obj;
				universalFileWrapper = universalFileWrapper2;
			}
			return universalFileWrapper;
		}
	}

	public UniversalFileWrapper<AncestralTrialTribe> AncestralTrialTribes
	{
		get
		{
			UniversalFileWrapper<AncestralTrialTribe> universalFileWrapper = _ancestralTrialTribes;
			if (universalFileWrapper == null)
			{
				UniversalFileWrapper<AncestralTrialTribe> obj = new UniversalFileWrapper<AncestralTrialTribe>(_memory, () => FindFile("Data/AncestralTrialTribes.dat"))
				{
					ExcludeZeroAddresses = true
				};
				UniversalFileWrapper<AncestralTrialTribe> universalFileWrapper2 = obj;
				_ancestralTrialTribes = obj;
				universalFileWrapper = universalFileWrapper2;
			}
			return universalFileWrapper;
		}
	}

	public ItemClasses ItemClasses { get; }

	public BaseItemTypes BaseItemTypes => _baseItemTypes ?? (_baseItemTypes = new BaseItemTypes(_memory, () => FindFile("Data/BaseItemTypes.dat")));

	public UniversalFileWrapper<ClientString> ClientStrings => _clientString ?? (_clientString = new UniversalFileWrapper<ClientString>(_memory, () => FindFile("Data/ClientStrings.dat")));

	public StatDescriptionWrapper<StatDescription> StatDescriptions => _statDescriptions ?? (_statDescriptions = new StatDescriptionWrapper<StatDescription>(_memory, () => FindFile("Metadata/StatDescriptions/stat_descriptions.txt")));

	public ModsDat Mods => _mods ?? (_mods = new ModsDat(_memory, () => FindFile("Data/Mods.dat"), Stats, Tags));

	public StatsDat Stats => _stats ?? (_stats = new StatsDat(_memory, () => FindFile("Data/Stats.dat")));

	public TagsDat Tags => _tags ?? (_tags = new TagsDat(_memory, () => FindFile("Data/Tags.dat")));

	public WorldAreas WorldAreas => worldAreas ?? (worldAreas = new WorldAreas(_memory, () => FindFile("Data/WorldAreas.dat")));

	public PassiveSkills PassiveSkills => passiveSkills ?? (passiveSkills = new PassiveSkills(_memory, () => FindFile("Data/PassiveSkills.dat")));

	public LabyrinthTrials LabyrinthTrials => labyrinthTrials ?? (labyrinthTrials = new LabyrinthTrials(_memory, () => FindFile("Data/LabyrinthTrials.dat")));

	public Quests Quests => quests ?? (quests = new Quests(_memory, () => FindFile("Data/Quest.dat")));

	public QuestStates QuestStates => questStates ?? (questStates = new QuestStates(_memory, () => FindFile("Data/QuestStates.dat")));

	public UniversalFileWrapper<QuestReward> QuestRewards => _questRewards ?? (_questRewards = new UniversalFileWrapper<QuestReward>(_memory, () => FindFile("Data/QuestRewards.dat")));

	public UniversalFileWrapper<QuestRewardOffer> QuestRewardOffers => _questRewardOffers ?? (_questRewardOffers = new UniversalFileWrapper<QuestRewardOffer>(_memory, () => FindFile("Data/QuestRewardOffers.dat")));

	public UniversalFileWrapper<Character> Characters => _characters ?? (_characters = new UniversalFileWrapper<Character>(_memory, () => FindFile("Data/Characters.dat")));

	public MonsterVarieties MonsterVarieties => monsterVarieties ?? (monsterVarieties = new MonsterVarieties(_memory, () => FindFile("Data/MonsterVarieties.dat")));

	public PropheciesDat Prophecies => prophecies ?? (prophecies = new PropheciesDat(_memory, () => FindFile("Data/Prophecies.dat")));

	public ItemVisualIdentities ItemVisualIdentities => _itemVisualIdentities ?? (_itemVisualIdentities = new ItemVisualIdentities(_memory, () => FindFile("Data/ItemVisualIdentity.dat")));

	public UniqueItemDescriptions UniqueItemDescriptions => _uniqueItemDescriptions ?? (_uniqueItemDescriptions = new UniqueItemDescriptions(_memory, () => FindFile("Data/UniqueStashLayout.dat")));

	public UniversalFileWrapper<WordEntry> Words => _word ?? (_word = new UniversalFileWrapper<WordEntry>(_memory, () => FindFile("Data/Words.dat")));

	public UniversalFileWrapper<AtlasNode> AtlasNodes => atlasNodes ?? (atlasNodes = new AtlasNodes(_memory, () => FindFile("Data/AtlasNode.dat")));

	public UniversalFileWrapper<BetrayalTarget> BetrayalTargets => _betrayalTargets ?? (_betrayalTargets = new UniversalFileWrapper<BetrayalTarget>(_memory, () => FindFile("Data/BetrayalTargets.dat")));

	public UniversalFileWrapper<BetrayalJob> BetrayalJobs => _betrayalJobs ?? (_betrayalJobs = new UniversalFileWrapper<BetrayalJob>(_memory, () => FindFile("Data/BetrayalJobs.dat")));

	public UniversalFileWrapper<BetrayalRank> BetrayalRanks => _betrayalRanks ?? (_betrayalRanks = new UniversalFileWrapper<BetrayalRank>(_memory, () => FindFile("Data/BetrayalRanks.dat")));

	public UniversalFileWrapper<BetrayalReward> BetrayalRewards => _betrayalRewards ?? (_betrayalRewards = new UniversalFileWrapper<BetrayalReward>(_memory, () => FindFile("Data/BetrayalTraitorRewards.dat")));

	public UniversalFileWrapper<BetrayalChoice> BetrayalChoises => _betrayalChoises ?? (_betrayalChoises = new UniversalFileWrapper<BetrayalChoice>(_memory, () => FindFile("Data/BetrayalChoices.dat")));

	public UniversalFileWrapper<BetrayalChoiceAction> BetrayalChoiceActions => _betrayalChoiceActions ?? (_betrayalChoiceActions = new UniversalFileWrapper<BetrayalChoiceAction>(_memory, () => FindFile("Data/BetrayalChoiceActions.dat")));

	public UniversalFileWrapper<BetrayalDialogue> BetrayalDialogue => _betrayalDialogue ?? (_betrayalDialogue = new UniversalFileWrapper<BetrayalDialogue>(_memory, () => FindFile("Data/BetrayalDialogue.dat")));

	public UniversalFileWrapper<ArchnemesisRecipe> ArchnemesisRecipes => _archnemesisRecipes ?? (_archnemesisRecipes = new UniversalFileWrapper<ArchnemesisRecipe>(_memory, () => FindFile("Data/ArchnemesisRecipes.dat")));

	public UniversalFileWrapper<ArchnemesisMod> ArchnemesisMods => _archnemesisMods ?? (_archnemesisMods = new UniversalFileWrapper<ArchnemesisMod>(_memory, () => FindFile("Data/ArchnemesisMods.dat")));

	public UniversalFileWrapper<LakeRoom> LakeRooms => _lakeRooms ?? (_lakeRooms = new UniversalFileWrapper<LakeRoom>(_memory, () => FindFile("Data/LakeRooms.dat")));

	public UniversalFileWrapper<StampChoice> StampChoices => _stampChoices ?? (_stampChoices = new UniversalFileWrapper<StampChoice>(_memory, () => FindFile("Data/StampChoice.dat")));

	public UniversalFileWrapper<BlightTowerDat> BlightTowers => _blightTowers ?? (_blightTowers = new UniversalFileWrapper<BlightTowerDat>(_memory, () => FindFile("Data/BlightTowers.dat")));

	public UniversalFileWrapper<HarvestSeed> HarvestSeeds => _harvestSeeds ?? (_harvestSeeds = new UniversalFileWrapper<HarvestSeed>(_memory, () => FindFile("Data/HarvestSeeds.dat")));

	public UniversalFileWrapper<GrantedEffectPerLevel> GrantedEffectsPerLevel => _grantedEffectsPerLevel ?? (_grantedEffectsPerLevel = new UniversalFileWrapper<GrantedEffectPerLevel>(_memory, () => FindFile("Data/GrantedEffectsPerLevel.dat")));

	public UniversalFileWrapper<GrantedEffect> GrantedEffects => _grantedEffects ?? (_grantedEffects = new UniversalFileWrapper<GrantedEffect>(_memory, () => FindFile("Data/GrantedEffects.dat")));

	public UniversalFileWrapper<BuffDefinition> BuffDefinitions => _buffDefinitions ?? (_buffDefinitions = new UniversalFileWrapper<BuffDefinition>(_memory, () => FindFile("Data/BuffDefinitions.dat")));

	public UniversalFileWrapper<BuffVisual> BuffVisuals => _buffVisuals ?? (_buffVisuals = new UniversalFileWrapper<BuffVisual>(_memory, () => FindFile("Data/BuffVisuals.dat")));

	public UniversalFileWrapper<HeistChestRecord> HeistChests => _heistChests ?? (_heistChests = new UniversalFileWrapper<HeistChestRecord>(_memory, () => FindFile("Data/HeistChests.dat")));

	public UniversalFileWrapper<ChestRecord> Chests => _chests ?? (_chests = new UniversalFileWrapper<ChestRecord>(_memory, () => FindFile("Data/Chests.dat")));

	public UniversalFileWrapper<HeistJobRecord> HeistJobs => _HeistJobs ?? (_HeistJobs = new UniversalFileWrapper<HeistJobRecord>(_memory, () => FindFile("Data/HeistJobs.dat")));

	public UniversalFileWrapper<HeistChestRewardTypeRecord> HeistChestRewardType => _HeistChestRewardTypes ?? (_HeistChestRewardTypes = new UniversalFileWrapper<HeistChestRewardTypeRecord>(_memory, () => FindFile("Data/HeistChestRewardTypes.dat")));

	public UniversalFileWrapper<HeistNpcRecord> HeistNpcs => _HeistNpcs ?? (_HeistNpcs = new UniversalFileWrapper<HeistNpcRecord>(_memory, () => FindFile("Data/HeistNPCs.dat")));

	public UniversalFileWrapper<MetamorphMetaSkill> MetamorphMetaSkills => _metamorphMetaSkills ?? (_metamorphMetaSkills = new UniversalFileWrapper<MetamorphMetaSkill>(_memory, () => FindFile("Data/MetamorphosisMetaSkills.dat")));

	public UniversalFileWrapper<MetamorphMetaSkillType> MetamorphMetaSkillTypes => _metamorphMetaSkillTypes ?? (_metamorphMetaSkillTypes = new UniversalFileWrapper<MetamorphMetaSkillType>(_memory, () => FindFile("Data/MetamorphosisMetaSkillTypes.dat")));

	public UniversalFileWrapper<MetamorphMetaMonster> MetamorphMetaMonsters => _metamorphMetaMonsters ?? (_metamorphMetaMonsters = new UniversalFileWrapper<MetamorphMetaMonster>(_memory, () => FindFile("Data/MetamorphosisMetaMonsters.dat")));

	public UniversalFileWrapper<MetamorphRewardType> MetamorphRewardTypes => _metamorphRewardTypes ?? (_metamorphRewardTypes = new UniversalFileWrapper<MetamorphRewardType>(_memory, () => FindFile("Data/MetamorphosisRewardTypes.dat")));

	public UniversalFileWrapper<MetamorphRewardTypeItemsClient> MetamorphRewardTypeItemsClient => _metamorphRewardTypeItemsClient ?? (_metamorphRewardTypeItemsClient = new UniversalFileWrapper<MetamorphRewardTypeItemsClient>(_memory, () => FindFile("Data/MetamorphosisRewardTypeItemsClient.dat")));

	public AtlasRegions AtlasRegions => _atlasRegions ?? (_atlasRegions = new AtlasRegions(_memory, () => FindFile("Data/AtlasRegions.dat")));

	public Dictionary<string, FileInformation> AllFiles { get; private set; }

	public Dictionary<string, FileInformation> Metadata { get; } = new Dictionary<string, FileInformation>();


	public Dictionary<string, FileInformation> Data { get; private set; } = new Dictionary<string, FileInformation>();


	public Dictionary<string, FileInformation> OtherFiles { get; } = new Dictionary<string, FileInformation>();


	public Dictionary<string, FileInformation> LoadedInThisArea { get; private set; } = new Dictionary<string, FileInformation>(1024);


	public Dictionary<int, List<KeyValuePair<string, FileInformation>>> GroupedByTest2 { get; set; }

	public Dictionary<int, List<KeyValuePair<string, FileInformation>>> GroupedByChangeAction { get; set; }

	public BestiaryCapturableMonsters BestiaryCapturableMonsters => _bestiaryCapturableMonsters ?? (_bestiaryCapturableMonsters = new BestiaryCapturableMonsters(_memory, () => FindFile("Data/BestiaryCapturableMonsters.dat")));

	public UniversalFileWrapper<BestiaryRecipe> BestiaryRecipes => _bestiaryRecipes ?? (_bestiaryRecipes = new UniversalFileWrapper<BestiaryRecipe>(_memory, () => FindFile("Data/BestiaryRecipes.dat")));

	public UniversalFileWrapper<BestiaryRecipeComponent> BestiaryRecipeComponents => _bestiaryRecipeComponents ?? (_bestiaryRecipeComponents = new UniversalFileWrapper<BestiaryRecipeComponent>(_memory, () => FindFile("Data/BestiaryRecipeComponent.dat")));

	public UniversalFileWrapper<BestiaryGroup> BestiaryGroups => _bestiaryGroups ?? (_bestiaryGroups = new UniversalFileWrapper<BestiaryGroup>(_memory, () => FindFile("Data/BestiaryGroups.dat")));

	public UniversalFileWrapper<BestiaryFamily> BestiaryFamilies => _bestiaryFamilies ?? (_bestiaryFamilies = new UniversalFileWrapper<BestiaryFamily>(_memory, () => FindFile("Data/BestiaryFamilies.dat")));

	public UniversalFileWrapper<BestiaryGenus> BestiaryGenuses => _bestiaryGenuses ?? (_bestiaryGenuses = new UniversalFileWrapper<BestiaryGenus>(_memory, () => FindFile("Data/BestiaryGenus.dat")));

	public UniversalFileWrapper<LabyrinthArea> LabyrinthAreas
	{
		get
		{
			UniversalFileWrapper<LabyrinthArea> universalFileWrapper = _labyrinthAreas;
			if (universalFileWrapper == null)
			{
				UniversalFileWrapper<LabyrinthArea> obj = new UniversalFileWrapper<LabyrinthArea>(_memory, () => FindFile("Data/LabyrinthAreas.dat"))
				{
					ExcludeZeroAddresses = true
				};
				UniversalFileWrapper<LabyrinthArea> universalFileWrapper2 = obj;
				_labyrinthAreas = obj;
				universalFileWrapper = universalFileWrapper2;
			}
			return universalFileWrapper;
		}
	}

	public UniversalFileWrapper<LabyrinthSectionDat> LabyrinthSections
	{
		get
		{
			UniversalFileWrapper<LabyrinthSectionDat> universalFileWrapper = _labyrinthSections;
			if (universalFileWrapper == null)
			{
				UniversalFileWrapper<LabyrinthSectionDat> obj = new UniversalFileWrapper<LabyrinthSectionDat>(_memory, () => FindFile("Data/LabyrinthSection.dat"))
				{
					ExcludeZeroAddresses = true
				};
				UniversalFileWrapper<LabyrinthSectionDat> universalFileWrapper2 = obj;
				_labyrinthSections = obj;
				universalFileWrapper = universalFileWrapper2;
			}
			return universalFileWrapper;
		}
	}

	public UniversalFileWrapper<LabyrinthSectionLayout> LabyrinthSectionLayouts
	{
		get
		{
			UniversalFileWrapper<LabyrinthSectionLayout> universalFileWrapper = _labyrinthSectionLayouts;
			if (universalFileWrapper == null)
			{
				UniversalFileWrapper<LabyrinthSectionLayout> obj = new UniversalFileWrapper<LabyrinthSectionLayout>(_memory, () => FindFile("Data/LabyrinthSectionLayout.dat"))
				{
					ExcludeZeroAddresses = true
				};
				UniversalFileWrapper<LabyrinthSectionLayout> universalFileWrapper2 = obj;
				_labyrinthSectionLayouts = obj;
				universalFileWrapper = universalFileWrapper2;
			}
			return universalFileWrapper;
		}
	}

	public UniversalFileWrapper<LabyrinthSecret> LabyrinthSecrets
	{
		get
		{
			UniversalFileWrapper<LabyrinthSecret> universalFileWrapper = _labyrinthSecrets;
			if (universalFileWrapper == null)
			{
				UniversalFileWrapper<LabyrinthSecret> obj = new UniversalFileWrapper<LabyrinthSecret>(_memory, () => FindFile("Data/LabyrinthSecrets.dat"))
				{
					ExcludeZeroAddresses = true
				};
				UniversalFileWrapper<LabyrinthSecret> universalFileWrapper2 = obj;
				_labyrinthSecrets = obj;
				universalFileWrapper = universalFileWrapper2;
			}
			return universalFileWrapper;
		}
	}

	public UniversalFileWrapper<LabyrinthNodeOverride> LabyrinthNodeOverrides
	{
		get
		{
			UniversalFileWrapper<LabyrinthNodeOverride> universalFileWrapper = _labyrinthNodeOverrides;
			if (universalFileWrapper == null)
			{
				UniversalFileWrapper<LabyrinthNodeOverride> obj = new UniversalFileWrapper<LabyrinthNodeOverride>(_memory, () => FindFile("Data/LabyrinthNodeOverrides.dat"))
				{
					ExcludeZeroAddresses = true
				};
				UniversalFileWrapper<LabyrinthNodeOverride> universalFileWrapper2 = obj;
				_labyrinthNodeOverrides = obj;
				universalFileWrapper = universalFileWrapper2;
			}
			return universalFileWrapper;
		}
	}

	public UniversalFileWrapper<SanctumRoom> SanctumRooms
	{
		get
		{
			UniversalFileWrapper<SanctumRoom> universalFileWrapper = _sanctumRooms;
			if (universalFileWrapper == null)
			{
				UniversalFileWrapper<SanctumRoom> obj = new UniversalFileWrapper<SanctumRoom>(_memory, () => FindFile("Data/SanctumRooms.dat"))
				{
					ExcludeZeroAddresses = true
				};
				UniversalFileWrapper<SanctumRoom> universalFileWrapper2 = obj;
				_sanctumRooms = obj;
				universalFileWrapper = universalFileWrapper2;
			}
			return universalFileWrapper;
		}
	}

	public UniversalFileWrapper<SanctumRoomType> SanctumRoomTypes
	{
		get
		{
			UniversalFileWrapper<SanctumRoomType> universalFileWrapper = _sanctumRoomTypes;
			if (universalFileWrapper == null)
			{
				UniversalFileWrapper<SanctumRoomType> obj = new UniversalFileWrapper<SanctumRoomType>(_memory, () => FindFile("Data/SanctumRoomTypes.dat"))
				{
					ExcludeZeroAddresses = true
				};
				UniversalFileWrapper<SanctumRoomType> universalFileWrapper2 = obj;
				_sanctumRoomTypes = obj;
				universalFileWrapper = universalFileWrapper2;
			}
			return universalFileWrapper;
		}
	}

	public UniversalFileWrapper<SanctumDeferredRewardCategory> SanctumDeferredRewardCategories
	{
		get
		{
			UniversalFileWrapper<SanctumDeferredRewardCategory> universalFileWrapper = _sanctumDeferredRewardCategories;
			if (universalFileWrapper == null)
			{
				UniversalFileWrapper<SanctumDeferredRewardCategory> obj = new UniversalFileWrapper<SanctumDeferredRewardCategory>(_memory, () => FindFile("Data/SanctumDeferredRewardCategories.dat"))
				{
					ExcludeZeroAddresses = true
				};
				UniversalFileWrapper<SanctumDeferredRewardCategory> universalFileWrapper2 = obj;
				_sanctumDeferredRewardCategories = obj;
				universalFileWrapper = universalFileWrapper2;
			}
			return universalFileWrapper;
		}
	}

	public UniversalFileWrapper<SanctumDeferredReward> SanctumDeferredRewards
	{
		get
		{
			UniversalFileWrapper<SanctumDeferredReward> universalFileWrapper = _sanctumDeferredRewards;
			if (universalFileWrapper == null)
			{
				UniversalFileWrapper<SanctumDeferredReward> obj = new UniversalFileWrapper<SanctumDeferredReward>(_memory, () => FindFile("Data/SanctumDeferredRewards.dat"))
				{
					ExcludeZeroAddresses = true
				};
				UniversalFileWrapper<SanctumDeferredReward> universalFileWrapper2 = obj;
				_sanctumDeferredRewards = obj;
				universalFileWrapper = universalFileWrapper2;
			}
			return universalFileWrapper;
		}
	}

	public UniversalFileWrapper<SanctumPersistentEffect> SanctumPersistentEffects
	{
		get
		{
			UniversalFileWrapper<SanctumPersistentEffect> universalFileWrapper = _sanctumPersistentEffects;
			if (universalFileWrapper == null)
			{
				UniversalFileWrapper<SanctumPersistentEffect> obj = new UniversalFileWrapper<SanctumPersistentEffect>(_memory, () => FindFile("Data/SanctumPersistentEffects.dat"))
				{
					ExcludeZeroAddresses = true
				};
				UniversalFileWrapper<SanctumPersistentEffect> universalFileWrapper2 = obj;
				_sanctumPersistentEffects = obj;
				universalFileWrapper = universalFileWrapper2;
			}
			return universalFileWrapper;
		}
	}

	public event EventHandler<Dictionary<string, FileInformation>> LoadedFiles;

	public FilesContainer(IMemory memory)
	{
		_memory = memory;
		ItemClasses = new ItemClasses();
		FilesFromMemory = new FilesFromMemory(_memory);
		using (new PerformanceTimer("Load files from memory"))
		{
			AllFiles = FilesFromMemory.GetAllFiles();
			Trace.WriteLine($"Loaded {AllFiles.Count} files from memory {AllFiles.Values.Count((FileInformation x) => x.Ptr > 0)}/{AllFiles.Count} has pointers.");
		}
		Task.Run(delegate
		{
			using (new PerformanceTimer("Preload stats and mods"))
			{
				_ = Stats.records.Count;
				_ = Mods.records.Count;
				ParseFiles(AllFiles);
			}
		});
	}

	public void LoadFiles()
	{
		AllFiles = FilesFromMemory.GetAllFilesSync();
	}

	public void ParseFiles(Dictionary<string, FileInformation> files)
	{
		foreach (KeyValuePair<string, FileInformation> file in files)
		{
			if (!string.IsNullOrEmpty(file.Key))
			{
				if (file.Key.StartsWith("Metadata/", StringComparison.Ordinal))
				{
					Metadata[file.Key] = file.Value;
				}
				else if (file.Key.StartsWith("Data/", StringComparison.Ordinal) && file.Key.EndsWith(".dat", StringComparison.Ordinal))
				{
					Data[file.Key] = file.Value;
				}
				else
				{
					OtherFiles[file.Key] = file.Value;
				}
			}
		}
	}

	public void ParseFiles(int gameAreaChangeCount)
	{
		if (AllFiles != null)
		{
			LoadedInThisArea = new Dictionary<string, FileInformation>(1024);
			ParseFiles(AllFiles);
			this.LoadedFiles?.Invoke(this, LoadedInThisArea);
		}
	}

	public long FindFile(string name)
	{
		try
		{
			if (AllFiles.TryGetValue(name, out var value))
			{
				return value.Ptr;
			}
		}
		catch (KeyNotFoundException)
		{
			MessageBox.Show($"Couldn't find the file in memory: {name}\nTry to restart the game.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			Environment.Exit(1);
		}
		return 0L;
	}
}
