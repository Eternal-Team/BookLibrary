using BaseLibrary.Items;
using BaseLibrary.UI;
using Terraria;
using Terraria.ID;

namespace BookLibrary;

public class BookItem : BaseItem
{
	public override void SetDefaults()
	{
		Item.useTime = 30;
		Item.useAnimation = 30;
		Item.useStyle = ItemUseStyleID.RaiseLamp;
		Item.rare = ItemRarityID.White;
	}

	public override bool ConsumeItem(Player player) => false;

	public override bool? UseItem(Player player)
	{
		if (Main.netMode != NetmodeID.Server && player.whoAmI == Main.LocalPlayer.whoAmI)
		{
			BookUI.Instance.Display = BookUI.Instance.Display == Display.Visible ? Display.None : Display.Visible;
			BookUI.Instance.Recalculate();
		}

		return true;
	}
}