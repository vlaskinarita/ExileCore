namespace ExileCore.PoEMemory.FilesInMemory;

public class WordEntry : RemoteMemoryObject
{
	private string _text;

	public string Text => _text ?? (_text = base.M.ReadStringU(base.M.Read<long>(base.Address + 4)));

	public override string ToString()
	{
		return Text;
	}
}
