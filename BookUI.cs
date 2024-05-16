using System.Collections.Generic;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace BookLibrary;

public class PortableStorageBook : ModBook
{
	public override void SetStaticDefaults()
	{
		AddCategory(new BookCategory { Name = "Items" });
	}
}

public class OtherBook : ModBook
{
	public override void SetStaticDefaults()
	{
		AddCategory(new BookCategory { Name = "Items" });
	}
}

public static class BookLoader
{
	internal static readonly IList<ModBook> items = new List<ModBook>();
	public static int ItemCount { get; private set; }

	internal static int Register(ModBook item)
	{
		items.Add(item);
		return ItemCount++;
	}
}

// Items, Mechanics, Tiles, ...
public class BookCategory
{
	public List<BookEntry> Items = [];
	public string Name;
	public string Texture = BaseLibrary.BaseLibrary.PlaceholderTexture;
}

// Adventurer Bag, Ammo Bag, ...
public class BookEntry
{
	public List<BookEntryItem> Items = [];
	public string Name;
	public string Texture = BaseLibrary.BaseLibrary.PlaceholderTexture;
}

// text, image, recipe, ...
public abstract class BookEntryItem
{
}

public abstract class ModBook : ModType, ILocalizedModType
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
		ModTypeLookup<ModBook>.Register(this);
		BookLoader.Register(this);
	}

	public sealed override void SetupContent()
	{
		SetStaticDefaults();
	}
}

// TODO: SFX
// TODO: Different font
// TODO: hovering animations
// TODO: video support - 'Xna.VideoPlayer' https://rbwhitaker.com/tutorials/xna/advanced/video-playback/
public class BookUI : UIPanel
{
	public static BookUI Instance = null!;
	private static readonly Dimension BookSize = Dimension.FromPixels(1010, 740);
	private static readonly Dimension WrapperSize = Dimension.FromPixels(910, 685);
	private static readonly Dimension WrapperPosition = Dimension.FromPixels(45, 25);
	private readonly UIModBook uiBook;
	private readonly BaseElement uiCategory;
	private readonly BaseElement uiMain;

	private readonly Queue<BaseElement> lastPages = [];
	private BaseElement currentElement;

	public BookUI()
	{
		Instance = this;

		Size = BookSize;
		Position = Dimension.FromPercent(50);
		Display = Display.None;
		Settings.Texture = ModContent.Request<Texture2D>("BookLibrary/Assets/Textures/BookBackground");

		UITexture textureReturn = new(ModContent.Request<Texture2D>("BookLibrary/Assets/Textures/ReturnButton"))
		{
			Size = Dimension.FromPixels(30),
			Position = Dimension.FromPixels(28, 12),
			Settings = { ScaleMode = ScaleMode.Stretch, SamplerState = SamplerState.PointClamp }
		};
		textureReturn.OnClick += args =>
		{
			if (lastPages.TryDequeue(out BaseElement? element))
			{
				element.Display = Display.Visible;
				if (currentElement != null) currentElement.Display = Display.None;
			}
			else
			{
				Display = Display.None;
			}

			args.Handled = true;
		};
		Add(textureReturn);

		uiMain = SetupMainPage();
		Add(uiMain);

		uiBook = new UIModBook
		{
			Size = WrapperSize,
			Position = WrapperPosition,
			Display = Display.None
		};

		Add(uiBook);
	}

	public void SetMainPage()
	{
		uiMain.Display = Display.Visible;
		uiBook.Display = Display.None;
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
				new UIText(ModContent.GetInstance<BookLibrary>().GetLocalization("UI.BookDescription"))
				{
					Size = new Dimension(0, -40, 100, 100),
					Position = Dimension.FromPixels(0, 40),
					Settings = { HorizontalAlignment = HorizontalAlignment.Center, VerticalAlignment = VerticalAlignment.Center }
				}
			}
		};

		wrapper.Add(pageRight);

		UIGrid<UIBookItem> grid = new()
		{
			Size = new Dimension(0, -48, 100, 100),
			Position = Dimension.FromPixels(0, 48)
		};

		pageLeft.Add(grid);

		foreach (ModBook modBook in BookLoader.items)
		{
			grid.Add(new UIBookItem(modBook) { Size = new Dimension(0, 64, 100, 0) }.AddOnClick(args =>
			{
				uiMain.Display = Display.None;
				uiBook.Display = Display.Visible;
				uiBook.SetBook(modBook);

				lastPages.Enqueue(uiMain);
				currentElement = uiBook;

				args.Handled = true;
			}));
		}

		return wrapper;
	}
}