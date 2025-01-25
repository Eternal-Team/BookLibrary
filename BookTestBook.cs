using Terraria;
using Terraria.ID;

namespace BookLibrary;

public class BookTestBook : ModBook
{
	public override void SetStaticDefaults()
	{
		Recipe? zenith = null, cookingpot = null;

		for (int i = 0; i < Recipe.numRecipes; i++)
		{
			Recipe recipe = Main.recipe[i];

			if (recipe.HasResult(ItemID.CopperPickaxe))
			{
				zenith = recipe;
			}
			else if (recipe.HasResult(ItemID.CookingPot))
			{
				cookingpot = recipe;
			}
		}

		BookCategory bookCategory = new() { Name = "Items" };
		bookCategory.Items.Add(new BookEntry {
			Name = "NormalBags",
			Mod = this,
			Items = {
				new BookEntryItem_Text("Here's a recipe for a cool sword:"),
				new BookEntryItem_Recipe(zenith),
				// new BookEntryItem_Text("But here is an even cooler recipe, those who own the mighty Cooking Pot can call themselves true Terrarians!"),
				// new BookEntryItem_Recipe(cookingpot)
			}
		});
		bookCategory.Items.Add(new BookEntry {
			Name = "AmmoPouch",
			Mod = this
		});
		bookCategory.Items.Add(new BookEntry {
			Name = "DartHolder",
			Mod = this
		});
		bookCategory.Items.Add(new BookEntry {
			Name = "FireproofContainer",
			Mod = this
		});
		bookCategory.Items.Add(new BookEntry {
			Name = "Magazine",
			Mod = this
		});
		bookCategory.Items.Add(new BookEntry {
			Name = "ThePerfectSolution",
			Mod = this
		});
		bookCategory.Items.Add(new BookEntry {
			Name = "Wallet",
			Mod = this
		});
		AddCategory(bookCategory);
		AddCategory(new BookCategory {
			Name = "Mechanics",
			Items = {
				new BookEntry {
					Name = "Crafting",
					Mod = this,
					Items = {
						new BookEntryItem_Text("\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\""),
						new BookEntryItem_Image(BaseLibrary.BaseLibrary.PlaceholderTexture),
						new BookEntryItem_Text("\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\""),
						new BookEntryItem_Text("\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\""),
						new BookEntryItem_Text("\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\""),
						new BookEntryItem_Text("\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\""),
						new BookEntryItem_Text("\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\""),
						new BookEntryItem_Text("\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\""),
					}
				}
			}
		});
	}
}