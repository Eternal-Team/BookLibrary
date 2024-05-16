using BaseLibrary.UI;
using Terraria;
using Terraria.ModLoader;

namespace BookLibrary;

// NOTE: generate from JSON?
public class BookLibrary : Mod
{
	public override void Load()
	{
		if (!Main.dedServ)
		{
			UISystem.UILayer.Add(new BookUI());
		}
	}
}