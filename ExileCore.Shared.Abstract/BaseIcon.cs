using System;
using System.Collections.Generic;
using System.Numerics;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using ExileCore.Shared.Helpers;
using ExileCore.Shared.Interfaces;
using SharpDX;

namespace ExileCore.Shared.Abstract;

public abstract class BaseIcon
{
	protected static readonly Dictionary<string, Size2> strongboxesUV = new Dictionary<string, Size2>
	{
		{
			"Metadata/Chests/StrongBoxes/Large",
			new Size2(7, 7)
		},
		{
			"Metadata/Chests/StrongBoxes/Strongbox",
			new Size2(1, 2)
		},
		{
			"Metadata/Chests/StrongBoxes/Armory",
			new Size2(2, 1)
		},
		{
			"Metadata/Chests/StrongBoxes/Arsenal",
			new Size2(4, 1)
		},
		{
			"Metadata/Chests/StrongBoxes/Artisan",
			new Size2(3, 1)
		},
		{
			"Metadata/Chests/StrongBoxes/Jeweller",
			new Size2(1, 1)
		},
		{
			"Metadata/Chests/StrongBoxes/Cartographer",
			new Size2(5, 1)
		},
		{
			"Metadata/Chests/StrongBoxes/CartographerLowMaps",
			new Size2(5, 1)
		},
		{
			"Metadata/Chests/StrongBoxes/CartographerMidMaps",
			new Size2(5, 1)
		},
		{
			"Metadata/Chests/StrongBoxes/CartographerHighMaps",
			new Size2(5, 1)
		},
		{
			"Metadata/Chests/StrongBoxes/Ornate",
			new Size2(7, 7)
		},
		{
			"Metadata/Chests/StrongBoxes/Arcanist",
			new Size2(1, 8)
		},
		{
			"Metadata/Chests/StrongBoxes/Gemcutter",
			new Size2(6, 1)
		},
		{
			"Metadata/Chests/StrongBoxes/StrongboxDivination",
			new Size2(7, 1)
		},
		{
			"Metadata/Chests/AbyssChest",
			new Size2(7, 7)
		}
	};

	protected static readonly Dictionary<string, Color> FossilRarity = new Dictionary<string, Color>
	{
		{
			"Fractured",
			Color.Aquamarine
		},
		{
			"Faceted",
			Color.Aquamarine
		},
		{
			"Glyphic",
			Color.Aquamarine
		},
		{
			"Hollow",
			Color.Aquamarine
		},
		{
			"Shuddering",
			Color.Aquamarine
		},
		{
			"Bloodstained",
			Color.Aquamarine
		},
		{
			"Tangled",
			Color.OrangeRed
		},
		{
			"Dense",
			Color.OrangeRed
		},
		{
			"Gilded",
			Color.OrangeRed
		},
		{
			"Sanctified",
			Color.Aquamarine
		},
		{
			"Encrusted",
			Color.Yellow
		},
		{
			"Aetheric",
			Color.Orange
		},
		{
			"Enchanted",
			Color.Orange
		},
		{
			"Pristine",
			Color.Orange
		},
		{
			"Prismatic",
			Color.Orange
		},
		{
			"Corroded",
			Color.Yellow
		},
		{
			"Perfect",
			Color.Orange
		},
		{
			"Jagged",
			Color.Yellow
		},
		{
			"Serrated",
			Color.Yellow
		},
		{
			"Bound",
			Color.Yellow
		},
		{
			"Lucent",
			Color.Yellow
		},
		{
			"Metallic",
			Color.Yellow
		},
		{
			"Scorched",
			Color.Yellow
		},
		{
			"Aberrant",
			Color.Yellow
		},
		{
			"Frigid",
			Color.Yellow
		}
	};

	private readonly ISettings _settings;

	protected bool _HasIngameIcon;

	public bool HasIngameIcon => _HasIngameIcon;

	public Entity Entity { get; }

