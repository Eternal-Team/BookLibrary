using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.ModLoader;

namespace BookLibrary.UI;

public class UIEntryItem_Image : UIEntryItem
{
	private readonly UIBorderedTexture texture;

	public UIEntryItem_Image(BookEntryItem_Image entry)
	{
		Asset<Texture2D> asset = ModContent.Request<Texture2D>(entry.Path, AssetRequestMode.ImmediateLoad);

		texture = new UIBorderedTexture(asset) {
			Settings = { ResizeToContent = true }
		};
		base.Add(texture);
	}

	public override void Recalculate()
	{
		Size.PercentX = 0;
		Size.PercentY = 0;

		Size.PixelsX = texture.Dimensions.Width;
		Size.PixelsY = texture.Dimensions.Height;

		base.Recalculate();
	}
}