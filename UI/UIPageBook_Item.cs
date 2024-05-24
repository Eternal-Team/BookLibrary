using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace BookLibrary.UI;

public class UIBookCategory : UIPanel
{
	private readonly BookCategory category;
	private readonly UITexture icon;
	private readonly UIText name;
	
	public UIBookCategory(BookCategory category)
	{
		this.category = category;
		
		Settings.BorderColor = Color.Transparent;
		Settings.BackgroundColor = new Color(0, 0, 0, 100);

		icon = new UITexture(ModContent.Request<Texture2D>(this.category.Texture))
		{
			Settings = { ScaleMode = ScaleMode.Stretch }
		};

		base.Add(icon);

		name = new UIText(category.DisplayName)
		{
			Settings = { VerticalAlignment = VerticalAlignment.Center }
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