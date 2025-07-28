using System.Collections.Generic;
using BaseLibrary.UI;
using BookLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace BookLibrary;

// BUG: scroll doesn't get blocked outside of left/right page panels
public class BookUI : UIPanel
{
	public static BookUI Instance = null!;
	private static readonly Dimension BookSize = Dimension.FromPixels(1010, 740);
	private static readonly Dimension WrapperSize = Dimension.FromPixels(910, 685);
	private static readonly Dimension WrapperPosition = Dimension.FromPixels(45, 25);
	internal static readonly Color TextColor = new Color(40, 25, 14);

	internal readonly UIPageMain uiMain;
	internal readonly UIPageBook uiBook;
	internal readonly UIPageCategory uiCategory;

	internal readonly Stack<BaseElement> lastPages = [];
	internal BaseElement currentElement;

	public BookUI()
	{
		Instance = this;

		Size = BookSize;
		Position = Dimension.FromPercent(50);
		Display = Display.None;
		Settings.Texture = ModContent.Request<Texture2D>("BookLibrary/Assets/Textures/BookBackground");

		UITexture textureReturn = new UITexture(ModContent.Request<Texture2D>("BookLibrary/Assets/Textures/ReturnButton")) {
			Size = Dimension.FromPixels(30),
			Position = Dimension.FromPixels(28, 12),
			Settings = { ScaleMode = ScaleMode.Stretch, SamplerState = SamplerState.PointClamp }
		};
		textureReturn.OnMouseDown += args => {
			if (lastPages.TryPop(out BaseElement? element))
			{
				element.Display = Display.Visible;
				if (currentElement != null) currentElement.Display = Display.None;
				currentElement = element;
			}
			else
			{
				Display = Display.None;
				currentElement = uiMain!;
			}

			args.Handled = true;
		};
		base.Add(textureReturn);

		currentElement = uiMain = new UIPageMain {
			Size = WrapperSize,
			Position = WrapperPosition,
		};
		base.Add(uiMain);

		uiBook = new UIPageBook {
			Size = WrapperSize,
			Position = WrapperPosition,
			Display = Display.None
		};
		base.Add(uiBook);

		uiCategory = new UIPageCategory {
			Size = WrapperSize,
			Position = WrapperPosition,
			Display = Display.None
		};
		base.Add(uiCategory);
	}

	public void PushPage(UIBookPage element)
	{
		currentElement.Display = Display.None;
		element.Display = Display.Visible;
		element.OpenPage();

		lastPages.Push(currentElement);
		currentElement = element;
	}
}