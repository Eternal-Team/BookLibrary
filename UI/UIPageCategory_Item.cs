using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace BookLibrary.UI;

public class UIBookEntry : UIPanel
{
	private readonly BookEntry entry;
	private readonly UITexture icon;
	private readonly UIText name;

	public UIBookEntry(BookEntry entry)
	{
		this.entry = entry;

		Settings.BorderColor = Color.Transparent;
		Settings.BackgroundColor = new Color(0, 0, 0, 100);

		icon = new UITexture(ModContent.Request<Texture2D>(this.entry.Texture))
		{
			Settings = { ScaleMode = ScaleMode.Stretch }
		};

		base.Add(icon);

		name = new UIText(entry.DisplayName)
		{
			Settings =
			{
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