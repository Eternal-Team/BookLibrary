using BaseLibrary.UI;
using Microsoft.Xna.Framework.Media;
using ReLogic.Content;
using Terraria.ModLoader;

namespace BookLibrary.UI;

public class UIEntryItem_Video : UIEntryItem
{
	private readonly UIVideo video;

	public UIEntryItem_Video(BookEntryItem_Video entry)
	{
		Asset<Video> asset = ModContent.Request<Video>(entry.Path, AssetRequestMode.ImmediateLoad);

		video = new UIVideo(asset) {
			Size = Dimension.FromPercent(100)
		};
		base.Add(video);
	}

	public override void Recalculate()
	{
		Size.PercentX = 0;
		Size.PercentY = 0;

		Size.PixelsX = video.Dimensions.Width;
		Size.PixelsY = video.Dimensions.Height;

		base.Recalculate();
	}
}