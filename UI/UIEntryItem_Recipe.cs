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
		public readonly int Group = group;
		public int CurrentIndex;
	}

	private const int SlotSize = 48;

	private readonly UIItem[] items;
	private readonly GroupData[] groups;
	private readonly ScreenTarget? target;

	private Recipe recipe;
	private int groupAnimationTimer;
	private UIItem result;

	// NOTE: maybe fallback to drawing the item if tile rendering fails
	// NOTE: (?) show if player has the required items
	// TODO: quantity location is too bottom-right for result
	// TODO: result should be larger (icon not slot)
	// TODO: rethink the position of UI elements (see https://discord.com/channels/103110554649894912/711551818194485259/1397871375661928448)

	public UIEntryItem_Recipe(BookEntryItem_Recipe entry)
	{
		recipe = entry.Recipe;

		result = new UIItem(entry.Recipe.createItem) {
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

		if (recipe.requiredTile.Count > 0)
		{
			// TODO: only render if the recipe is visible
			target = new ScreenTarget(sb => {
				Main.graphics.GraphicsDevice.Clear(Color.Transparent);

				sb.End();
				sb.Begin();

				CustomTileRenderer.PerformAction(() => {
					float offset = 0f;
					foreach (int tileID in recipe.requiredTile)
					{
						CustomTileRenderer.PlaceAndDrawTile(sb, tileID, new Vector2(offset, 0f), out Vector2 size);
						offset += size.X + 16f;
					}
				});
			}, () => true, 0f, _ => new Vector2(256f));
		}
	}

	protected override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);

		if (target is not null)
		{
			// TODO: resize the result so they are all the same size
			spriteBatch.Draw(target.RenderTarget, result.OuterDimensions.Right() + new Vector2(32f, 0f), Color.White);
		}
	}

	protected override void Update(GameTime gameTime)
	{
		if (!KeyboardInput.IsKeyDown(Keys.LeftShift) && ++groupAnimationTimer >= 30)
		{
			groupAnimationTimer = 0;

			for (int i = 0; i < groups.Length; i++)
			{
				ref GroupData data = ref groups[i];
				if (data.Group == -1) continue;

				RecipeGroup recipeGroup = RecipeGroup.recipeGroups[data.Group];
				if (++data.CurrentIndex >= recipeGroup.ValidItems.Count)
					data.CurrentIndex = 0;

				items[i].Item = new Item(recipeGroup.ValidItems.ElementAt(data.CurrentIndex)) { stack = items[i].Item.stack };
				items[i].Item.SetNameOverride(recipeGroup.GetText());
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