﻿// source: https://github.com/ProjectStarlight/StarlightRiver

using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace BookLibrary;

public class ScreenTarget
{
	/// <summary>
	/// What gets rendered to this screen target. Spritebatch is automatically started and RT automatically set, you only need to write the code for what you are rendering.
	/// </summary>
	public Action<SpriteBatch> drawFunct;

	/// <summary>
	/// If this render target should be rendered. Make sure this it as restrictive as possible to prevent uneccisary rendering work.
	/// </summary>
	public Func<bool> activeFunct;

	/// <summary>
	/// Optional function that runs when the screen is resized. Returns the size the render target should be. Return null to prevent resizing.
	/// </summary>
	public Func<Vector2, Vector2?> onResize;

	/// <summary>
	/// Where this render target should fall in the order of rendering. Important if you want to render something to chain into another RT.
	/// </summary>
	public float order;

	/// <summary>
	/// If this screen target should draw in the game menu. Be careful with these as lots of things are different in the menu!
	/// </summary>
	public bool allowOnMenu;

	public RenderTarget2D RenderTarget { get; set; }

	public ScreenTarget(Action<SpriteBatch> draw, Func<bool> active, float order, Func<Vector2, Vector2?> onResize = null)
	{
		if (Main.dedServ)
			return;

		drawFunct = draw;
		activeFunct = active;
		this.order = order;
		this.onResize = onResize;

		Vector2? initialDims = onResize is null ? new Vector2(Main.screenWidth, Main.screenHeight) : onResize(new Vector2(Main.screenWidth, Main.screenHeight));
		Main.QueueMainThreadAction(() => RenderTarget = new RenderTarget2D(Main.instance.GraphicsDevice, (int)initialDims?.X, (int)initialDims?.Y, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents));

		ScreenTargetHandler.AddTarget(this);
	}

	/// <summary>
	/// Foribly resize a target to a new size
	/// </summary>
	/// <param name="size"></param>
	public void ForceResize(Vector2 size)
	{
		if (Main.dedServ)
			return;

		RenderTarget.Dispose();
		RenderTarget = new RenderTarget2D(Main.instance.GraphicsDevice, (int)size.X, (int)size.Y, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
	}
}

internal class ScreenTargetHandler : ModSystem
{
	public static List<ScreenTarget> targets = new();
	public static Semaphore targetSem = new(1, 1);

	private static int firstResizeTime = 0;
	private static bool wasIngame;

	public float Priority => 1;

	public override void Load()
	{
		if (!Main.dedServ)
		{
			On_Main.CheckMonoliths += RenderScreens;
			Main.OnResolutionChanged += ResizeScreens;
			On_Main.UpdateMenu += MenuUpdate;
		}
	}

	public override void Unload()
	{
		if (!Main.dedServ)
		{
			On_Main.CheckMonoliths -= RenderScreens;
			Main.OnResolutionChanged -= ResizeScreens;

			Main.QueueMainThreadAction(() => {
				if (targets != null)
				{
					targets.ForEach(n => n.RenderTarget?.Dispose());
					targets.Clear();
					targets = null;
				}
				else
				{
					Mod.Logger.Warn("Screen targets was null, all ScreenTargets may not have been released! (leaking VRAM!)");
				}
			});
		}
	}

	/// <summary>
	/// Registers a new screen target and orders it into the list. Called automatically by the constructor of ScreenTarget!
	/// </summary>
	/// <param name="toAdd"></param>
	public static void AddTarget(ScreenTarget toAdd)
	{
		targetSem.WaitOne();

		targets.Add(toAdd);
		targets.Sort((a, b) => a.order.CompareTo(b.order));

		targetSem.Release();
	}

	/// <summary>
	/// Removes a screen target from the targets list. Should not normally need to be used.
	/// </summary>
	/// <param name="toRemove"></param>
	public static void RemoveTarget(ScreenTarget toRemove)
	{
		targetSem.WaitOne();

		targets.Remove(toRemove);
		targets.Sort((a, b) => a.order - b.order > 0 ? 1 : -1);

		targetSem.Release();
	}

	public static void ResizeScreens(Vector2 obj)
	{
		if (Main.dedServ)
			return;

		targetSem.WaitOne();

		targets.ForEach(n => {
			if (!Main.gameMenu || n.allowOnMenu)
			{
				Vector2? size = obj;

				if (n.onResize != null)
					size = n.onResize(obj);

				if (Main.gameMenu)
				{
					float menuScalingFactor = (float)size.Value.Y / 900f;
					if (menuScalingFactor < 1f)
						menuScalingFactor = 1f;

					if (Main.SettingDontScaleMainMenuUp)
						menuScalingFactor = 1f;

					size /= menuScalingFactor;
				}

				if (size != null)
				{
					n.RenderTarget?.Dispose();
					n.RenderTarget = new RenderTarget2D(Main.instance.GraphicsDevice, (int)size?.X, (int)size?.Y, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
				}
			}
		});

		targetSem.Release();
	}

	private void RenderScreens(On_Main.orig_CheckMonoliths orig)
	{
		orig();

		if (Main.dedServ)
			return;

		RenderTargetBinding[] bindings = Main.graphics.GraphicsDevice.GetRenderTargets();

		targetSem.WaitOne();

		foreach (ScreenTarget target in targets)
		{
			if (Main.gameMenu && !target.allowOnMenu)
				continue;

			if (target.drawFunct is null) //allows for RTs which dont draw in the default loop, like the lighting tile buffers
				continue;

			if (target.activeFunct())
			{
				Main.spriteBatch.Begin(default, default, Main.DefaultSamplerState, default, RasterizerState.CullNone, default);
				Main.graphics.GraphicsDevice.SetRenderTarget(target.RenderTarget);
				Main.graphics.GraphicsDevice.Clear(Color.Transparent);

				target.drawFunct(Main.spriteBatch);

				Main.spriteBatch.End();
			}
		}

		Main.graphics.GraphicsDevice.SetRenderTargets(bindings);

		targetSem.Release();
	}

	public override void PostUpdateEverything()
	{
		if (!wasIngame)
		{
			firstResizeTime = 0;
			wasIngame = true;
		}
		else
		{
			firstResizeTime++;
		}

		if (firstResizeTime == 20)
			ResizeScreens(new Vector2(Main.screenWidth, Main.screenHeight));
	}

	private void MenuUpdate(On_Main.orig_UpdateMenu orig)
	{
		if (wasIngame)
		{
			firstResizeTime = 0;
			wasIngame = false;
		}
		else
		{
			firstResizeTime++;
		}

		if (firstResizeTime == 20)
			ResizeScreens(new Vector2(Main.screenWidth, Main.screenHeight));

		orig();
	}
}