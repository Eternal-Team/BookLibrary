using System.Collections.Generic;
using BaseLibrary.UI;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace BookLibrary;

public class PortableStorageBook : ModBookEntry
{
	public override string Texture => "";

	public override void SetStaticDefaults()
	{
		AddCategory(new BookCategory() { Name = "Items" });
		
		// will need to kind of compositor for entries
	}
}

public class OtherBook : ModBookEntry
{
	public override string Texture => "";

	public override void SetStaticDefaults()
	{
		AddCategory(new BookCategory() { Name = "Items" });
	}
}

public static class BookLoader
{
	public static int ItemCount { get; private set; }
	internal static readonly IList<ModBookEntry> items = new List<ModBookEntry>();

	internal static int Register(ModBookEntry item)
	{
		items.Add(item);
		return ItemCount++;
	}
}

public class BookCategory
{
	public string Name;
	public string Texture;
}

public abstract class ModBookEntry : ModType, ILocalizedModType
{
	public string LocalizationCategory => "Book";

	public abstract string Texture { get; }

	public List<BookCategory> Categories = [];

	public void AddCategory(BookCategory category)
	{
		Categories.Add(category);
	}

	protected sealed override void Register()
	{
		ModTypeLookup<ModBookEntry>.Register(this);
		BookLoader.Register(this);
	}

	public sealed override void SetupContent()
	{
		SetStaticDefaults();
	}

	// When you first open the book it should show all registered books

	// Portable Storage (this can replace 'The One Book')
	//    -> Items
	//          -> Adventurer Bag
	//                 -> Icon
	//                 -> Text
	//                 -> Recipe
	//          -> Ammo Bag
	//          -> Builder's Satchel
	//          -> etc.
	//    -> Mechanics
	//          -> Autopickup
	//                 -> Text
	//                 -> Image
}

public class BookUI : UIPanel
{
	public BookUI()
	{
		Settings.Texture = ModContent.Request<Texture2D>("BookLibrary/Assets/Textures/BookBackground");
		Size = Dimension.FromPixels(1010, 740);
		Position = Dimension.FromPercent(50);

		BaseElement leftPage = new()
		{
			Size = Dimension.FromPixels(430, 685),
			Position = Dimension.FromPixels(45, 25),
			Children =
			{
				new UIText(ModContent.GetInstance<BookLibrary>().GetLocalization("UI.BookName"), 1.1f)
				{
					Size = new Dimension(0, 30, 100, 0),
					Settings = { HorizontalAlignment = HorizontalAlignment.Center }
				}
			}
		};

		int index = 0;
		foreach (ModBookEntry bookEntry in BookLoader.items)
		{
			leftPage.Add(new UIText(bookEntry.GetLocalization("Name"))
			{
				Size = new Dimension(0, 30, 100, 0),
				Position = Dimension.FromPixels(0,50+ 38 * index++)
			});
		}

		Children.Add(leftPage);

		Children.Add(new BaseElement
		{
			Size = Dimension.FromPixels(420, 670),
			Position = Dimension.FromPixels(535, 25),
			Children =
			{
				new UITexture(ModContent.Request<Texture2D>("BookLibrary/Assets/Textures/CloseButton"))
				{
					Size = Dimension.FromPixels(40),
					Position = Dimension.FromPercent(100, 0),
					Settings = { ScaleMode = ScaleMode.Stretch, SamplerState = SamplerState.PointClamp }
				}
			}
		});
	}
}