using Terraria;
using Terraria.ID;

namespace BookLibrary;

public class BookTestBook : ModBook
{
	public override void SetStaticDefaults()
	{
		Recipe? zenith = null;

		for (int i = 0; i < Recipe.numRecipes; i++)
		{
			Recipe recipe = Main.recipe[i];

			if (recipe.HasResult(ItemID.CopperPickaxe))
			{
				zenith = recipe;
			}
			else if (recipe.HasResult(ItemID.CookingPot))
			{
			}
		}

		BookCategory sampleCategory = new BookCategory {
			Name = "Tests",
			Items = {
				new BookEntry {
					Name = "RecipeTest",
					Mod = this,
					Items = {
						new BookEntryItem_Text("Here's a recipe for a cool sword:"),
						new BookEntryItem_Recipe(zenith)
					}
				},
				new BookEntry {
					Name = "VideoTest",
					Mod = this,
					Items = {
						new BookEntryItem_Video("BookLibrary/Assets/TestVideo")
					}
				}
			}
		};
		AddCategory(sampleCategory);
	}
}