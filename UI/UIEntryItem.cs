using BaseLibrary.UI;

namespace BookLibrary.UI;

public class UIEntryItem : UIPanel
{
	public UIEntryItem(BookEntryItem entryItem)
	{
		UIText name = new(entryItem.GetType().Name)
		{
			Size = Dimension.FromPercent(100),
			Settings = { VerticalAlignment = VerticalAlignment.Center }
		};

		base.Add(name);
	}
}