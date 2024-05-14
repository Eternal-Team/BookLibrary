using System.Collections.Generic;
using System.Linq;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace BookLibrary;

public class PortableStorageBook : ModBookEntry
{
	public override void SetStaticDefaults()
	{
		AddCategory(new BookCategory { Name = "Items" });

		// will need to kind of compositor for entries
	}
}

public class OtherBook : ModBookEntry
{
	public override void SetStaticDefaults()
	{
		AddCategory(new BookCategory { Name = "Items" });
	}
}

public static class BookLoader
{
	internal static readonly IList<ModBookEntry> items = new List<ModBookEntry>();
	public static int ItemCount { get; private set; }

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
	public List<BookCategory> Categories = [];

	public virtual string Texture => BaseLibrary.BaseLibrary.PlaceholderTexture;
	public string LocalizationCategory => "Book";

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

// TODO: SFX
// TODO: hovering animations
public class BookUI : UIPanel
{
	public static BookUI Instance = null!;

	private static readonly Dimension BookSize = Dimension.FromPixels(1010, 740);
	private static readonly Dimension WrapperSize = Dimension.FromPixels(910, 685);
	private static readonly Dimension WrapperPosition = Dimension.FromPixels(45, 25);

	// NOTE: could also isolate this to classes
	private readonly BaseElement wrapperBook;
	private readonly BaseElement wrapperCategory;
	private readonly BaseElement wrapperMain;

	public BookUI()
	{
		Instance = this;
		Display = Display.None;

		Settings.Texture = ModContent.Request<Texture2D>("BookLibrary/Assets/Textures/BookBackground");
		Size = BookSize;
		Position = Dimension.FromPercent(50);

		wrapperMain = SetupMainPage();
		Add(wrapperMain);

		wrapperBook = SetupBookPage();
		wrapperBook.Display = Display.None;
		Add(wrapperBook);
	}

	private BaseElement SetupMainPage()
	{
		BaseElement wrapper = new()
		{
			Size = WrapperSize,
			Position = WrapperPosition
		};

		BaseElement pageLeft = new()
		{
			Size = Dimension.FromPixels(420, 685),
			Children =
			{
				new UIText(ModContent.GetInstance<BookLibrary>().GetLocalization("UI.BookName"), 1.1f)
				{
					Size = new Dimension(0, 30, 100, 0),
					Settings = { HorizontalAlignment = HorizontalAlignment.Center }
				},
				new UITexture(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Separator1"))
				{
					Size = new Dimension(0, 4, 100, 0),
					Position = Dimension.FromPixels(0, 38),
					Settings = { ScaleMode = ScaleMode.Stretch, Color = new Color(138, 89, 45) }
				}
			}
		};

		wrapper.Add(pageLeft);

		BaseElement pageRight = new()
		{
			Size = Dimension.FromPixels(420, 670),
			Position = Dimension.FromPercent(100, 0),
			Children =
			{
				new UITexture(ModContent.Request<Texture2D>("BookLibrary/Assets/Textures/CloseButton"))
				{
					Size = Dimension.FromPixels(32),
					Position = Dimension.FromPercent(100, 0),
					Settings = { ScaleMode = ScaleMode.Stretch, SamplerState = SamplerState.PointClamp }
				}.AddOnClick(_ => Display = Display.None),
				new UIText(ModContent.GetInstance<BookLibrary>().GetLocalization("UI.BookDescription"))
				{
					Size = new Dimension(0, -40, 100, 100),
					Position = Dimension.FromPixels(0, 40),
					Settings = { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }
				}
			}
		};

		wrapper.Add(pageRight);

		int index = 0;
		int size = 64;
		// todo: actual grid
		foreach (ModBookEntry bookEntry in BookLoader.items)
		{
			UIBookItem item = new(bookEntry)
			{
				Size = new Dimension(0, size, 100, 0),
				Position = Dimension.FromPixels(0, 48 + (size + 8) * index++)
			};

			item.OnClick += args =>
			{
				wrapperMain.Display = Display.None;
				wrapperBook.Display = Display.Visible;
				(wrapperBook.First().First().First() as UIText)!.Text = bookEntry.GetLocalization("Name");
				args.Handled = true;
			};

			pageLeft.Add(item);
		}

		return wrapper;
	}

	private BaseElement SetupBookPage()
	{
		BaseElement wrapper = new()
		{
			Size = WrapperSize,
			Position = WrapperPosition
		};

		wrapper.Add(new BaseElement
		{
			Size = Dimension.FromPixels(420, 685),
			Children =
			{
				// note: or keep the same as the main page?
				// this would be a good use for horizontal UIGrid
				new UIPanel
				{
					Size = new Dimension(-24, 40, 100, 0),
					Settings = { BorderColor = Color.Transparent, BackgroundColor = new Color(0, 0, 0, 150) },
					Children =
					{
						new UIText("test")
						{
							Size = Dimension.FromPercent(100),
							Settings = { VerticalAlignment = VerticalAlignment.Center }
						}
					}
				},
				new UIPanel
				{
					Size = new Dimension(20, 40, 0, 0),
					Position = Dimension.FromPercent(100, 0)
				}.AddOnClick(args =>
				{
					wrapperMain.Display = Display.Visible;
					wrapperBook.Display = Display.None;
					args.Handled = true;
				})
			}
		});

		return wrapper;
	}
}