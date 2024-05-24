using BaseLibrary.UI;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace BookLibrary.UI;

public class UIEntryItem : BaseElement
{
	private readonly BaseElement element;

	public UIEntryItem(BookEntryItem entryItem)
	{
		// TODO: Y dimensions should be based on the UIText/UITexture/...

		switch (entryItem)
		{
			case BookEntryItem_Text text:
			{
				element = new UIText(text.Text)
				{
					Size = Dimension.FromPercent(100),
					Padding = new Padding(4)
				};

				base.Add(element);
				break;
			}
			case BookEntryItem_Image image:
			{
				Asset<Texture2D> texture = ModContent.Request<Texture2D>(image.TexturePath, AssetRequestMode.ImmediateLoad);
				Size.PixelsY = texture.Height();

				element = new UITexture(texture) { Size = Dimension.FromPercent(100) };
				base.Add(element);

				break;
			}
		}
	}

	public override void Recalculate()
	{
		Size.PixelsY = element switch
		{
			UIText text => (int)text.TotalHeight,
			UITexture texture => texture.Texture?.Height() ?? 20,
			_ => Size.PixelsY
		};

		base.Recalculate();
	}
}