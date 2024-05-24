using System.Collections.Generic;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BookLibrary;

public class PortableStorageBook : ModBook
{
	public override void SetStaticDefaults()
	{
		BookCategory bookCategory = new() { Name = "Items" };
		bookCategory.Items.Add(new BookEntry
		{
			Name = "Normals Bags",
			Mod = this
		});
		bookCategory.Items.Add(new BookEntry
		{
			Name = "Ammo Pouch",
			Mod = this
		});
		bookCategory.Items.Add(new BookEntry
		{
			Name = "Dart Holder",
			Mod = this
		});
		bookCategory.Items.Add(new BookEntry
		{
			Name = "Fireproof Container",
			Mod = this
		});
		bookCategory.Items.Add(new BookEntry
		{
			Name = "Magazine",
			Mod = this
		});
		bookCategory.Items.Add(new BookEntry
		{
			Name = "The Perfect Solution",
			Mod = this
		});
		bookCategory.Items.Add(new BookEntry
		{
			Name = "Wallet",
			Mod = this
		});
		AddCategory(bookCategory);
		AddCategory(new BookCategory
		{
			Name = "Mechanics",
			Items =
			{
				new BookEntry
				{
					Name = "Crafting",
					Mod = this,
					Items =
					{
						new BookEntryItem_Text("\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\""),
						new BookEntryItem_Image(BaseLibrary.BaseLibrary.PlaceholderTexture)
					}
				}
			}
		});
	}
}

public class OtherBook : ModBook
{
	public override void SetStaticDefaults()
	{
		for (int i = 0; i < 20; i++)
		{
			AddCategory(new BookCategory { Name = "Items" + i });
		}
	}
}

public class BookCategory
{
	public virtual LocalizedText DisplayName => Mod.GetLocalization($"Category.{Name}");

	public List<BookEntry> Items = [];
	public ModBook Mod;
	public string Name;
	public string Texture = BaseLibrary.BaseLibrary.PlaceholderTexture;
}

public class BookEntry
{
	public virtual LocalizedText DisplayName => Mod.GetLocalization($"Entry.{Name}");
	public ModBook Mod;

	public List<BookEntryItem> Items = [];
	public string Name;
	public string Texture = BaseLibrary.BaseLibrary.PlaceholderTexture;
}

// text, image, recipe, ...
public abstract class BookEntryItem
{
}

public class BookEntryItem_Text(string text) : BookEntryItem
{
	public readonly string Text = text;
}

public class BookEntryItem_Image(string texturePath) : BookEntryItem
{
	public readonly string TexturePath = texturePath;
}

// TODO: SFX
// TODO: Different font
// TODO: hovering animations
// TODO: video support - 'Xna.VideoPlayer' https://rbwhitaker.com/tutorials/xna/advanced/video-playback/
// https://github.com/Mirsario/TerrariaOverhaul/blob/dev/Core/VideoPlayback/OgvReader.cs
// https://github.com/Mirsario/TerrariaOverhaul/blob/dev/Core/Interface/UIVideo.cs
public class BookUI : UIPanel
{
	public static BookUI Instance = null!;
	private static readonly Dimension BookSize = Dimension.FromPixels(1010, 740);
	private static readonly Dimension WrapperSize = Dimension.FromPixels(910, 685);
	private static readonly Dimension WrapperPosition = Dimension.FromPixels(45, 25);
	internal readonly UIModBook uiBook;
	internal readonly UICategory uiCategory;
	internal readonly BaseElement uiMain;

	internal readonly Stack<BaseElement> lastPages = [];
	internal BaseElement currentElement;

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
		textureReturn.OnMouseDown += args =>
		{
			if (lastPages.TryPop(out BaseElement? element))
			{
				element.Display = Display.Visible;
				if (currentElement != null) currentElement.Display = Display.None;
				currentElement = element;
			}
			else
			{
				Display = Display.None;
				currentElement = uiMain;
			}

			args.Handled = true;
		};
		Add(textureReturn);

		uiMain = SetupMainPage();
		Add(uiMain);
		currentElement = uiMain;

		uiBook = new UIModBook
		{
			Size = WrapperSize,
			Position = WrapperPosition,
			Display = Display.None
		};
		Add(uiBook);

		uiCategory = new UICategory
		{
			Size = WrapperSize,
			Position = WrapperPosition,
			Display = Display.None
		};
		Add(uiCategory);
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
				PushPage(uiBook);
				uiBook.SetBook(modBook);

				args.Handled = true;
			}));
		}

		return wrapper;
	}

	public void PushPage(BaseElement element)
	{
		currentElement!.Display = Display.None;
		element.Display = Display.Visible;
		
		lastPages.Push(currentElement);
		currentElement = element;
	}
}