using BaseLibrary.Input;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace BookLibrary.UI;

public class UIPageMain : UIPageBook
{
	public UIPageMain()
	{
		pageLeft.Add(new UIText(BookLibrary.Instance.GetLocalization("UI.BookName"), 1.1f) {
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
			Settings = {
				ScaleMode = ScaleMode.Stretch,
				Color = new Color(138, 89, 45)
			}
		});

		SetupIntroPage();

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

	private void SetupIntroPage()
	{
		UITexture textureBrain = new UITexture(Main.Assets.Request<Texture2D>("Images/NPC_266")) {
			Size = new Dimension(300, 270, 0, 0),
			Position = new Dimension(0, 0, 50, 0),
			Settings = {
				ScaleMode = ScaleMode.Stretch,
				SourceRectangle = new Rectangle(0, 0, 200, 180),
				Color = new Color(150, 150, 150)
			}
		};
		pageRight.Add(textureBrain);

		UIText textName = new UIText(BookLibrary.Instance.GetLocalization("UI.ModName"), 2.5f) {
			Size = new Dimension(0, 40, 100, 0),
			Position = Dimension.FromPixels(0, 50),
			Settings = {
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				TextColor = Color.White,
				BorderColor = Color.Black
			}
		};
		pageRight.Add(textName);

		UIText textAuthor = new UIText(BookLibrary.Instance.GetLocalization("UI.ModAuthor"), 1.2f) {
			Size = new Dimension(0, 40, 100, 0),
			Position = Dimension.FromPixels(0, 140),
			Settings = {
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				TextColor = Color.White,
				BorderColor = Color.Black
			}
		};
		pageRight.Add(textAuthor);

		UIText textDescription = new UIText(BookLibrary.Instance.GetLocalization("UI.ModDescription")) {
			Size = new Dimension(0, -250, 100, 100),
			Position = Dimension.FromPixels(0, 200),
			Settings = {
				HorizontalAlignment = HorizontalAlignment.Left,
				VerticalAlignment = VerticalAlignment.Center,
				TextColor = BookUI.TextColor,
				BorderColor = Color.Transparent
			}
		};
		pageRight.Add(textDescription);

		UIText textPatreon = new UIText(BookLibrary.Instance.GetLocalization("UI.ModPatreon"), 1.1f) {
			Size = new Dimension(0, 30, 50, 0),
			Position = Dimension.FromPercent(0, 100),
			HoverText = "patreon.com/Itorius",
			Settings = {
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Top,
				TextColor = new Color(252, 116, 78),
				BorderColor = Color.Black
			}
		};
		textPatreon.OnClick += args => {
			if (args.Button != MouseButton.Left) return;
			args.Handled = true;

			Utils.OpenToURL("https://www.patreon.com/Itorius");
		};
		pageRight.Add(textPatreon);

		UIText textDiscord = new UIText(BookLibrary.Instance.GetLocalization("UI.ModDiscord"), 1.1f) {
			Size = new Dimension(0, 30, 50, 0),
			Position = Dimension.FromPercent(100, 100),
			HoverText = "discord.gg/EP9nfZV",
			Settings = {
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Top,
				TextColor = new Color(78, 162, 252),
				BorderColor = Color.Black
			}
		};
		textDiscord.OnClick += args => {
			if (args.Button != MouseButton.Left) return;
			args.Handled = true;

			Utils.OpenToURL("https://discord.com/invite/EP9nfZV");
		};
		pageRight.Add(textDiscord);
	}
}