using System;
using System.Collections.Generic;


using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;

namespace Bloody_Birds
{
	public class enemy
	{
			
		private SpriteUV 				sprite;
		private TextureInfo				spriteTex, spriteTex2;
		private int 					spWidth, spHeight, scWidth, scHeight, moveSpeed, deadSpeed, deadYspeed;
		private int 					animTimer;
		private Vector2					deadR;
		private bool					dead, infinite;
		//Direction = True = Start from the left, move to right.
		//Direction = False = Start from right, move to left.
		private bool					direction;
		

		
		
		public enemy (Scene scene, System.Random rand, bool inf)
		{
			
			dead = false;
			
			spriteTex = new TextureInfo("Application/Graphics/Brown_Bird_Wings_Up.png");
			spriteTex2 = new TextureInfo("Application/Graphics/Brown_Bird_Wings_Down.png");
			sprite = new SpriteUV(spriteTex);
			spWidth = sprite.TextureInfo.Texture.Width;
			spHeight = sprite.TextureInfo.Texture.Height;
			scWidth = Director.Instance.GL.Context.GetViewport().Width;
			scHeight = Director.Instance.GL.Context.GetViewport().Height;
			moveSpeed = rand.Next(1, 4);
			sprite.FlipU = true;
			deadYspeed = rand.Next (1, 20);
			deadYspeed = deadYspeed-(deadYspeed*2);
			sprite.RotationNormalize = new Vector2(1.0f, 0.0f);
			deadR = new Vector2(1.0f, 0.0f);
			direction = false;
			sprite.Position = new Vector2(scWidth + (spWidth + rand.Next(0, 2048)), 0 + (rand.Next (0 + spHeight, scHeight - spHeight)));
			animTimer = rand.Next(1, 60);
			sprite.Quad.S 	= spriteTex.TextureSizef;
			this.infinite = inf;
			//add the bird to the scene
			scene.AddChild(sprite);
			
		
		}
		
		
		public void Update(float deltaTime, Random rand)
		{
			if(!dead)
			{
				//Move bird depending on what direction it is facing (left/right)
				if(direction)
				{
					sprite.Position = new Vector2(sprite.Position.X + moveSpeed, sprite.Position.Y);
				} else if (!direction)
				{
					sprite.Position = new Vector2(sprite.Position.X - moveSpeed, sprite.Position.Y);
				}
				
				animTimer++;
				if(animTimer >= 5 && animTimer < 10)
				{
					sprite.TextureInfo = spriteTex2;
					sprite.Quad.S 	= spriteTex2.TextureSizef;
				} else if (animTimer >= 10)
				{
					sprite.TextureInfo = spriteTex;
					sprite.Quad.S 	= spriteTex.TextureSizef;
					animTimer = 0;
				}
				
			} else if (dead)
			{
				sprite.Position = new Vector2(sprite.Position.X - moveSpeed/20, sprite.Position.Y - deadYspeed);
				deadYspeed++;
				Console.WriteLine(sprite.Position.X);
				Console.WriteLine(sprite.Position.Y);
				sprite.RotationNormalize = deadR;
				deadR.Y -= 0.1f;
			}
			
			if(infinite == true && sprite.Position.Y < 0 - spHeight)
			{
				sprite.Position = new Vector2(scWidth + (spWidth + rand.Next(256, 1024)), 0 + (rand.Next (0 + spHeight, scHeight - spHeight)));
				dead = false;
				sprite.RotationNormalize = new Vector2(1.0f, 0.0f);
				moveSpeed = rand.Next(1, 4);
				deadR = new Vector2(1.0f, 0.0f);
				deadYspeed = rand.Next (1, 20);
				deadYspeed = deadYspeed-(deadYspeed*2);
			}
			
		}
		
		public void Dispose()
		{
			spriteTex.Dispose();
		}
		
		public int getX()
		{
			return (int)sprite.Position.X;
		}
		
		public int getY()
		{
			return (int)sprite.Position.Y;
		}
		
		public int getWidth()
		{
			return (int)sprite.TextureInfo.Texture.Width;
		}
		
		public int getHeight()
		{
			return (int)sprite.TextureInfo.Texture.Height;
		}
		
		public void setDead(bool t)
		{
			dead = t;
		}
		
		public bool getDead()
		{
			return dead;
		}
		
		
	}
}

