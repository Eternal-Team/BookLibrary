using BaseLibrary.UI;
using Microsoft.Xna.Framework;

namespace BookLibrary.UI;

public class UIEntryItem_Text : UIEntryItem
{
	private UIText text;

	public UIEntryItem_Text(BookEntryItem_Text entry)
	{
		text = new UIText(entry.Text) {
			Size = Dimension.FromPercent(100),
			Padding = new Padding(4),
			Settings = {
				TextColor = BookUI.TextColor,
				BorderColor = Color.Transparent
			}
		};
		base.Add(text);
	}

	public override void Recalculate()
	{
		Size.PixelsY = (int)text.TotalHeight;

		base.Recalculate();
	}
}