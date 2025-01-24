using System.Collections.Generic;
using BaseLibrary.UI;
using BookLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace BookLibrary;

public class PortableStorageBook : ModBook
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

public class BookUI : UIPanel
{
	public static BookUI Instance = null!;
	private static readonly Dimension BookSize = Dimension.FromPixels(1010, 740);
	private static readonly Dimension WrapperSize = Dimension.FromPixels(910, 685);
	private static readonly Dimension WrapperPosition = Dimension.FromPixels(45, 25);
	internal static readonly Color TextColor = new Color(40, 25, 14);

	internal readonly UIPageMain uiMain;
	internal readonly UIPageBook uiBook;
	internal readonly UIPageCategory uiCategory;

	internal readonly Stack<BaseElement> lastPages = [];
	internal BaseElement currentElement;

	public BookUI()
	{
		Instance = this;

		Size = BookSize;
		Position = Dimension.FromPercent(50);
		Display = Display.None;
		Settings.Texture = ModContent.Request<Texture2D>("BookLibrary/Assets/Textures/BookBackground");

		UITexture textureReturn = new(ModContent.Request<Texture2D>("BookLibrary/Assets/Textures/ReturnButton")) {
			Size = Dimension.FromPixels(30),
			Position = Dimension.FromPixels(28, 12),
			Settings = { ScaleMode = ScaleMode.Stretch, SamplerState = SamplerState.PointClamp }
		};
		textureReturn.OnMouseDown += args => {
			if (lastPages.TryPop(out BaseElement? element))
			{
				element.Display = Display.Visible;
				if (currentElement != null) currentElement.Display = Display.None;
				currentElement = element;
			}
			else
			{
				Display = Display.None;
				currentElement = uiMain!;
			}

			args.Handled = true;
		};
		base.Add(textureReturn);

		currentElement = uiMain = new UIPageMain {
			Size = WrapperSize,
			Position = WrapperPosition,
		};
		base.Add(uiMain);

		uiBook = new UIPageBook {
			Size = WrapperSize,
			Position = WrapperPosition,
			Display = Display.None
		};
		base.Add(uiBook);

		uiCategory = new UIPageCategory {
			Size = WrapperSize,
			Position = WrapperPosition,
			Display = Display.None
		};
		base.Add(uiCategory);
	}

	public void PushPage(UIBookPage element)
	{
		currentElement.Display = Display.None;
		element.Display = Display.Visible;
		element.OpenPage();

		lastPages.Push(currentElement);
		currentElement = element;
	}
}