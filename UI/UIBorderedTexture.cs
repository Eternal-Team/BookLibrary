using System;
using BaseLibrary;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace BookLibrary.UI;

public class UIBorderedTexture : UITexture
{
	private static Asset<Texture2D> BorderTexture;

	public UIBorderedTexture(Asset<Texture2D>? texture) : base(texture)
	{
		BorderTexture ??= Main.Assets.Request<Texture2D>("Images/UI/Achievement_Borders");

		Size = Dimension.FromPercent(100);
		Margin = new Margin(6);
	}

	protected override void Draw(SpriteBatch spriteBatch)
	{
		Texture2D texture = Texture is null ? MissingTexture.Value : Texture.Value;

		RasterizerState rasterizer = new() { CullMode = CullMode.None, ScissorTestEnable = true };

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Settings.SamplerState, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

		Vector2 textureSize = Settings.SourceRectangle?.Size() ?? texture.Size();

		Vector2 scale = Settings.ScaleMode switch {
			ScaleMode.Stretch => new Vector2(Dimensions.Width / textureSize.X, Dimensions.Height / textureSize.Y),
			ScaleMode.Zoom => new Vector2(Math.Min(Dimensions.Width / textureSize.X, Dimensions.Height / textureSize.Y)),
			_ => Vector2.One
		};

		scale *= Settings.Scale;
		textureSize *= scale;

		Vector2 position = new() {
			X = Dimensions.X + Settings.ImagePos.PercentX * Dimensions.Width * 0.01f - Settings.ImagePos.PercentX * (textureSize.X) * 0.01f + Settings.ImagePos.PixelsX,
			Y = Dimensions.Y + Settings.ImagePos.PercentY * Dimensions.Height * 0.01f - Settings.ImagePos.PercentY * (textureSize.Y) * 0.01f + Settings.ImagePos.PixelsY
		};

		spriteBatch.Draw(texture, position, Settings.SourceRectangle, Settings.Color, Settings.Rotation, Settings.Origin, scale, Settings.SpriteEffects, 0f);

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);

		// TODO: this could be generalized for any border type
		
		spriteBatch.Draw(BorderTexture.Value, position.OffsetBy(-6f, -6f), new Rectangle(0, 0, 24, 24), Color.White);
		spriteBatch.Draw(BorderTexture.Value, position.OffsetBy(textureSize.X - 18f, -6f), new Rectangle(48, 0, 24, 24), Color.White);
		spriteBatch.Draw(BorderTexture.Value, position.OffsetBy(-6f, textureSize.Y - 18f), new Rectangle(0, 48, 24, 24), Color.White);
		spriteBatch.Draw(BorderTexture.Value, position.OffsetBy(textureSize.X - 18f, textureSize.Y - 18f), new Rectangle(48, 48, 24, 24), Color.White);

		// BUG: this will have issues for too small texture sizes
		int width = (int)(textureSize.X + 12 - 48);
		int height = (int)(textureSize.Y + 12 - 48);
		spriteBatch.Draw(BorderTexture.Value, new Rectangle((int)position.X + 18, (int)position.Y - 6, width, 6), new Rectangle(36, 0, 2, 6), Color.White);
		spriteBatch.Draw(BorderTexture.Value, new Rectangle((int)position.X + 18, (int)((int)position.Y + textureSize.Y), width, 6), new Rectangle(36, 66, 2, 6), Color.White);

		spriteBatch.Draw(BorderTexture.Value, new Rectangle((int)position.X - 6, (int)position.Y + 18, 6, height), new Rectangle(0, 36, 6, 2), Color.White);
		spriteBatch.Draw(BorderTexture.Value, new Rectangle((int)((int)position.X + textureSize.X), (int)position.Y + 18, 6, height), new Rectangle(66, 36, 6, 2), Color.White);
	}
}