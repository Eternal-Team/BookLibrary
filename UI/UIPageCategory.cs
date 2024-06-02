using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;

namespace BookLibrary.UI;

public class UIPageCategory : BaseElement
{
	private readonly UIText textCategoryName;
	private readonly UIText textEntryName;

	private readonly UIGrid<UIBookEntry> gridEntries;
	private readonly UIGrid<UIEntryItem> gridEntryItems;

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

		gridEntries = new UIGrid<UIBookEntry> {
			Size = new Dimension(0, -48, 100, 100),
			Position = Dimension.FromPixels(0, 48)
		};

		BaseElement pageLeft = new() {
			Size = Dimension.FromPixels(420, 685),
			Children = {
				textCategoryName,
				new UITexture(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Separator1")) {
					Size = new Dimension(0, 4, 100, 0),
					Position = Dimension.FromPixels(0, 38),
					Settings = { ScaleMode = ScaleMode.Stretch, Color = new Color(138, 89, 45) }
				},
				gridEntries
			}
		};
		base.Add(pageLeft);

		textEntryName = new UIText(LocalizedText.Empty, 1.1f) {
			Size = new Dimension(0, 30, 100, 0),
			Settings = {
				HorizontalAlignment = HorizontalAlignment.Center,
				TextColor = BookUI.TextColor,
				BorderColor = Color.Transparent
			}
		};

		gridEntryItems = new UIGrid<UIEntryItem> {
			Size = new Dimension(0, -48, 100, 100),
			Position = Dimension.FromPixels(0, 48),
			Settings = { ItemMargin = 8 }
		};

		BaseElement pageRight = new() {
			Size = Dimension.FromPixels(420, 670),
			Position = Dimension.FromPercent(100, 0), Children = {
				textEntryName,
				new UITexture(Main.Assets.Request<Texture2D>("Images/UI/CharCreation/Separator1")) {
					Size = new Dimension(0, 4, 100, 0),
					Position = Dimension.FromPixels(0, 38),
					Settings = { ScaleMode = ScaleMode.Stretch, Color = new Color(138, 89, 45) }
				},
				gridEntryItems
			}
		};
		base.Add(pageRight);
	}

	public void SetCategory(BookCategory category)
	{
		textCategoryName.Text = category.DisplayName;

		gridEntries.Clear();
		foreach (BookEntry entry in category.Items)
		{
			UIBookEntry uiBookEntry = new(entry);
			uiBookEntry.OnClick += args => {
				textEntryName.Text = entry.DisplayName;

				gridEntryItems.Clear();

				foreach (BookEntryItem entryItem in entry.Items)
				{
					UIEntryItem item = new(entryItem);
					gridEntryItems.Add(item);
				}

				args.Handled = true;
			};
			gridEntries.Add(uiBookEntry);
		}
	}
}