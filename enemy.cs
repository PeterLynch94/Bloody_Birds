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
		private TextureInfo				spriteTex;
		private int 					spWidth, spHeight, scWidth, scHeight, moveSpeed;	
		private bool					dead;
		//Direction = True = Start from the left, move to right.
		//Direction = False = Start from right, move to left.
		private bool					direction;
		

		
		
		public enemy (Scene scene, System.Random rand)
		{
			
			dead = false;
			
			spriteTex = new TextureInfo("Application/Graphics/enemy.png");;
			sprite = new SpriteUV(spriteTex);
			spWidth = sprite.TextureInfo.Texture.Width;
			spHeight = sprite.TextureInfo.Texture.Height;
			scWidth = Director.Instance.GL.Context.GetViewport().Width;
			scHeight = Director.Instance.GL.Context.GetViewport().Height;
			moveSpeed = rand.Next(1, 4);
			
			
			direction = false;
			sprite.Position = new Vector2(scWidth + (spWidth + rand.Next(0, 2048)), 0 + (rand.Next (0 + spHeight, scHeight - spHeight)));

			sprite.Quad.S 	= spriteTex.TextureSizef;

			//add the bird to the scene
			scene.AddChild(sprite);
			
		
		}
		
		
		public void Update(float deltaTime)
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
			} else if (dead)
			{
				sprite.Position = new Vector2(-200, -200);
			}
			
//			if((direction == true && sprite.Position.X > scWidth+100) || (direction == false && sprite.Position.X < -100))
//			{
//				setRandPos (rand);
//				moveSpeed = rand.Next(1, 4);
//			}
//			
			
			//Console.WriteLine (rand.Next(0, 7));
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
		
		public void setRandPos(System.Random rand)
		{
			int random;
			random = rand.Next(1,3);
			if(random == 1)
			{
				direction = true;
				sprite.Position = new Vector2(0 - (spWidth + rand.Next(0, 128)), scHeight - 100.0f);
				sprite.FlipU = false;
			} else if (random == 2) {
				direction = false;
				sprite.Position = new Vector2(scWidth + (spWidth + rand.Next(0, 128)), scHeight - 100.0f);
				sprite.FlipU = true;
			}
		}
	}
}

