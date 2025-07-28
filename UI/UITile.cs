using System;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;

namespace BookLibrary.UI;

public class UITile : BaseElement
{
	private readonly ScreenTarget target;
	private Vector2 tileSize;
	private int tileType;

	public UITile(int type)
	{
		tileType = type;

		// TODO: if we have a 1000 workbenches we don't need 1000 targets
		// TODO: cycles through alternatives?
		target = new ScreenTarget(DrawTile, () => true, 0f, _ => new Vector2(256f));
	}

	// TODO: only render if visible
	private void DrawTile(SpriteBatch sb)
	{
		Main.graphics.GraphicsDevice.Clear(Color.Transparent);

		sb.End();
		sb.Begin();

		CustomTileRenderer.PerformAction(() => CustomTileRenderer.PlaceAndDrawTile(tileType, Vector2.Zero, out tileSize));
	}

	private static string GetTileName(int tile)
	{
		int requiredTileStyle = Recipe.GetRequiredTileStyle(tile);
		string tileName = Lang.GetMapObjectName(MapHelper.TileToLookup(tile, requiredTileStyle));
		if (tileName == "")
		{
			tileName = tile < TileID.Count ? TileID.Search.GetName(tile) : TileLoader.GetTile(tile).Name;
		}

		return tileName;
	}

	protected override void Draw(SpriteBatch spriteBatch)
	{
		float scale = 1f;
		if (tileSize.X > InnerDimensions.Width || tileSize.Y > InnerDimensions.Height)
			scale = Math.Min(InnerDimensions.Width / tileSize.X, InnerDimensions.Height / tileSize.Y);

		Vector2 center = Dimensions.Center();

		spriteBatch.Draw(target.RenderTarget, center, null, Color.White, 0f, tileSize * 0.5f, scale, SpriteEffects.None, 0f);

		if (IsMouseHovering)
		{
			UICommon.TooltipMouseText(GetTileName(tileType));
		}
	}
}