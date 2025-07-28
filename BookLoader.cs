using System.Collections.Generic;
using Terraria.Localization;
using Terraria.ModLoader;

namespace BookLibrary;

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

public abstract class ModBook : ModType, ILocalizedModType
{
	public string LocalizationCategory => "Books";

	public readonly List<BookCategory> Categories = [];

	public virtual string Texture => BaseLibrary.BaseLibrary.PlaceholderTexture;
	public virtual LocalizedText DisplayName => this.GetLocalization("Name", PrettyPrintName);

	public void AddCategory(BookCategory category)
	{
		category.Mod = this;
		Categories.Add(category);
	}

	public sealed override void Register()
	{
		ModTypeLookup<ModBook>.Register(this);
		BookLoader.Register(this);
	}
	
	public sealed override void SetupContent()
	{
		SetStaticDefaults();
	}
}