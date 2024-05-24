using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BookLibrary.UI;

// Note: right side could be used for mod description
public class UIPageBook : BaseElement
{
	private readonly UIText textBookName;
	private readonly UIGrid<UIBookCategory> grid;

	public UIPageBook()
	{
		textBookName = new UIText(LocalizedText.Empty, 1.1f)
		{
			Size = new Dimension(0, 30, 100, 0),
			Settings = { HorizontalAlignment = HorizontalAlignment.Center }
		};

		grid = new UIGrid<UIBookCategory>
		{
			Size = new Dimension(0, -48, 100, 100),
			Position = Dimension.FromPixels(0, 48)
		};

		BaseElement pageLeft = new()
		{
			Size = Dimension.FromPixels(420, 685),
			Children =
			{
				textBookName,
				new UITexture(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Separator1"))
				{
					Size = new Dimension(0, 4, 100, 0),
					Position = Dimension.FromPixels(0, 38),
					Settings = { ScaleMode = ScaleMode.Stretch, Color = new Color(138, 89, 45) }
				},
				grid
			}
		};
		base.Add(pageLeft);
	}

	public void SetBook(ModBook book)
	{
		textBookName.Text = book.GetLocalization("Name");

		grid.Clear();
		foreach (BookCategory category in book.Categories)
		{
			grid.Add(new UIBookCategory(category) { Size = new Dimension(0, 64, 100, 0) }.AddOnClick(args =>
			{
				BookUI.Instance.PushPage(BookUI.Instance.uiCategory);
				BookUI.Instance.uiCategory.SetCategory(category);

				args.Handled = true;
			}));
		}
	}
}