using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using BaseLibrary;
using BaseLibrary.Input;
using BaseLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Liquid;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace BookLibrary.UI;

public class UIEntryItem_Recipe : UIEntryItem
{
	private const ushort WorldWidth = 200;
	private const ushort WorldHeight = 200;

	private struct GroupData
	{
		public GroupData(int group = -1)
		{
			this.group = group;
		}

		public int group;
		public int currentItem;
	}

	private Recipe _recipe;
	private UIItem[] _items;
	private GroupData[] _groups;

	public static Tilemap? Tilemap;

	public unsafe UIEntryItem_Recipe(BookEntryItem_Recipe entry)
	{
		if (Tilemap is null)
		{
			Tilemap map = new Tilemap();
			uint data = WorldWidth | WorldHeight << 16;
			Unsafe.Copy(&map, ref data);
			Tilemap = map;
		}

		_recipe = entry.Recipe;

		UIItem result = new UIItem(entry.Recipe.createItem) {
			Size = Dimension.FromPixels(62),
			Position = new Dimension(0, 6, 50, 0)
		};
		base.Add(result);

		UIText text = new UIText("at") {
			Size = Dimension.FromPixels(20, 62),
			Position = new Dimension(48, 6, 50, 0),
			Settings = {
				VerticalAlignment = VerticalAlignment.Center,
				TextColor = BookUI.TextColor,
				BorderColor = Color.Transparent
			}
		};
		base.Add(text);

		DoActionOnTilemap(() => {
			for (int i = 10; i <= 12; i++)
			{
				for (int j = 8; j <= 10; j++)
				{
					Main.tile[i, j].ClearEverything();
				}
			}

			TileObject.CanPlace(11, 10, TileID.Furnaces, 0, 0, out TileObject objectData);

			// objectData.random = random;
			TileObject.Place(objectData);
		});

		_items = new UIItem[entry.Recipe.requiredItem.Count];
		_groups = new GroupData[entry.Recipe.requiredItem.Count];

		for (int i = 0; i < entry.Recipe.requiredItem.Count; i++)
		{
			Item item = entry.Recipe.requiredItem[i];

			_groups[i] = new GroupData(-1);
			foreach (int num in _recipe.acceptedGroups.Where(num => RecipeGroup.recipeGroups[num].ContainsItem(item.type)))
			{
				_groups[i] = new GroupData(num);
				break;
			}

			UIItem ingredient = new UIItem(item) {
				Size = Dimension.FromPixels(36 + 12)
			};
			_items[i] = ingredient;
			base.Add(ingredient);
		}
	}

	private const int SlotSize = 52;

	private int ticks = 0;

	private static void DoActionOnTilemap(Action action)
	{
		// Make sure autosave doesn't run when we are swapping Main.tile
		bool prevAutosave = Main.skipMenu;
		Main.skipMenu = true;

		Tilemap cache = Main.tile;
		int oldTilesX = Main.maxTilesX;
		int oldTilesY = Main.maxTilesY;
		Main.maxTilesX = 200;
		Main.maxTilesY = 200;
		Main.tile = Tilemap.Value;

		action();

		Main.tile = cache;
		Main.maxTilesX = oldTilesX;
		Main.maxTilesY = oldTilesY;

		Main.skipMenu = prevAutosave;
	}

