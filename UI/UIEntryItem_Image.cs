using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace BookLibrary.UI;

public class UIEntryItem_Image : UIEntryItem
{
	private UIBorderedTexture texture;

	public UIEntryItem_Image(BookEntryItem_Image entry)
	{
		Asset<Texture2D> asset = ModContent.Request<Texture2D>(entry.Path, AssetRequestMode.ImmediateLoad);

		texture = new UIBorderedTexture(asset);
		base.Add(texture);
	}

	public override void Recalculate()
	{
		Size.PixelsY = (texture.Texture?.Height() ?? 40) + texture.Margin.Top + texture.Margin.Bottom;

		base.Recalculate();
	}
}