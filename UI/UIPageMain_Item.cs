using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace BookLibrary.UI;

public class UIModBook : UIPanel
{
	private readonly ModBook book;
	private readonly UITexture icon;
	private readonly UIText name;

	public UIModBook(ModBook book)
	{
		this.book = book;

		Size = new Dimension(0, 64, 100, 0);
		Settings.BorderColor = Color.Transparent;
		Settings.BackgroundColor = new Color(50, 50, 50, 100);

		icon = new UITexture(ModContent.Request<Texture2D>(book.Texture)) {
			Settings = { ScaleMode = ScaleMode.Stretch }
		};

		base.Add(icon);

		name = new UIText(book.DisplayName) {
			Settings = {
				VerticalAlignment = VerticalAlignment.Center,
				TextColor = BookUI.TextColor,
				BorderColor = Color.Transparent
			}
		};

		base.Add(name);
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