using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace BookLibrary;

public class BookLibraryConfig : ModConfig
{
	public override ConfigScope Mode => ConfigScope.ClientSide;

	[Header("UI")] [DefaultValue(true)] public bool RecipeTileRendering;
}