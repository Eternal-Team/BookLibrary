using System.Collections.Generic;
using BaseLibrary.UI;
using BookLibrary.UI;
using Microsoft.Xna.Framework.Graphics;
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
			Name = "NormalsBags",
			Mod = this
		});
		bookCategory.Items.Add(new BookEntry
		{
			Name = "AmmoPouch",
			Mod = this
		});
		bookCategory.Items.Add(new BookEntry
		{
			Name = "DartHolder",
			Mod = this
		});
		bookCategory.Items.Add(new BookEntry
		{
			Name = "FireproofContainer",
			Mod = this
		});
		bookCategory.Items.Add(new BookEntry
		{
			Name = "Magazine",
			Mod = this
		});
		bookCategory.Items.Add(new BookEntry
		{
			Name = "ThePerfectSolution",
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
						new BookEntryItem_Image(BaseLibrary.BaseLibrary.PlaceholderTexture),
						new BookEntryItem_Text("\"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\""),
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

		uiMain = new UIPageMain
		{
			Size = WrapperSize,
			Position = WrapperPosition,
		};
		Add(uiMain);
		currentElement = uiMain;

		uiBook = new UIPageBook
		{
			Size = WrapperSize,
			Position = WrapperPosition,
			Display = Display.None
		};
		Add(uiBook);

		uiCategory = new UIPageCategory
		{
			Size = WrapperSize,
			Position = WrapperPosition,
			Display = Display.None
		};
		Add(uiCategory);
	}

	public void PushPage(BaseElement element)
	{
		currentElement.Display = Display.None;
		element.Display = Display.Visible;

		lastPages.Push(currentElement);
		currentElement = element;
	}
}