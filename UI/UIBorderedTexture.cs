using System;
using BaseLibrary;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;

namespace BookLibrary.UI;

public class UIBorderedTexture : UITexture
{
	public UIBorderedTexture(Asset<Texture2D>? texture2) : base(texture2)
	{
		Padding = new Padding(6);
	}

	protected override void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(TextureAssets.MagicPixel.Value, InnerDimensions, new Color(40, 25, 14, 100));

		RasterizerState rasterizer = new RasterizerState { CullMode = CullMode.None, ScissorTestEnable = true };

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Settings.SamplerState, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

		Vector2 textureSize = Settings.SourceRectangle?.Size() ?? Texture.Size();

		Vector2 scale = Settings.ScaleMode switch {
			ScaleMode.Stretch => new Vector2(InnerDimensions.Width / textureSize.X, InnerDimensions.Height / textureSize.Y),
			ScaleMode.Zoom => new Vector2(Math.Min(InnerDimensions.Width / textureSize.X, InnerDimensions.Height / textureSize.Y)),
			_ => Vector2.One
		};

		scale *= Settings.Scale;
		textureSize *= scale;

		Vector2 position = new Vector2 {
			X = InnerDimensions.X + Settings.ImagePos.PercentX * InnerDimensions.Width * 0.01f - Settings.ImagePos.PercentX * textureSize.X * 0.01f + Settings.ImagePos.PixelsX,
			Y = InnerDimensions.Y + Settings.ImagePos.PercentY * InnerDimensions.Height * 0.01f - Settings.ImagePos.PercentY * textureSize.Y * 0.01f + Settings.ImagePos.PixelsY
		};

		spriteBatch.Draw(Texture, position, Settings.SourceRectangle, Settings.Color, Settings.Rotation, Settings.Origin, scale, Settings.SpriteEffects, 0f);

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

		DrawingUtility.DrawAchievementBorder(spriteBatch, Dimensions.TopLeft(), Dimensions.Size());
	}
}