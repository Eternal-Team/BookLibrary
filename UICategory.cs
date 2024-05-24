using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace BookLibrary;

public class UICategory : BaseElement
{
	private readonly UIText textCategoryName;
	private readonly UIGrid<UICategoryItem> grid;
	
	public UICategory()
	{
		textCategoryName = new UIText("BookName", 1.1f)
		{
			Size = new Dimension(0, 30, 100, 0),
			Settings = { HorizontalAlignment = HorizontalAlignment.Center }
		};

		BaseElement pageLeft = new()
		{
			Size = Dimension.FromPixels(420, 685),
			Children =
			{
				textCategoryName,
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

	public void SetCategory(BookCategory category)
	{
		textCategoryName.Text = category.DisplayName;

		grid.Clear();
		foreach (BookEntry entry in category.Items)
		{
			grid.Add(new UIBookEntry(entry) { Size = new Dimension(0, 64, 100, 0) });
		}
	}
}

public class UIBookEntry : UIPanel
{
	private readonly BookEntry _entry;
	private readonly UITexture icon;
	private readonly UIText name;
	
	public UIBookEntry(BookEntry entry)
	{
		_entry = entry;
		
		Settings.BorderColor = Color.Transparent;
		Settings.BackgroundColor = new Color(0, 0, 0, 100);

		icon = new UITexture(ModContent.Request<Texture2D>(_entry.Texture))
		{
			Settings = { ScaleMode = ScaleMode.Stretch }
		};

		Add(icon);

		name = new UIText(entry.DisplayName)
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