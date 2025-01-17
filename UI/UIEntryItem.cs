using BaseLibrary.UI;

namespace BookLibrary.UI;

public abstract class UIEntryItem : BaseElement
{
	private readonly BaseElement element = null!;

	protected UIEntryItem()
	{
		Size = new Dimension(0, 20, 100, 0);
	}
}