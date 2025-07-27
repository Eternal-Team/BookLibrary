using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.Liquid;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.ObjectData;

namespace BookLibrary;

internal static class CustomTileRenderer
{
	public const ushort WorldWidth = 200;
	public const ushort WorldHeight = 200;

	private static Tilemap? Tilemap;
	private static TileDrawInfo tileDrawInfo;

	public static unsafe void Load()
	{
		if (Tilemap is not null) return;

		Tilemap map = new Tilemap();
		uint data = WorldWidth | WorldHeight << 16;
		Unsafe.Copy(&map, ref data);
		Tilemap = map;

		tileDrawInfo = new TileDrawInfo();
	}

	public static void PerformAction(Action action)
	{
		// BUG: seems like this isn't enough to stop world saving, might have to hook SaveWorld
		bool prevAutosave = Main.skipMenu;
		Main.skipMenu = true;

		Tilemap cache = Main.tile;
		int oldTilesX = Main.maxTilesX;
		int oldTilesY = Main.maxTilesY;
		Main.maxTilesX = WorldWidth;
		Main.maxTilesY = WorldHeight;
		Main.tile = Tilemap.Value;

		action();

		Main.tile = cache;
		Main.maxTilesX = oldTilesX;
		Main.maxTilesY = oldTilesY;

		Main.skipMenu = prevAutosave;
	}

	// TODO: how would modded draw methods work? a flag?
	private static void DrawSingleTile(TileDrawInfo drawData, int tileX, int tileY, Vector2 screenPosition, Vector2 screenOffset)
	{
		drawData.tileCache = Main.tile[tileX, tileY];
		drawData.typeCache = drawData.tileCache.TileType;
		drawData.tileFrameX = drawData.tileCache.TileFrameX;
		drawData.tileFrameY = drawData.tileCache.TileFrameY;
		drawData.tileLight = Color.White;

		if (!drawData.tileCache.HasTile || drawData.tileCache is { LiquidType: > 0, TileType: TileID.LilyPad })
			return;

		Main.instance.TilesRenderer.GetTileDrawData(tileX, tileY, drawData.tileCache, drawData.typeCache, ref drawData.tileFrameX, ref drawData.tileFrameY, out drawData.tileWidth, out drawData.tileHeight, out drawData.tileTop, out drawData.halfBrickHeight, out drawData.addFrX, out drawData.addFrY, out drawData.tileSpriteEffect, out drawData.glowTexture, out drawData.glowSourceRect, out drawData.glowColor);
		drawData.drawTexture = Main.instance.TilesRenderer.GetTileDrawTexture(drawData.tileCache, tileX, tileY);
		Texture2D highlightTexture = null;
		Color highlightColor = Color.Transparent;
		if (TileID.Sets.HasOutlines[drawData.typeCache])
			Main.instance.TilesRenderer.GetTileOutlineInfo(tileX, tileY, drawData.typeCache, ref drawData.tileLight, ref highlightTexture, ref highlightColor);

		Main.instance.TilesRenderer.CacheSpecialDraws_Part1(tileX, tileY, drawData.typeCache, drawData.tileFrameX, drawData.tileFrameY, false);
		Main.instance.TilesRenderer.CacheSpecialDraws_Part2(tileX, tileY, drawData, false);
		
		if (drawData is { typeCache: TileID.MushroomTrees, tileFrameX: >= 36 })
		{
			int num4 = drawData.tileFrameY switch {
				18 => 1,
				36 => 2,
				_ => 0
			};

			Main.spriteBatch.Draw(TextureAssets.ShroomCap.Value, new Vector2(tileX * 16 - (int)screenPosition.X - 22, tileY * 16 - (int)screenPosition.Y - 26) + screenOffset, new Rectangle(num4 * 62, 0, 60, 42), Lighting.GetColor(tileX, tileY), 0f, Vector2.Zero, 1f, drawData.tileSpriteEffect, 0f);
		}

		Rectangle normalTileRectangle = new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY, drawData.tileWidth, drawData.tileHeight - drawData.halfBrickHeight);
		Vector2 normalTilePosition = new Vector2(tileX * 16 - (int)screenPosition.X - (drawData.tileWidth - 16f) / 2f, tileY * 16 - (int)screenPosition.Y + drawData.tileTop + drawData.halfBrickHeight) + screenOffset;

