using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BookLibrary;

public class BookCategory
{
	public ModBook Mod;
	public string Name;
	public string Texture = BaseLibrary.BaseLibrary.PlaceholderTexture;

	public LocalizedText DisplayName => Mod.GetLocalization($"Category.{Name}");

	public List<BookEntry> Items = [];
}

public class BookEntry
{
	public ModBook Mod;
	public string Name;
	public string Texture = BaseLibrary.BaseLibrary.PlaceholderTexture;

	public LocalizedText DisplayName => Mod.GetLocalization($"Entry.{Name}");

	public List<BookEntryItem> Items = [];
}

public abstract class BookEntryItem
{
}

public class BookEntryItem_Text(string text) : BookEntryItem
{
	public readonly string Text = text;
}

public class BookEntryItem_Image(string path) : BookEntryItem
{
	public readonly string Path = path;
}

public class BookEntryItem_Video(string path) : BookEntryItem
{
	public readonly string Path = path;
}

// NOTE: this might be problematic, how do you select the right recipe? (mainly a problem if books are generated from files)
public class BookEntryItem_Recipe(Recipe recipe) : BookEntryItem
{
	public readonly Recipe Recipe = recipe;
}