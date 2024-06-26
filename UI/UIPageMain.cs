using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace BookLibrary.UI;

public class UIPageMain : UIPageBook
{
	public UIPageMain()
	{
		pageLeft.Add(new UIText(ModContent.GetInstance<BookLibrary>().GetLocalization("UI.BookName"), 1.1f) {
			Size = new Dimension(0, 30, 100, 0),
			Settings = {
				HorizontalAlignment = HorizontalAlignment.Center,
				TextColor = BookUI.TextColor,
				BorderColor = Color.Transparent
			}
		});
		
		pageLeft.Add(new UITexture(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Separator1")) {
			Size = new Dimension(0, 4, 100, 0),
			Position = Dimension.FromPixels(0, 38),
			Settings = { ScaleMode = ScaleMode.Stretch, Color = new Color(138, 89, 45) }
		});

		pageRight.Add(new UIText(ModContent.GetInstance<BookLibrary>().GetLocalization("UI.BookDescription")) {
			Size = new Dimension(0, -40, 100, 100),
			Position = Dimension.FromPixels(0, 40),
			Settings = {
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				TextColor = BookUI.TextColor,
				BorderColor = Color.Transparent
			}
		});

		UIGrid<UIModBook> grid = new() {
			Size = new Dimension(0, -48, 100, 100),
			Position = Dimension.FromPixels(0, 48)
		};

		pageLeft.Add(grid);

		foreach (ModBook modBook in BookLoader.items)
		{
			grid.Add(new UIModBook(modBook).AddOnClick(args => {
				BookUI.Instance.PushPage(BookUI.Instance.uiBook);
				BookUI.Instance.uiBook.SetBook(modBook);

				args.Handled = true;
			}));
		}
	}
}