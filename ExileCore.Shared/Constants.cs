using SharpDX;

namespace ExileCore.Shared;

public static class Constants
{
	public const int DefaultMaxStringLength = 256;

	public static readonly Size2F MapIconsSize = new Size2F(14f, 44f);

	public static readonly Size2F MyMapIcons = new Size2F(7f, 8f);

	public static readonly uint[] PlayerXpLevels = new uint[101]
	{
		0u, 0u, 525u, 1760u, 3781u, 7184u, 12186u, 19324u, 29377u, 43181u,
		61693u, 85990u, 117506u, 157384u, 207736u, 269997u, 346462u, 439268u, 551295u, 685171u,
		843709u, 1030734u, 1249629u, 1504995u, 1800847u, 2142652u, 2535122u, 2984677u, 3496798u, 4080655u,
		4742836u, 5490247u, 6334393u, 7283446u, 8348398u, 9541110u, 10874351u, 12361842u, 14018289u, 15859432u,
		17905634u, 20171471u, 22679999u, 25456123u, 28517857u, 31897771u, 35621447u, 39721017u, 44225461u, 49176560u,
		54607467u, 60565335u, 67094245u, 74247659u, 82075627u, 90631041u, 99984974u, 110197515u, 121340161u, 133497202u,
		146749362u, 161191120u, 176922628u, 194049893u, 212684946u, 232956711u, 255001620u, 278952403u, 304972236u, 333233648u,
		363906163u, 397194041u, 433312945u, 472476370u, 514937180u, 560961898u, 610815862u, 664824416u, 723298169u, 786612664u,
		855129128u, 929261318u, 1009443795u, 1096169525u, 1189918242u, 1291270350u, 1400795257u, 1519130326u, 1646943474u, 1784977296u,
		1934009687u, 2094900291u, 2268549086u, 2455921256u, 2658074992u, 2876116901u, 3111280300u, 3364828162u, 3638186694u, 3932818530u,
		4250334444u
	};

	public static readonly uint[] ToraExperienceLevels = new uint[8] { 0u, 770u, 2690u, 9310u, 32230u, 100368u, 308696u, 934584u };

	public static readonly uint[] CatarinaExperienceLevels = new uint[8] { 0u, 770u, 2690u, 9310u, 32230u, 100368u, 308696u, 934584u };

	public static readonly uint[] HakuExperienceLevels = new uint[8] { 0u, 430u, 1640u, 6250u, 23770u, 81288u, 274592u, 913031u };

	public static readonly uint[] VaganExperienceLevels = new uint[8] { 0u, 1050u, 3480u, 11500u, 37960u, 112743u, 330712u, 954947u };

	public static readonly uint[] VoriciExperienceLevels = new uint[8] { 0u, 1050u, 3480u, 11500u, 37960u, 112743u, 330712u, 954947u };

	public static readonly uint[] ElreonExperienceLevels = new uint[8] { 0u, 430u, 1640u, 6250u, 23770u, 81288u, 274592u, 913031u };

	public static readonly uint[] ZanaExperienceLevels = new uint[8] { 0u, 4700u, 13170u, 36870u, 103260u, 289130u, 809570u, 2266810u };

	public static readonly uint[] LeoExperienceLevels = new uint[8] { 0u, 2700u, 7030u, 18270u, 47520u, 111204u, 257016u, 584710u };

	public static int GetLevel(uint[] expLevels, uint currExp)
	{
		for (int i = 1; i < expLevels.Length; i++)
		{
			if (currExp < expLevels[i])
			{
				return i;
			}
		}
		return 8;
	}

	public static uint GetFullCurrentLevel(uint[] expLevels, uint currExp)
	{
		uint num = 0u;
		for (int i = 1; i < expLevels.Length; i++)
		{
			uint num2 = expLevels[i];
			if (currExp < num2)
			{
				return num + num2;
			}
			num += num2;
		}
		return 8u;
	}
}
