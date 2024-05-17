using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BookLibrary;

public class UIModBook : BaseElement
{
	private readonly UIText textBookName;
	private readonly UIGrid<UICategoryItem> grid;
	
	public UIModBook()
	{
		textBookName = new UIText("BookName", 1.1f)
		{
			Size = new Dimension(0, 30, 100, 0),
			Settings = { HorizontalAlignment = HorizontalAlignment.Center }
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
				}
			}
		};
		Add(pageLeft);

		grid = new UIGrid<UICategoryItem>
		{
			Size = new Dimension(0, -48, 100, 100),
			Position = Dimension.FromPixels(0, 48)
		};

		pageLeft.Add(grid);
	}

	public void SetBook(ModBook book)
	{
		textBookName.Text = book.GetLocalization("Name");

		grid.Clear();
		foreach (BookCategory category in book.Categories)
		{
			grid.Add(new UICategoryItem(category) { Size = new Dimension(0, 64, 100, 0) });
		}
	}
}

public class UICategoryItem : UIPanel
{
	private readonly BookCategory _category;
	private readonly UITexture icon;
	private readonly UIText name;
	
	public UICategoryItem(BookCategory category)
	{
		_category = category;
		
		Settings.BorderColor = Color.Transparent;
		Settings.BackgroundColor = new Color(0, 0, 0, 100);

		icon = new UITexture(ModContent.Request<Texture2D>(_category.Texture))
		{
			Settings = { ScaleMode = ScaleMode.Stretch }
		};

		Add(icon);

		name = new UIText(category.DisplayName)
		{
			Settings = { VerticalAlignment = VerticalAlignment.Center }
		};

		Add(name);
	}
	
	public override void Recalculate()
	{
		base.Recalculate();

		icon.Size = Dimension.FromPixels(InnerDimensions.Height);
		name.Size = new Dimension(-(InnerDimensions.Height + 8), 0, 100, 100);
		name.Position = new Dimension(InnerDimensions.Height + 8, 0, 0, 0);
		icon.Recalculate();
		name.Recalculate();
	}
}