	[Obsolete]
	public Func<SharpDX.Vector2> GridPosition
	{
		get
		{
			return () => GridPositionNum().ToSharpDx();
		}
		set
		{
			GridPositionNum = () => value().ToVector2Num();
		}
	}

	public Func<System.Numerics.Vector2> GridPositionNum { get; set; }

	public RectangleF DrawRect { get; set; }

	public Func<bool> Show { get; set; }

	public Func<bool> Hidden { get; protected set; } = () => false;


	public HudTexture MainTexture { get; protected set; }

	public IconPriority Priority { get; protected set; }

	public MonsterRarity Rarity { get; protected set; }

	public string Text { get; protected set; }

	public string RenderName => Entity.RenderName;

	public BaseIcon(Entity entity, ISettings settings)
	{
		BaseIcon baseIcon = this;
		_settings = settings;
		Entity = entity;
		if (_settings == null || Entity == null)
		{
			return;
		}
		Rarity = Entity.Rarity;
		Priority = Rarity switch
		{
			MonsterRarity.White => IconPriority.Low, 
			MonsterRarity.Magic => IconPriority.Medium, 
			MonsterRarity.Rare => IconPriority.High, 
			MonsterRarity.Unique => IconPriority.Critical, 
			_ => IconPriority.Critical, 
		};
		Show = () => baseIcon.Entity.IsValid;
		Hidden = () => entity.IsHidden;
		GridPositionNum = () => baseIcon.Entity.GridPosNum;
		if (!Entity.TryGetComponent<MinimapIcon>(out var component))
		{
			return;
		}
		string name = component.Name;
		if (string.IsNullOrEmpty(name))
		{
			return;
		}
		MapIconsIndex mapIconsIndex = Extensions.IconIndexByName(name);
		if (mapIconsIndex != MapIconsIndex.MyPlayer)
		{
			MainTexture = new HudTexture("Icons.png")
			{
				UV = SpriteHelper.GetUV(mapIconsIndex),
				Size = 16f
			};
			_HasIngameIcon = true;
		}
		if (Entity.HasComponent<Portal>() && Entity.TryGetComponent<Transitionable>(out var transitionable))
		{
			Text = RenderName;
			Show = () => baseIcon.Entity.IsValid && transitionable.Flag1 == 2;
		}
		else if (Entity.Path.StartsWith("Metadata/Terrain/Labyrinth/Objects/Puzzle_Parts/Switch", StringComparison.Ordinal))
		{
			Show = delegate
			{
				Transitionable component4 = baseIcon.Entity.GetComponent<Transitionable>();
				MinimapIcon component5 = baseIcon.Entity.GetComponent<MinimapIcon>();
				return baseIcon.Entity.IsValid && component5 != null && component5.IsVisible && !component5.IsHide && component4?.Flag1 != 2;
			};
		}
		else if (Entity.Path.StartsWith("Metadata/MiscellaneousObjects/Abyss/Abyss"))
		{
			Show = delegate
			{
				MinimapIcon component3 = baseIcon.Entity.GetComponent<MinimapIcon>();
				return baseIcon.Entity.IsValid && component3 != null && component3.IsVisible && (!component3.IsHide || baseIcon.Entity.GetComponent<Transitionable>().Flag1 == 1);
			};
		}
		else if (entity.Path.Contains("Metadata/Terrain/Leagues/Delve/Objects/DelveMineral"))
		{
			Priority = IconPriority.High;
			MainTexture.UV = SpriteHelper.GetUV(MapIconsIndex.DelveMineralVein);
			Text = "Sulphite";
			Show = () => entity.IsValid && entity.IsTargetable;
		}
		else
		{
			Show = delegate
			{
				MinimapIcon component2 = baseIcon.Entity.GetComponent<MinimapIcon>();
				return component2 != null && component2.IsVisible && !component2.IsHide;
			};
		}
	}

	protected bool PathCheck(Entity path, params string[] check)
	{
		foreach (string value in check)
		{
			if (path.Path.Equals(value, StringComparison.Ordinal))
			{
				return true;
			}
		}
		return false;
	}
}
