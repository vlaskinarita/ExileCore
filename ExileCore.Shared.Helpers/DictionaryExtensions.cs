using System.Collections.Generic;
using System.Linq;

namespace ExileCore.Shared.Helpers;

public static class DictionaryExtensions
{
	public static T MergeLeft<T, TK, TV>(this T me, params IDictionary<TK, TV>[] others) where T : IDictionary<TK, TV>, new()
	{
		T result = new T();
		foreach (IDictionary<TK, TV> item in new List<IDictionary<TK, TV>> { me }.Concat(others))
		{
			foreach (KeyValuePair<TK, TV> item2 in item)
			{
				ref T reference = ref result;
				T val = default(T);
				if (val == null)
				{
					val = reference;
					reference = ref val;
				}
				reference[item2.Key] = item2.Value;
			}
		}
		return result;
	}
}
