using System;
using System.Linq;
using BaseLibrary.Input;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BookLibrary.UI;

public class UIEntryItem_Recipe : UIEntryItem
{
	private struct GroupData(int group)
	{
		public readonly int Group = group;
		public int CurrentIndex;
	}

	private const int SlotSize = 48;
	private const int ResultSlotSize = 80;

	private readonly UIItem[] items;
	private readonly GroupData[] groups;

	private int groupAnimationTimer;
	private UIItem result;
	private Vector2 tileSize;

	// NOTE: maybe fallback to drawing the item if tile rendering fails
	// NOTE: (?) show if player has the required items

	public UIEntryItem_Recipe(BookEntryItem_Recipe entry)
	{
		Recipe recipe = entry.Recipe;

		result = new UIItem(recipe.createItem) {
			Size = Dimension.FromPixels(ResultSlotSize)
		};
		base.Add(result);

		items = new UIItem[recipe.requiredItem.Count];
		groups = new GroupData[recipe.requiredItem.Count];

		for (int i = 0; i < recipe.requiredItem.Count; i++)
		{
			Item item = recipe.requiredItem[i];

			groups[i] = new GroupData(recipe.acceptedGroups.FirstOrDefault(groupID => RecipeGroup.recipeGroups[groupID].ContainsItem(item.type), -1));

			UIItem ingredient = new UIItem(item) {
				Size = Dimension.FromPixels(SlotSize)
			};
			items[i] = ingredient;
			base.Add(ingredient);
		}

		for (int i = 0; i < recipe.requiredTile.Count; i++)
		{
			UITile requiredTile = new UITile(recipe.requiredTile[i]) {
				Position = Dimension.FromPixels(ResultSlotSize + 16 + i * (24 + 4), 0),
				Size = Dimension.FromPixels(24)
			};
			base.Add(requiredTile);
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
		// TODO: this will have issue with too many ingredients - UIGrid or s/e
		for (int i = 0; i < items.Length; i++)
		{
			UIItem item = items[i];

			item.Position.PixelsX = ResultSlotSize + 16 + i * (SlotSize + 4);
			item.Position.PixelsY = ResultSlotSize - SlotSize;
			item.Recalculate();
		}

		Size.PixelsY = ResultSlotSize;

		base.Recalculate();
	}
}