using BaseLibrary.UI;
using Terraria;
using Terraria.ModLoader;

namespace BookLibrary;

public class BookLibrary : Mod
{
	public override void Load()
	{
		if (!Main.dedServ)
		{
			UISystem.UILayer.Add(new BookUI());
		}
	}
}

// public class test : ModSystem
// {
// 	private float rotation;
//
// 	public static Texture2D GetTexturePremultiplied(string path)
// 	{
// 		Texture2D texture = ModContent.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad).Value;
// 		Color[] buffer = new Color[texture.Width * texture.Height];
// 		texture.GetData(buffer);
// 		for (int i = 0; i < buffer.Length; i++) buffer[i] = Color.FromNonPremultiplied(buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
// 		texture.SetData(buffer);
// 		return texture;
// 	}
//
// 	private Texture2D ass;
//
// 	public override void Load()
// 	{
// 		Main.QueueMainThreadAction(() => ass = GetTexturePremultiplied("BookLibrary/effect"));
// 	}
//
// 	public override void PostDrawInterface(SpriteBatch spriteBatch)
// 	{
// 		Color c = new Color(53, 225, 252);
// 		float scale = 1f + MathF.Sin(rotation * 3f) * 0.2f;
// 		spriteBatch.Draw(ass, new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f, null, c * 0.7f, rotation += 0.01f, new Vector2(256f), new Vector2(2f) * scale, SpriteEffects.None, 0f);
// 		spriteBatch.Draw(ass, new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f, null, c * 0.3f, -rotation, new Vector2(256f), new Vector2(1.75f), SpriteEffects.None, 0f);
// 	}
// }