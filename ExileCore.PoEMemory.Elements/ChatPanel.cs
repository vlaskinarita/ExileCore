namespace ExileCore.PoEMemory.Elements;

public class ChatPanel : Element
{
	public Element ChatTitlePanel => ReadObjectAt<Element>(768);

	public Element ChatInputElement => ReadObjectAt<Element>(840);

	public PoeChatElement ChatBox => ReadObjectAt<Element>(816).ReadObjectAt<PoeChatElement>(920);

	public string InputText => ChatInputElement.Text;
}