	private void DrawSingleTile(TileDrawInfo drawData, int tileX, int tileY, bool solidLayer, int waterStyleOverride, Vector2 screenPosition, Vector2 screenOffset)
	{
		// TODO: how would modded draw methods work? a flag?

		drawData.tileCache = Main.tile[tileX, tileY];
		drawData.typeCache = drawData.tileCache.TileType;
		drawData.tileFrameX = drawData.tileCache.TileFrameX;
		drawData.tileFrameY = drawData.tileCache.TileFrameY;
		drawData.tileLight = Lighting.GetColor(tileX, tileY);
		if (drawData.tileCache.LiquidType > 0 && drawData.tileCache.TileType == TileID.LilyPad)
			return;

		if (!drawData.tileCache.HasTile)
			return;

		// TODO: this is gnarly
		Main.instance.TilesRenderer.GetTileDrawData(tileX, tileY, drawData.tileCache, drawData.typeCache, ref drawData.tileFrameX, ref drawData.tileFrameY, out drawData.tileWidth, out drawData.tileHeight, out drawData.tileTop, out drawData.halfBrickHeight, out drawData.addFrX, out drawData.addFrY, out drawData.tileSpriteEffect, out drawData.glowTexture, out drawData.glowSourceRect, out drawData.glowColor);
		drawData.drawTexture = Main.instance.TilesRenderer.GetTileDrawTexture(drawData.tileCache, tileX, tileY);
		Texture2D highlightTexture = null;
		Rectangle empty = Rectangle.Empty;
		Color highlightColor = Color.Transparent;
		if (TileID.Sets.HasOutlines[drawData.typeCache])
			Main.instance.TilesRenderer.GetTileOutlineInfo(tileX, tileY, drawData.typeCache, ref drawData.tileLight, ref highlightTexture, ref highlightColor);

		// dust code
		/*
		if (_isActiveAndNotPaused) {
			if (!Lighting.UpdateEveryFrame || new FastRandom(Main.TileFrameSeed).WithModifier(tileX, tileY).Next(4) == 0)
				DrawTiles_EmitParticles(tileY, tileX, drawData.tileCache, drawData.typeCache, drawData.tileFrameX, drawData.tileFrameY, drawData.tileLight);

			drawData.tileLight = DrawTiles_GetLightOverride(tileY, tileX, drawData.tileCache, drawData.typeCache, drawData.tileFrameX, drawData.tileFrameY, drawData.tileLight);
		}*/

		bool visible = false;
		if (drawData.tileLight.R >= 1 || drawData.tileLight.G >= 1 || drawData.tileLight.B >= 1)
			visible = true;

		if (drawData.tileCache.WallType > 0 && (drawData.tileCache.WallType == 318 || drawData.tileCache.IsWallFullbright))
			visible = true;


		Vector2 _zero = new Vector2(0, 0);
		// visible &= IsVisible(drawData.tileCache);
		// CacheSpecialDraws_Part1(tileX, tileY, drawData.typeCache, drawData.tileFrameX, drawData.tileFrameY, !visible);
		// CacheSpecialDraws_Part2(tileX, tileY, drawData, !visible);
		if (drawData.typeCache == 72 && drawData.tileFrameX >= 36)
		{
			int num4 = 0;
			if (drawData.tileFrameY == 18)
				num4 = 1;
			else if (drawData.tileFrameY == 36)
				num4 = 2;

			Main.spriteBatch.Draw(TextureAssets.ShroomCap.Value, new Vector2(tileX * 16 - (int)screenPosition.X - 22, tileY * 16 - (int)screenPosition.Y - 26) + screenOffset, new Rectangle(num4 * 62, 0, 60, 42), Lighting.GetColor(tileX, tileY), 0f, _zero, 1f, drawData.tileSpriteEffect, 0f);
		}

		Rectangle rectangle = new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY, drawData.tileWidth, drawData.tileHeight - drawData.halfBrickHeight);
		Vector2 normaltilepos = new Vector2((float)(tileX * 16 - (int)screenPosition.X) - ((float)drawData.tileWidth - 16f) / 2f, tileY * 16 - (int)screenPosition.Y + drawData.tileTop + drawData.halfBrickHeight) + screenOffset;

		TileLoader.DrawEffects(tileX, tileY, drawData.typeCache, Main.spriteBatch, ref drawData);
		// if (!visible)
		// return;

