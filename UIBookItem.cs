using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace BookLibrary;

public class UIBookItem : UIPanel
{
	private readonly ModBook _book;
	private readonly UITexture icon;
	private readonly UIText name;

	public UIBookItem(ModBook book)
	{
		_book = book;
		Settings.BorderColor = Color.Transparent;
		Settings.BackgroundColor = new Color(0, 0, 0, 100);

		icon = new UITexture(ModContent.Request<Texture2D>(book.Texture))
		{
			Settings = { ScaleMode = ScaleMode.Stretch }
		};

		Add(icon);

		name = new UIText(book.DisplayName)
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