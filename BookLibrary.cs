using BaseLibrary.UI;
using Terraria;
using Terraria.ModLoader;

namespace BookLibrary;

/*
 * TODO: images could have borders around them
 * TODO: recipe item
 * TODO: fix translations
 * TODO: NPC display
 * TODO: generate from JSON/other format
 */

public class BookLibrary : Mod
{
	internal static BookLibrary Instance = null!;
	
	public override void Load()
	{
		Instance = this;
		
		if (!Main.dedServ)
		{
			UISystem.UILayer.Add(new BookUI());
		}
	}
}