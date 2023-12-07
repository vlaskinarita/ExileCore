using System;
using ExileCore.Shared.Helpers;

namespace ExileCore.PoEMemory.Elements;

public class DelveCellInfoStrings : RemoteMemoryObject
{
	private bool _interesting;

	private string _testString;

	private string _testString2;

	private string _testString3;

	private string _testString4;

	private string _testString5;

	private string _testStringGood;

	public string TestString
	{
		get
		{
			string obj = _testString ?? base.M.ReadStringU(base.M.Read<long>(base.Address));
			string result = obj;
			_testString = obj;
			return result;
		}
	}

	public string TestStringGood
	{
		get
		{
			string obj = _testStringGood ?? _testString.InsertBeforeUpperCase(Environment.NewLine);
			string result = obj;
			_testStringGood = obj;
			return result;
		}
	}

	public string TestString2
	{
		get
		{
			string obj = _testString2 ?? base.M.ReadStringU(base.M.Read<long>(base.Address + 8));
			string result = obj;
			_testString2 = obj;
			return result;
		}
	}

	public string TestString3
	{
		get
		{
			string obj = _testString3 ?? base.M.ReadStringU(base.M.Read<long>(base.Address + 64));
			string result = obj;
			_testString3 = obj;
			return result;
		}
	}

	public string TestString4
	{
		get
		{
			string obj = _testString4 ?? base.M.ReadStringU(base.M.Read<long>(base.Address + 88));
			string result = obj;
			_testString4 = obj;
			return result;
		}
	}

	public string TestString5
	{
		get
		{
			string testString = _testString5;
			if (testString != null)
			{
				return testString;
			}
			_testString5 = base.M.ReadStringU(base.M.Read<long>(base.Address + 96));
			return _testString5;
		}
	}

	public bool Interesting
	{
		get
		{
			if (_testString5 == null)
			{
				string testString = TestString5;
				if (testString.Length > 1 && !testString.EndsWith("Azurite") && !TestString.StartsWith("Azurite3") && !testString.EndsWith("Weapons") && !testString.EndsWith("Armour") && !testString.EndsWith("Jewellery") && !testString.EndsWith("Items"))
				{
					_interesting = true;
				}
				else if (TestString.StartsWith("Obstruction"))
				{
					_interesting = true;
				}
			}
			return _interesting;
		}
	}
}
