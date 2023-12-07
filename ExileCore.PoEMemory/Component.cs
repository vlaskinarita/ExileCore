using System.Reflection;
using System.Text;
using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory;

public class Component : RemoteMemoryObject
{
	public long OwnerAddress => base.M.Read<long>(base.Address + 8);

	public Entity Owner => ReadObject<Entity>(base.Address + 8);

	public string DumpObject()
	{
		PropertyInfo[] properties = GetType().GetProperties();
		StringBuilder stringBuilder = new StringBuilder();
		PropertyInfo[] array = properties;
		foreach (PropertyInfo propertyInfo in array)
		{
			object value = propertyInfo.GetValue(this, null);
			if (value is RemoteMemoryObject)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder3 = stringBuilder2;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(4, 2, stringBuilder2);
				handler.AppendFormatted(propertyInfo.Name);
				handler.AppendLiteral(" => ");
				handler.AppendFormatted<object>(value);
				stringBuilder3.AppendLine(ref handler);
				stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder4 = stringBuilder2;
				handler = new StringBuilder.AppendInterpolatedStringHandler(12, 1, stringBuilder2);
				handler.AppendLiteral("ToString => ");
				handler.AppendFormatted<object>(value.GetType().GetMethod("ToString").Invoke(value, null));
				stringBuilder4.AppendLine(ref handler);
			}
			else
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder5 = stringBuilder2;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(4, 2, stringBuilder2);
				handler.AppendFormatted(propertyInfo.Name);
				handler.AppendLiteral(" => ");
				handler.AppendFormatted<object>(value);
				stringBuilder5.AppendLine(ref handler);
			}
		}
		return stringBuilder.ToString();
	}
}
