using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace BookLibrary.UI;

// TODO: have separate entry items for each type
public class UIEntryItem : BaseElement
{
	private readonly BaseElement element = null!;

	public UIEntryItem(BookEntryItem entryItem)
	{
		Size = new Dimension(0, 20, 100, 0);

		switch (entryItem)
		{
			case BookEntryItem_Text text:
			{
				element = new UIText(text.Text) {
					Size = Dimension.FromPercent(100),
					Padding = new Padding(4),
					Settings = {
						TextColor = BookUI.TextColor,
						BorderColor = Color.Transparent
					}
				};
				base.Add(element);

				break;
			}
			case BookEntryItem_Image image:
			{
				Asset<Texture2D> texture = ModContent.Request<Texture2D>(image.Path, AssetRequestMode.ImmediateLoad);
				// Size.PixelsY = texture.Height();

				// BUG: this should account for the border (+12px)
				
				element = new UIBorderedTexture(texture);
				base.Add(element);

				break;
			}
		}
	}

	public override void Recalculate()
	{
		Size.PixelsY = element switch {
			UIText text => (int)text.TotalHeight,
			UITexture texture => texture.Texture?.Height() + 12 ?? 20,
			_ => Size.PixelsY
		};

		base.Recalculate();
	}
}