		drawData.colorTint = Color.White;
		// drawData.finalColor = Main.instance.TilesRenderer.GetFinalLight(drawData.tileCache, drawData.typeCache, drawData.tileLight, drawData.colorTint);
		drawData.finalColor = Color.White;
		switch (drawData.typeCache)
		{
			case 136:
				switch (drawData.tileFrameX / 18)
				{
					case 1:
						normaltilepos.X += -2f;
						break;
					case 2:
						normaltilepos.X += 2f;
						break;
				}

				break;
			case 442:
			{
				int num7 = drawData.tileFrameX / 22;
				if (num7 == 3)
					normaltilepos.X += 2f;

				break;
			}
			case 51:
				drawData.finalColor = drawData.tileLight * 0.5f;
				break;
			case 160:
			case 692:
			{
				Color color = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 255);
				// if (drawData.tileCache.IsActuated)
				// color = drawData.tileCache.actuatedColor;

				drawData.finalColor = color;
				break;
			}
			case 129:
			{
				drawData.finalColor = new Color(255, 255, 255, 100);
				int num6 = 2;
				if (drawData.tileFrameX >= 324)
					drawData.finalColor = Color.Transparent;

				if (drawData.tileFrameY < 36)
					normaltilepos.Y += num6 * (drawData.tileFrameY == 0).ToDirectionInt();
				else
					normaltilepos.X += num6 * (drawData.tileFrameY == 36).ToDirectionInt();

				break;
			}
			case 272:
			{
				int num5 = Main.tileFrame[drawData.typeCache];
				num5 += tileX % 2;
				num5 += tileY % 2;
				num5 += tileX % 3;
				num5 += tileY % 3;
				num5 %= 2;
				num5 *= 90;
				drawData.addFrY += num5;
				rectangle.Y += num5;
				break;
			}
			case 80:
			{
				WorldGen.GetCactusType(tileX, tileY, drawData.tileFrameX, drawData.tileFrameY, out var evil, out var good, out var crimson);
				if (evil)
					rectangle.Y += 54;

				if (good)
					rectangle.Y += 108;

				if (crimson)
					rectangle.Y += 162;

				break;
			}
			case 83:
				drawData.drawTexture = Main.instance.TilesRenderer.GetTileDrawTexture(drawData.tileCache, tileX, tileY);
				break;
			case 323:
				if (drawData.tileCache.TileFrameX <= 132 && drawData.tileCache.TileFrameX >= 88)
					return;
				normaltilepos.X += drawData.tileCache.TileFrameY;
				break;
			case 114:
				if (drawData.tileFrameY > 0)
					rectangle.Height += 2;
				break;
		}

		// TODO: this might not be that hard to replace
		// if (drawData.typeCache == 314)
		// 	DrawTile_MinecartTrack(screenPosition, screenOffset, tileX, tileY, drawData);
		// else if (drawData.typeCache == 171)
		// 	DrawXmasTree(screenPosition, screenOffset, tileX, tileY, drawData);
		// else
		typeof(TileDrawing).GetMethod("DrawBasicTile", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(Main.instance.TilesRenderer, new object?[] { screenPosition, screenOffset, tileX, tileY, drawData, rectangle, normaltilepos });

		/*if (Main.tileGlowMask[drawData.tileCache.TileType] != -1)
		{
			short num8 = Main.tileGlowMask[drawData.tileCache.TileType];
			if (TextureAssets.GlowMask.IndexInRange(num8))
				drawData.drawTexture = TextureAssets.GlowMask[num8].Value;

			double num9 = Main.timeForVisualEffects * 0.08;
			Color color2 = Color.White;
			bool flag2 = false;
			switch (drawData.tileCache.TileType)
			{
				case 633:
					color2 = Color.Lerp(Color.White, drawData.finalColor, 0.75f);
					break;
				case 659:
				case 667:
					color2 = LiquidRenderer.GetShimmerGlitterColor(top: true, tileX, tileY);
					break;
				case 350:
					color2 = new Color(new Vector4((float)((0.0 - Math.Cos(((int)(num9 / 6.283) % 3 == 1) ? num9 : 0.0)) * 0.2 + 0.2)));
					break;
				case 381:
				case 517:
				case 687:
					// color2 = _lavaMossGlow;
					break;
				case 534:
				case 535:
				case 689:
					// color2 = _kryptonMossGlow;
					break;
				case 536:
				case 537:
				case 690:
					// color2 = _xenonMossGlow;
					break;
				case 539:
				case 540:
				case 688:
					// color2 = _argonMossGlow;
					break;
				case 625:
				case 626:
				case 691:
					// color2 = _violetMossGlow;
					break;
				case 627:
				case 628:
				case 692:
					color2 = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
					break;
				case 370:
				case 390:
					// color2 = _meteorGlow;
					break;
				case 391:
					color2 = new Color(250, 250, 250, 200);
					break;
				case 209:
					color2 = PortalHelper.GetPortalColor(Main.myPlayer, (drawData.tileCache.TileFrameX >= 288) ? 1 : 0);
					break;
				case 429:
				case 445:
					// drawData.drawTexture = GetTileDrawTexture(drawData.tileCache, tileX, tileY);
					// drawData.addFrY = 18;
					break;
				case 129:
				{
					if (drawData.tileFrameX < 324)
					{
						flag2 = true;
						break;
					}

					drawData.drawTexture = Main.instance.TilesRenderer.GetTileDrawTexture(drawData.tileCache, tileX, tileY);
					color2 = Main.hslToRgb(0.7f + (float)Math.Sin((float)Math.PI * 2f * Main.GlobalTimeWrappedHourly * 0.16f + (float)tileX * 0.3f + (float)tileY * 0.7f) * 0.16f, 1f, 0.5f);
					color2.A /= 2;
					color2 *= 0.3f;
					int num10 = 72;
					for (float num11 = 0f; num11 < (float)Math.PI * 2f; num11 += (float)Math.PI / 2f)
					{
						Main.spriteBatch.Draw(drawData.drawTexture, normaltilepos + num11.ToRotationVector2() * 2f, new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY + num10, drawData.tileWidth, drawData.tileHeight), color2, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
					}

					color2 = new Color(255, 255, 255, 100);
					break;
				}
			}

			if (!flag2)
			{
				if (drawData.tileCache.Slope == 0 && !drawData.tileCache.IsHalfBlock)
				{
					Main.spriteBatch.Draw(drawData.drawTexture, normaltilepos, new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY, drawData.tileWidth, drawData.tileHeight), color2, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				}
				else if (drawData.tileCache.IsHalfBlock)
				{
					Main.spriteBatch.Draw(drawData.drawTexture, normaltilepos, rectangle, color2, 0f, _zero, 1f, SpriteEffects.None, 0f);
				}
				else if (TileID.Sets.Platforms[drawData.tileCache.TileType])
				{
					Main.spriteBatch.Draw(drawData.drawTexture, normaltilepos, rectangle, color2, 0f, _zero, 1f, drawData.tileSpriteEffect, 0f);
					// if (drawData.tileCache.Slope == 1 && Main.tile[tileX + 1, tileY + 1].HasTile && Main.tileSolid[Main.tile[tileX + 1, tileY + 1].TileType] && Main.tile[tileX + 1, tileY + 1].Slope != 2 && !Main.tile[tileX + 1, tileY + 1].IsHalfBlock && (!Main.tile[tileX, tileY + 1].HasTile || (Main.tile[tileX, tileY + 1].blockType() != 0 && Main.tile[tileX, tileY + 1].blockType() != 5) || (!TileID.Sets.BlocksStairs[Main.tile[tileX, tileY + 1].TileType] && !TileID.Sets.BlocksStairsAbove[Main.tile[tileX, tileY + 1].TileType])))
					// {
					// 	Rectangle value = new Rectangle(198, drawData.tileFrameY, 16, 16);
					// 	if (TileID.Sets.Platforms[Main.tile[tileX + 1, tileY + 1].TileType] && Main.tile[tileX + 1, tileY + 1].Slope == 0)
					// 		value.X = 324;
					//
					// 	Main.spriteBatch.Draw(drawData.drawTexture, normaltilepos + new Vector2(0f, 16f), value, color2, 0f, _zero, 1f, drawData.tileSpriteEffect, 0f);
					// }
					// else if (drawData.tileCache.Slope == 2 && Main.tile[tileX - 1, tileY + 1].HasTile && Main.tileSolid[Main.tile[tileX - 1, tileY + 1].TileType] && Main.tile[tileX - 1, tileY + 1].Slope != 1 && !Main.tile[tileX - 1, tileY + 1].IsHalfBlock && (!Main.tile[tileX, tileY + 1].HasTile || (Main.tile[tileX, tileY + 1].blockType() != 0 && Main.tile[tileX, tileY + 1].blockType() != 4) || (!TileID.Sets.BlocksStairs[Main.tile[tileX, tileY + 1].TileType] && !TileID.Sets.BlocksStairsAbove[Main.tile[tileX, tileY + 1].TileType])))
					// {
					// 	Rectangle value2 = new Rectangle(162, drawData.tileFrameY, 16, 16);
					// 	if (TileID.Sets.Platforms[Main.tile[tileX - 1, tileY + 1].TileType] && Main.tile[tileX - 1, tileY + 1].Slope == 0)
					// 		value2.X = 306;
					//
					// 	Main.spriteBatch.Draw(drawData.drawTexture, normaltilepos + new Vector2(0f, 16f), value2, color2, 0f, _zero, 1f, drawData.tileSpriteEffect, 0f);
					// }
				}
				else if (TileID.Sets.HasSlopeFrames[drawData.tileCache.TileType])
				{
					Main.spriteBatch.Draw(drawData.drawTexture, normaltilepos, new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY, 16, 16), color2, 0f, _zero, 1f, drawData.tileSpriteEffect, 0f);
				}
				else
				{
					int num12 = (int)drawData.tileCache.Slope;
					int num13 = 2;
					for (int i = 0; i < 8; i++)
					{
						int num14 = i * -2;
						int num15 = 16 - i * 2;
						int num16 = 16 - num15;
						int num17;
						switch (num12)
						{
							case 1:
								num14 = 0;
								num17 = i * 2;
								num15 = 14 - i * 2;
								num16 = 0;
								break;
							case 2:
								num14 = 0;
								num17 = 16 - i * 2 - 2;
								num15 = 14 - i * 2;
								num16 = 0;
								break;
							case 3:
								num17 = i * 2;
								break;
							default:
								num17 = 16 - i * 2 - 2;
								break;
						}

						Main.spriteBatch.Draw(drawData.drawTexture, normaltilepos + new Vector2(num17, i * num13 + num14), new Rectangle(drawData.tileFrameX + drawData.addFrX + num17, drawData.tileFrameY + drawData.addFrY + num16, num13, num15), color2, 0f, _zero, 1f, drawData.tileSpriteEffect, 0f);
					}

					int num18 = ((num12 <= 2) ? 14 : 0);
					Main.spriteBatch.Draw(drawData.drawTexture, normaltilepos + new Vector2(0f, num18), new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY + num18, 16, 2), color2, 0f, _zero, 1f, drawData.tileSpriteEffect, 0f);
				}
			}
		}*/

		// if (drawData.glowTexture != null)
		// {
		// 	Vector2 position = new Vector2((float)(tileX * 16 - (int)screenPosition.X) - ((float)drawData.tileWidth - 16f) / 2f, tileY * 16 - (int)screenPosition.Y + drawData.tileTop) + screenOffset;
		// 	if (TileID.Sets.Platforms[drawData.typeCache])
		// 		position = vector;
		//
		// 	Main.spriteBatch.Draw(drawData.glowTexture, position, drawData.glowSourceRect, drawData.glowColor, 0f, _zero, 1f, drawData.tileSpriteEffect, 0f);
		// }
		//
		// if (highlightTexture != null)
		// {
		// 	empty = new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY, drawData.tileWidth, drawData.tileHeight);
		// 	int num19 = 0;
		// 	int num20 = 0;
		// 	Main.spriteBatch.Draw(highlightTexture, new Vector2((float)(tileX * 16 - (int)screenPosition.X) - ((float)drawData.tileWidth - 16f) / 2f + (float)num19, tileY * 16 - (int)screenPosition.Y + drawData.tileTop + num20) + screenOffset, empty, highlightColor, 0f, _zero, 1f, drawData.tileSpriteEffect, 0f);
		// }
	}

	protected override unsafe void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);

		DoActionOnTilemap(() => {
			TileDrawInfo info = new TileDrawInfo();

			for (int i = 7; i <= 10; i++)
			{
				for (int j = 10; j <= 12; j++)
				{
					DrawSingleTile(info, j, i, false, -1, -Dimensions.TopLeft(), Vector2.Zero);
				}
			}
		});
	}

	protected override void Update(GameTime gameTime)
	{
		if (!KeyboardInput.IsKeyDown(Keys.LeftShift) && ++ticks >= 30)
		{
			ticks = 0;

			for (int i = 0; i < _groups.Length; i++)
			{
				ref GroupData data = ref _groups[i];
				if (data.group == -1) continue;

				if (++data.currentItem >= RecipeGroup.recipeGroups[data.group].ValidItems.Count)
					data.currentItem = 0;

				_items[i].Item = new Item(RecipeGroup.recipeGroups[data.group].ValidItems.ElementAt(data.currentItem)) { stack = _items[i].Item.stack };
			}
		}

		base.Update(gameTime);
	}

	public override void Recalculate()
	{
		int slotsize = 36 + 12;

		int numPerRow = (int)Math.Floor(InnerDimensions.Width / (float)slotsize) - 1;
		int rows = (int)Math.Ceiling(_items.Length / (float)numPerRow);
		int toRender = _items.Length;
		int center = InnerDimensions.Width / 2;

		int num = 0;
		for (int row = 0; row < rows; row++)
		{
			int limit = Math.Min(toRender, numPerRow);
			int width = limit * (slotsize + 8) - 8;
			for (int i = 0; i < limit; i++)
			{
				toRender--;

				UIItem item = _items[row * numPerRow + i];
				item.Position.PixelsX = center - width / 2 + i * (slotsize + 8);
				item.Position.PixelsY = 76 + row * (slotsize + 8);
				item.Recalculate();
			}
		}

		Size.PixelsY = 76 + rows * (slotsize + 8) - 8;

		base.Recalculate();
	}
}