		drawData.colorTint = Color.White;
		drawData.finalColor = TileDrawing.GetFinalLight(drawData.tileCache, drawData.typeCache, drawData.tileLight, drawData.colorTint);

		switch (drawData.typeCache)
		{
			case TileID.Switches:
				switch (drawData.tileFrameX / 18)
				{
					case 1:
						normalTilePosition.X += -2f;
						break;
					case 2:
						normalTilePosition.X += 2f;
						break;
				}

				break;
			case TileID.ProjectilePressurePad:
			{
				int num7 = drawData.tileFrameX / 22;
				if (num7 == 3)
					normalTilePosition.X += 2f;

				break;
			}
			case TileID.Cobweb:
				drawData.finalColor = drawData.tileLight * 0.5f;
				break;
			case TileID.RainbowBrick:
			case TileID.RainbowMossBlock:
			{
				Color color = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 255);
				if (drawData.tileCache.IsActuated) color *= 0.4f;
				drawData.finalColor = color;

				break;
			}
			case TileID.Crystals:
			{
				drawData.finalColor = new Color(255, 255, 255, 100);
				if (drawData.tileFrameX >= 324) drawData.finalColor = Color.Transparent;

				if (drawData.tileFrameY < 36) normalTilePosition.Y += 2 * (drawData.tileFrameY == 0).ToDirectionInt();
				else normalTilePosition.X += 2 * (drawData.tileFrameY == 36).ToDirectionInt();

				break;
			}
			case TileID.Cog:
			{
				int num5 = Main.tileFrame[drawData.typeCache];
				num5 += tileX % 2;
				num5 += tileY % 2;
				num5 += tileX % 3;
				num5 += tileY % 3;
				num5 %= 2;
				num5 *= 90;
				drawData.addFrY += num5;
				normalTileRectangle.Y += num5;
				break;
			}
			case TileID.Cactus:
			{
				WorldGen.GetCactusType(tileX, tileY, drawData.tileFrameX, drawData.tileFrameY, out bool evil, out bool good, out bool crimson);
				if (evil) normalTileRectangle.Y += 54;
				if (good) normalTileRectangle.Y += 108;
				if (crimson) normalTileRectangle.Y += 162;

				break;
			}
			case TileID.MatureHerbs:
				drawData.drawTexture = Main.instance.TilesRenderer.GetTileDrawTexture(drawData.tileCache, tileX, tileY);
				break;
			case TileID.PalmTree:
				if (drawData.tileCache.TileFrameX is <= 132 and >= 88)
					return;
				normalTilePosition.X += drawData.tileCache.TileFrameY;
				break;
			case TileID.TinkerersWorkbench:
				if (drawData.tileFrameY > 0)
					normalTileRectangle.Height += 2;
				break;
		}

		Main.instance.TilesRenderer.DrawBasicTile(screenPosition, screenOffset, tileX, tileY, drawData, normalTileRectangle, normalTilePosition);
		// typeof(TileDrawing).GetMethod("DrawBasicTile", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(Main.instance.TilesRenderer, new object?[] { screenPosition, screenOffset, tileX, tileY, drawData, normalTileRectangle, normalTilePosition });

		if (Main.tileGlowMask[drawData.tileCache.TileType] != -1)
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
					color2 = new Color(new Vector4((float)((0.0 - Math.Cos((int)(num9 / 6.283) % 3 == 1 ? num9 : 0.0)) * 0.2 + 0.2)));
					break;
				case 381:
				case 517:
				case 687:
					color2 = Main.instance.TilesRenderer._lavaMossGlow;
					break;
				case 534:
				case 535:
				case 689:
					color2 = Main.instance.TilesRenderer._kryptonMossGlow;
					break;
				case 536:
				case 537:
				case 690:
					color2 = Main.instance.TilesRenderer._xenonMossGlow;
					break;
				case 539:
				case 540:
				case 688:
					color2 = Main.instance.TilesRenderer._argonMossGlow;
					break;
				case 625:
				case 626:
				case 691:
					color2 = Main.instance.TilesRenderer._violetMossGlow;
					break;
				case 627:
				case 628:
				case 692:
					color2 = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB);
					break;
				case 370:
				case 390:
					color2 = Main.instance.TilesRenderer._meteorGlow;
					break;
				case 391:
					color2 = new Color(250, 250, 250, 200);
					break;
				case 209:
					color2 = PortalHelper.GetPortalColor(Main.myPlayer, drawData.tileCache.TileFrameX >= 288 ? 1 : 0);
					break;
				case 429:
				case 445:
					drawData.drawTexture = Main.instance.TilesRenderer.GetTileDrawTexture(drawData.tileCache, tileX, tileY);
					drawData.addFrY = 18;
					break;
				case 129:
				{
					if (drawData.tileFrameX < 324)
					{
						flag2 = true;
						break;
					}

					drawData.drawTexture = Main.instance.TilesRenderer.GetTileDrawTexture(drawData.tileCache, tileX, tileY);
					color2 = Main.hslToRgb(0.7f + (float)Math.Sin((float)Math.PI * 2f * Main.GlobalTimeWrappedHourly * 0.16f + tileX * 0.3f + tileY * 0.7f) * 0.16f, 1f, 0.5f);
					color2.A /= 2;
					color2 *= 0.3f;
					int num10 = 72;
					for (float num11 = 0f; num11 < (float)Math.PI * 2f; num11 += (float)Math.PI / 2f)
					{
						Main.spriteBatch.Draw(drawData.drawTexture, normalTilePosition + num11.ToRotationVector2() * 2f, new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY + num10, drawData.tileWidth, drawData.tileHeight), color2, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
					}

					color2 = new Color(255, 255, 255, 100);
					break;
				}
			}

			if (!flag2)
			{
				if (drawData.tileCache.Slope == 0 && !drawData.tileCache.IsHalfBlock)
				{
					Main.spriteBatch.Draw(drawData.drawTexture, normalTilePosition, new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY, drawData.tileWidth, drawData.tileHeight), color2, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				}
				else if (drawData.tileCache.IsHalfBlock)
				{
					Main.spriteBatch.Draw(drawData.drawTexture, normalTilePosition, normalTileRectangle, color2, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
				}
				else if (TileID.Sets.Platforms[drawData.tileCache.TileType])
				{
					Main.spriteBatch.Draw(drawData.drawTexture, normalTilePosition, normalTileRectangle, color2, 0f, Vector2.Zero, 1f, drawData.tileSpriteEffect, 0f);
					if (drawData.tileCache.Slope == SlopeType.SlopeDownLeft && Main.tile[tileX + 1, tileY + 1].HasTile && Main.tileSolid[Main.tile[tileX + 1, tileY + 1].TileType] && Main.tile[tileX + 1, tileY + 1].Slope != SlopeType.SlopeDownRight && !Main.tile[tileX + 1, tileY + 1].IsHalfBlock && (!Main.tile[tileX, tileY + 1].HasTile || (Main.tile[tileX, tileY + 1].blockType() != 0 && Main.tile[tileX, tileY + 1].BlockType != BlockType.SlopeUpRight) || (!TileID.Sets.BlocksStairs[Main.tile[tileX, tileY + 1].TileType] && !TileID.Sets.BlocksStairsAbove[Main.tile[tileX, tileY + 1].TileType])))
					{
						Rectangle value = new Rectangle(198, drawData.tileFrameY, 16, 16);
						if (TileID.Sets.Platforms[Main.tile[tileX + 1, tileY + 1].TileType] && Main.tile[tileX + 1, tileY + 1].Slope == 0)
							value.X = 324;

						Main.spriteBatch.Draw(drawData.drawTexture, normalTilePosition + new Vector2(0f, 16f), value, color2, 0f, Vector2.Zero, 1f, drawData.tileSpriteEffect, 0f);
					}
					else if (drawData.tileCache.Slope == SlopeType.SlopeDownRight && Main.tile[tileX - 1, tileY + 1].HasTile && Main.tileSolid[Main.tile[tileX - 1, tileY + 1].TileType] && Main.tile[tileX - 1, tileY + 1].Slope != SlopeType.SlopeDownLeft && !Main.tile[tileX - 1, tileY + 1].IsHalfBlock && (!Main.tile[tileX, tileY + 1].HasTile || (Main.tile[tileX, tileY + 1].blockType() != 0 && Main.tile[tileX, tileY + 1].BlockType != BlockType.SlopeUpLeft) || (!TileID.Sets.BlocksStairs[Main.tile[tileX, tileY + 1].TileType] && !TileID.Sets.BlocksStairsAbove[Main.tile[tileX, tileY + 1].TileType])))
					{
						Rectangle value2 = new Rectangle(162, drawData.tileFrameY, 16, 16);
						if (TileID.Sets.Platforms[Main.tile[tileX - 1, tileY + 1].TileType] && Main.tile[tileX - 1, tileY + 1].Slope == 0)
							value2.X = 306;

						Main.spriteBatch.Draw(drawData.drawTexture, normalTilePosition + new Vector2(0f, 16f), value2, color2, 0f, Vector2.Zero, 1f, drawData.tileSpriteEffect, 0f);
					}
				}
				else if (TileID.Sets.HasSlopeFrames[drawData.tileCache.TileType])
				{
					Main.spriteBatch.Draw(drawData.drawTexture, normalTilePosition, new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY, 16, 16), color2, 0f, Vector2.Zero, 1f, drawData.tileSpriteEffect, 0f);
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

						Main.spriteBatch.Draw(drawData.drawTexture, normalTilePosition + new Vector2(num17, i * num13 + num14), new Rectangle(drawData.tileFrameX + drawData.addFrX + num17, drawData.tileFrameY + drawData.addFrY + num16, num13, num15), color2, 0f, Vector2.Zero, 1f, drawData.tileSpriteEffect, 0f);
					}

					int num18 = num12 <= 2 ? 14 : 0;
					Main.spriteBatch.Draw(drawData.drawTexture, normalTilePosition + new Vector2(0f, num18), new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY + num18, 16, 2), color2, 0f, Vector2.Zero, 1f, drawData.tileSpriteEffect, 0f);
				}
			}
		}

		if (drawData.glowTexture != null)
		{
			Vector2 position = new Vector2(tileX * 16 - (int)screenPosition.X - (drawData.tileWidth - 16f) / 2f, tileY * 16 - (int)screenPosition.Y + drawData.tileTop) + screenOffset;
			if (TileID.Sets.Platforms[drawData.typeCache])
				position = normalTilePosition;

			Main.spriteBatch.Draw(drawData.glowTexture, position, drawData.glowSourceRect, drawData.glowColor, 0f, Vector2.Zero, 1f, drawData.tileSpriteEffect, 0f);
		}

		if (highlightTexture != null)
		{
			Rectangle sourceRectangle = new Rectangle(drawData.tileFrameX + drawData.addFrX, drawData.tileFrameY + drawData.addFrY, drawData.tileWidth, drawData.tileHeight);
			Main.spriteBatch.Draw(highlightTexture, new Vector2(tileX * 16 - (int)screenPosition.X - (drawData.tileWidth - 16f) / 2f, tileY * 16 - (int)screenPosition.Y + drawData.tileTop) + screenOffset, sourceRectangle, highlightColor, 0f, Vector2.Zero, 1f, drawData.tileSpriteEffect, 0f);
		}
	}

	public static void DrawTileRegion(SpriteBatch spriteBatch, int startX, int startY, int endX, int endY)
	{
		for (int y = startY; y <= endY; y++)
		{
			for (int x = startX; x <= endX; x++)
			{
				// What is missing: TileLoader.PreDraw, adding special draw points, TileLoader.PostDraw
				DrawSingleTile(tileDrawInfo, x, y, Vector2.Zero, Vector2.Zero);
			}
		}
	}

	// NOTE: some tile types just don't work because Terraria is stupid
	public static void PlaceAndDrawTile(SpriteBatch spriteBatch, int tileID, Vector2 position, out Vector2 size)
	{
		for (int x = 5; x <= 25; x++)
		{
			for (int y = 5; y <= 25; y++)
			{
				Main.tile[x, y].ClearEverything();
			}
		}

		const int baseX = 5, baseY = 5;

		TileObjectData? data = TileObjectData.GetTileData(tileID, 0);
		if (data == null)
		{
			WorldGen.PlaceTile(baseX, baseY, tileID, true, true, -1, 0);
			WorldGen.PlaceTile(baseX + 1, baseY, tileID, true, true, -1, 0);
			WorldGen.PlaceTile(baseX + 1, baseY + 1, tileID, true, true, -1, 0);
			WorldGen.PlaceTile(0, baseY + 1, tileID, true, true, -1, 0);

			size = new Vector2(32f);
		}
		else
		{
			TileObject.CanPlace(baseX + data.Origin.X, baseY + data.Origin.Y, tileID, 0, 0, out TileObject objectData);
			TileObject.Place(objectData);

			size = new Vector2(data.Width * 16f, data.Height * 16f);
		}

		Point16 topLeft = new Point16(baseX, baseY);

		for (int y = topLeft.Y; y < topLeft.Y + (data?.Height ?? 2); y++)
		{
			for (int x = topLeft.X; x < topLeft.X + (data?.Width ?? 2); x++)
			{
				Tile tile = Main.tile[x, y];
				if (!tile.HasTile) continue;

				if (!TileLoader.PreDraw(x, y, tile.type, Main.spriteBatch))
					goto PostDraw;

				switch (tile.TileType)
				{
					case 52:
					case 62:
					case 115:
					case 205:
					case 382:
					case 528:
					case 636:
					case 638:
						// if (flag)
						// CrawlToTopOfVineAndAddSpecialPoint(i, j);
						continue;
					case 549:
						// if (flag)
						// CrawlToBottomOfReverseVineAndAddSpecialPoint(i, j);
						continue;
					case 34:
						// if (frameX % 54 == 0 && frameY % 54 == 0 && flag)
						// AddSpecialPoint(j, i, TileCounterType.MultiTileVine);
						continue;
					case 454:
						// if (frameX % 72 == 0 && frameY % 54 == 0 && flag)
						// AddSpecialPoint(j, i, TileCounterType.MultiTileVine);
						continue;
					case 42:
					case 270:
					case 271:
					case 572:
					case 581:
					case 660:
						// if (frameX % 18 == 0 && frameY % 36 == 0 && flag)
						// AddSpecialPoint(j, i, TileCounterType.MultiTileVine);
						continue;
					case 91:
						// if (frameX % 18 == 0 && frameY % 54 == 0 && flag)
						// AddSpecialPoint(j, i, TileCounterType.MultiTileVine);
						continue;
					case 95:
					case 126:
					case 444:
						// if (frameX % 36 == 0 && frameY % 36 == 0 && flag)
						// AddSpecialPoint(j, i, TileCounterType.MultiTileVine);
						continue;
					case 465:
					case 591:
					case 592:
						// if (frameX % 36 == 0 && frameY % 54 == 0 && flag)
						// AddSpecialPoint(j, i, TileCounterType.MultiTileVine);
						continue;
					case 27:
						// if (frameX % 36 == 0 && frameY == 0 && flag)
						// AddSpecialPoint(j, i, TileCounterType.MultiTileGrass);
						continue;
					case 236:
					case 238:
						// if (frameX % 36 == 0 && frameY == 0 && flag)
						// AddSpecialPoint(j, i, TileCounterType.MultiTileGrass);
						continue;
					case 233:
						// if (frameY == 0 && frameX % 54 == 0 && flag)
						// AddSpecialPoint(j, i, TileCounterType.MultiTileGrass);
						// if (frameY == 34 && frameX % 36 == 0 && flag)
						// AddSpecialPoint(j, i, TileCounterType.MultiTileGrass);
						continue;
					case 652:
						// if (frameX % 36 == 0 && flag)
						// AddSpecialPoint(j, i, TileCounterType.MultiTileGrass);
						continue;
					case 651:
						// if (frameX % 54 == 0 && flag)
						// AddSpecialPoint(j, i, TileCounterType.MultiTileGrass);
						continue;
					case 530:
						// if (frameX < 270) {
						// if (frameX % 54 == 0 && frameY == 0 && flag)
						// AddSpecialPoint(j, i, TileCounterType.MultiTileGrass);

						// continue;
						// }
						break;
					case 485:
					case 489:
					case 490:
						// if (frameY == 0 && frameX % 36 == 0 && flag)
						// AddSpecialPoint(j, i, TileCounterType.MultiTileGrass);
						continue;
					case 521:
					case 522:
					case 523:
					case 524:
					case 525:
					case 526:
					case 527:
						// if (frameY == 0 && frameX % 36 == 0 && flag)
						// AddSpecialPoint(j, i, TileCounterType.MultiTileGrass);
						continue;
					case 493:
						// if (frameY == 0 && frameX % 18 == 0 && flag)
						// AddSpecialPoint(j, i, TileCounterType.MultiTileGrass);
						continue;
					case 519:
						// if (frameX / 18 <= 4 && flag)
						// AddSpecialPoint(j, i, TileCounterType.MultiTileGrass);
						continue;
					case 373:
					case 374:
					case 375:
					case 461:
						// EmitLiquidDrops(i, j, tile, type);
						continue;
					case 491:
						// if (flag && frameX == 18 && frameY == 18)
						// AddSpecialPoint(j, i, TileCounterType.VoidLens);
						break;
					case 597:
						// if (flag && frameX % 54 == 0 && frameY == 0)
						// AddSpecialPoint(j, i, TileCounterType.TeleportationPylon);
						break;
					case 617:
						// if (flag && frameX % 54 == 0 && frameY % 72 == 0)
						// AddSpecialPoint(j, i, TileCounterType.MasterTrophy);
						break;
					case 184:
						// if (flag)
						// AddSpecialPoint(j, i, TileCounterType.AnyDirectionalGrass);
						continue;
					default:
						// if (ShouldSwayInWind(j, i, tile)) {
						// if (flag)
						// AddSpecialPoint(j, i, TileCounterType.WindyGrass);

						// continue;
						// }
						break;
				}

				DrawSingleTile(tileDrawInfo, x, y, topLeft.ToVector2() * 16f - position * Main.UIScale, Vector2.Zero);

				PostDraw:
				TileLoader.PostDraw(x, y, tile.type, Main.spriteBatch);
			}
		}
		
		Main.instance.TilesRenderer.DrawSpecialTilesLegacy(topLeft.ToVector2() * 16f - position * Main.UIScale, Vector2.Zero);
	}
}