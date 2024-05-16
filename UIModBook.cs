using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace BookLibrary;

public class UIModBook : BaseElement
{
	private readonly UIText textBookName;

	public UIModBook()
	{
		textBookName = new UIText("test")
		{
			Size = Dimension.FromPercent(100),
			Settings = { VerticalAlignment = VerticalAlignment.Center }
		};

		Add(new BaseElement
		{
			Size = Dimension.FromPixels(420, 685),
			Children =
			{
				// note: or keep the same as the main page?
				// this would be a good use for horizontal UIGrid
				new UIPanel
				{
					Size = new Dimension(-40, 40, 100, 0),
					Settings = { BorderColor = Color.Transparent, BackgroundColor = new Color(0, 0, 0, 150) },
					Children = { textBookName }
				}
				// BookUI.Instance.SetMainPage();
				// args.Handled = true;
			}
		});
	}

	public void SetBook(ModBook book)
	{
		textBookName.Text = book.GetLocalization("Name");
	}
}