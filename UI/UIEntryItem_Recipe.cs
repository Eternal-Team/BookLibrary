using System;
using System.Linq;
using BaseLibrary.Input;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;

namespace BookLibrary.UI;

public class UIEntryItem_Recipe : UIEntryItem
{
	private struct GroupData(int group)
	{
		public int group = group;
		public int currentIndex;
	}

	private const int SlotSize = 48;

	private Recipe recipe;
	private readonly UIItem[] items;
	private readonly GroupData[] groups;
	private int groupAnimationTimer;

	public UIEntryItem_Recipe(BookEntryItem_Recipe entry)
	{
		recipe = entry.Recipe;

		UIItem result = new UIItem(entry.Recipe.createItem) {
			Size = Dimension.FromPixels(62),
			Position = new Dimension(0, 6, 50, 0)
		};
		base.Add(result);

		UIText text = new UIText("at") {
			Size = Dimension.FromPixels(20, 62),
			Position = new Dimension(48, 6, 50, 0),
			Settings = {
				VerticalAlignment = VerticalAlignment.Center,
				TextColor = BookUI.TextColor,
				BorderColor = Color.Transparent
			}
		};
		base.Add(text);

		items = new UIItem[entry.Recipe.requiredItem.Count];
		groups = new GroupData[entry.Recipe.requiredItem.Count];

		for (int i = 0; i < entry.Recipe.requiredItem.Count; i++)
		{
			Item item = entry.Recipe.requiredItem[i];

			groups[i] = new GroupData(recipe.acceptedGroups.FirstOrDefault(groupID => RecipeGroup.recipeGroups[groupID].ContainsItem(item.type), -1));

			UIItem ingredient = new UIItem(item) {
				Size = Dimension.FromPixels(SlotSize)
			};
			items[i] = ingredient;
			base.Add(ingredient);
		}
	}

	protected override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);

		spriteBatch.End();
		spriteBatch.Begin();

		CustomTileRenderer.PerformAction(() => {
			CustomTileRenderer.PlaceAndDrawTile(spriteBatch, TileID.DemonAltar, InnerDimensions.TopLeft());
			CustomTileRenderer.PlaceAndDrawTile(spriteBatch, TileID.Furnaces, InnerDimensions.TopLeft() + new Vector2(0f, 40f));
			CustomTileRenderer.PlaceAndDrawTile(spriteBatch, TileID.LifeFruit, InnerDimensions.TopLeft() + new Vector2(0f, 80f));
		});

		spriteBatch.End();

		RasterizerState rasterizer = new RasterizerState { CullMode = CullMode.None, ScissorTestEnable = true };
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, rasterizer, null, Main.UIScaleMatrix);
	}

	protected override void Update(GameTime gameTime)
	{
		// Cycle items in groups
		if (!KeyboardInput.IsKeyDown(Keys.LeftShift) && ++groupAnimationTimer >= 30)
		{
			groupAnimationTimer = 0;

			for (int i = 0; i < groups.Length; i++)
			{
				ref GroupData data = ref groups[i];
				if (data.group == -1) continue;

				if (++data.currentIndex >= RecipeGroup.recipeGroups[data.group].ValidItems.Count)
					data.currentIndex = 0;

				items[i].Item = new Item(RecipeGroup.recipeGroups[data.group].ValidItems.ElementAt(data.currentIndex)) { stack = items[i].Item.stack };
			}
		}

		base.Update(gameTime);
	}

	public override void Recalculate()
	{
		int numPerRow = (int)Math.Floor(InnerDimensions.Width / (float)SlotSize) - 1;
		int rows = (int)Math.Ceiling(items.Length / (float)numPerRow);
		int toRender = items.Length;
		int center = InnerDimensions.Width / 2;

		for (int row = 0; row < rows; row++)
		{
			int limit = Math.Min(toRender, numPerRow);
			int width = limit * (SlotSize + 8) - 8;
			for (int i = 0; i < limit; i++)
			{
				toRender--;

				UIItem item = items[row * numPerRow + i];
				item.Position.PixelsX = center - width / 2 + i * (SlotSize + 8);
				item.Position.PixelsY = 76 + row * (SlotSize + 8);
				item.Recalculate();
			}
		}

		Size.PixelsY = 76 + rows * (SlotSize + 8) - 8;

		base.Recalculate();
	}
}