using System;
using BaseLibrary;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

namespace BookLibrary.UI;

public class UIItem : BaseElement
{
	private static Asset<Texture2D> BorderTexture;

	public Item Item;

	public UIItem(Item item)
	{
		BorderTexture ??= Main.Assets.Request<Texture2D>("Images/UI/Achievement_Borders");

		Item = item;
		Padding = new Padding(6);
	}

	protected void DrawItem(SpriteBatch spriteBatch, Item item, float scale)
	{
		Vector2 position = Dimensions.TopLeft() + Dimensions.Size() * 0.5f;

		ItemSlot.DrawItemIcon(item, 0, spriteBatch, position, 1f, Math.Min(InnerDimensions.Width - 4, InnerDimensions.Height - 4), Color.White);

		if (item.stack > 1)
		{
			string text = item.stack.ToString();
			float texscale = 0.75f;

			Vector2 textPos = InnerDimensions.BottomRight() - FontAssets.MouseText.Value.MeasureString(text) * texscale + new Vector2(-4f, 6f) * texscale;
			ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.MouseText.Value, text, textPos, Color.White, 0f, Vector2.Zero, new Vector2(texscale));
		}

		if (IsMouseHovering)
		{
			Main.LocalPlayer.cursorItemIconEnabled = false;
			Main.ItemIconCacheUpdate(0);
			Main.HoverItem = item.Clone();
			Main.hoverItemName = Main.HoverItem.Name;
			
			/*HoverItem = recipe[availableRecipe[focusRecipe]].requiredItem[i].Clone();
			ItemSlot.MouseHover(22);
			hoverItemName = recipe[availableRecipe[focusRecipe]].requiredItem[i].Name;
			if (recipe[availableRecipe[focusRecipe]].ProcessGroupsForText(recipe[availableRecipe[focusRecipe]].requiredItem[i].type, out var theText))
				HoverItem.SetNameOverride(theText);

			if (recipe[availableRecipe[focusRecipe]].anyIronBar && recipe[availableRecipe[focusRecipe]].requiredItem[i].type == 22)
				HoverItem.SetNameOverride(Lang.misc[37].Value + " " + Lang.GetItemNameValue(22));
			else if (recipe[availableRecipe[focusRecipe]].anyWood && recipe[availableRecipe[focusRecipe]].requiredItem[i].type == 9)
				HoverItem.SetNameOverride(Lang.misc[37].Value + " " + Lang.GetItemNameValue(9));
			else if (recipe[availableRecipe[focusRecipe]].anySand && recipe[availableRecipe[focusRecipe]].requiredItem[i].type == 169)
				HoverItem.SetNameOverride(Lang.misc[37].Value + " " + Lang.GetItemNameValue(169));
			else if (recipe[availableRecipe[focusRecipe]].anyFragment && recipe[availableRecipe[focusRecipe]].requiredItem[i].type == 3458)
				HoverItem.SetNameOverride(Lang.misc[37].Value + " " + Lang.misc[51].Value);
			else if (recipe[availableRecipe[focusRecipe]].anyPressurePlate && recipe[availableRecipe[focusRecipe]].requiredItem[i].type == 542)
				HoverItem.SetNameOverride(Lang.misc[37].Value + " " + Lang.misc[38].Value);

			if (recipe[availableRecipe[focusRecipe]].requiredItem[i].stack > 1)
				hoverItemName = hoverItemName + " (" + recipe[availableRecipe[focusRecipe]].requiredItem[i].stack + ")";*/
		}
	}

	protected override void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Draw(TextureAssets.MagicPixel.Value, InnerDimensions, new Color(40, 25, 14, 100));

		Vector2 position = InnerDimensions.TopLeft();
		Vector2 textureSize = InnerDimensions.Size();

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

		if (!Item.IsAir) DrawItem(spriteBatch, Item, 1f);
	}
}