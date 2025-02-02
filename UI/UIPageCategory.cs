using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace BookLibrary.UI;

public class UIPageCategory : UIPageBook
{
	private readonly UIText textCategoryName;
	private readonly UIText textEntryName;

	private readonly UIGrid<UIBookEntry> gridEntries;
	private readonly UIGrid<UIEntryItem> gridEntryItems;

	private readonly UITexture separatorRight;

	public UIPageCategory()
	{
		textCategoryName = new UIText(LocalizedText.Empty, 1.1f) {
			Size = new Dimension(0, 30, 100, 0),
			Settings = {
				HorizontalAlignment = HorizontalAlignment.Center,
				TextColor = BookUI.TextColor,
				BorderColor = Color.Transparent
			}
		};
		pageLeft.Add(textCategoryName);

		// TODO: highlight the current selected entry
		gridEntries = new UIGrid<UIBookEntry> {
			Size = new Dimension(0, -48, 100, 100),
			Position = Dimension.FromPixels(0, 48)
		};
		pageLeft.Add(gridEntries);

		pageLeft.Add(new UITexture(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Separator1")) {
			Size = new Dimension(0, 4, 100, 0),
			Position = Dimension.FromPixels(0, 38),
			Settings = { ScaleMode = ScaleMode.Stretch, Color = new Color(138, 89, 45) }
		});

		textEntryName = new UIText(LocalizedText.Empty, 1.1f) {
			Size = new Dimension(0, 30, 100, 0),
			Settings = {
				HorizontalAlignment = HorizontalAlignment.Center,
				TextColor = BookUI.TextColor,
				BorderColor = Color.Transparent
			}
		};
		pageRight.Add(textEntryName);

		gridEntryItems = new UIGrid<UIEntryItem> {
			Size = new Dimension(0, -48, 100, 100),
			Position = Dimension.FromPixels(0, 48),
			Settings = {
				ItemMargin = 8,
				ItemAlignment = HorizontalAlignment.Center
			}
		};
		pageRight.Add(gridEntryItems);

		separatorRight = new UITexture(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Separator1")) {
			Size = new Dimension(0, 4, 100, 0),
			Position = Dimension.FromPixels(0, 38),
			Settings = { ScaleMode = ScaleMode.Stretch, Color = new Color(138, 89, 45) }
		};
		pageRight.Add(separatorRight);
	}

	public override void OpenPage()
	{
		gridEntryItems.Clear();
		textEntryName.Display = Display.None;
		separatorRight.Display = Display.None;
	}

	public void SetCategory(BookCategory category)
	{
		textCategoryName.Text = category.DisplayName;

		gridEntries.Clear();
		foreach (BookEntry entry in category.Items)
		{
			UIBookEntry uiBookEntry = new(entry);
			uiBookEntry.OnClick += args => {
				separatorRight.Display = Display.Visible;
				textEntryName.Display = Display.Visible;
				textEntryName.Text = entry.DisplayName;

				gridEntryItems.Clear();

				foreach (BookEntryItem entryItem in entry.Items)
				{
					UIEntryItem? item = entryItem switch {
						BookEntryItem_Text text => new UIEntryItem_Text(text),
						BookEntryItem_Image image => new UIEntryItem_Image(image),
						BookEntryItem_Recipe recipe => new UIEntryItem_Recipe(recipe),
						BookEntryItem_Video video => new UIEntryItem_Video(video),
						_ => null
					};

					if (item is not null)
						gridEntryItems.Add(item);
				}

				args.Handled = true;
			};
			gridEntries.Add(uiBookEntry);
		}
	}
}