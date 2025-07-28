using Terraria;
using Terraria.ID;

namespace BookLibrary;

public class BookTestBook : ModBook
{
	public override void SetStaticDefaults()
	{
		BookEntry recipeEntry = new BookEntry {
			Name = "RecipeTest",
			Mod = this,
			Items = {
				new BookEntryItem_Text("Here's a recipe for a cool sword:"),
			}
		};

		BookCategory sampleCategory = new BookCategory {
			Name = "Tests",
			Items = {
				new BookEntry {
					Name = "ImageTest",
					Mod = this,
					Items = {
						new BookEntryItem_Text("Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Nulla non lectus sed nisl molestie malesuada. Quisque tincidunt scelerisque libero. Fusce wisi. Curabitur vitae diam non enim vestibulum interdum. Aliquam ante. In enim a arcu imperdiet malesuada. Phasellus rhoncus. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Maecenas ipsum velit, consectetuer eu lobortis ut, dictum at dui. Vivamus ac leo pretium faucibus. Temporibus autem quibusdam et aut officiis debitis aut rerum necessitatibus saepe eveniet ut et voluptates repudiandae sint et molestiae non recusandae. Fusce wisi."),
						new BookEntryItem_Image("Terraria/Images/Misc/MoonExplosion/Head"),
						new BookEntryItem_Text("Pellentesque arcu. Donec quis nibh at felis congue commodo. Integer imperdiet lectus quis justo. Integer tempor. Quisque porta. Aliquam erat volutpat. Cras elementum. Praesent vitae arcu tempor neque lacinia pretium. Nullam lectus justo, vulputate eget mollis sed, tempor sed magna. In laoreet, magna id viverra tincidunt, sem odio bibendum justo, vel imperdiet sapien wisi sed libero. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Donec ipsum massa, ullamcorper in, auctor et, scelerisque sed, est. In rutrum. Curabitur vitae diam non enim vestibulum interdum. Pellentesque sapien. Duis viverra diam non justo."),
						new BookEntryItem_Image(BaseLibrary.BaseLibrary.PlaceholderTexture),
						new BookEntryItem_Image(BaseLibrary.BaseLibrary.PlaceholderTexture),
						new BookEntryItem_Text("Sed ac dolor sit amet purus malesuada congue. Fusce suscipit libero eget elit. Integer in sapien. Praesent id justo in neque elementum ultrices. Proin mattis lacinia justo. Praesent in mauris eu tortor porttitor accumsan. Aenean vel massa quis mauris vehicula lacinia. Suspendisse nisl. Vestibulum fermentum tortor id mi. Maecenas fermentum, sem in pharetra pellentesque, velit turpis volutpat ante, in pharetra metus odio a lectus. Nunc tincidunt ante vitae massa. Fusce aliquam vestibulum ipsum. Pellentesque arcu."),
					}
				},
				recipeEntry,
				new BookEntry {
					Name = "VideoTest",
					Mod = this,
					Items = {
						new BookEntryItem_Video("BookLibrary/Assets/TestVideo")
					}
				}
			}
		};

		for (int i = 0; i < 100; i++)
		{
			int random = Main.rand.Next(0, Recipe.numRecipes);

			recipeEntry.Items.Add(new BookEntryItem_Recipe(Main.recipe[random]));
			recipeEntry.Items.Add(new BookEntryItem_Text("...space..."));
		}


		AddCategory(sampleCategory);
	}
}