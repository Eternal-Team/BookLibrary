using BaseLibrary.UI;
using Terraria;
using Terraria.ModLoader;

namespace BookLibrary;

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