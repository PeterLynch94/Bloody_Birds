using System;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Graphics;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.Core.Input; // for touch input

namespace Bloody_Birds
{
	public class Enemy
	{
		//private variables
		private SpriteUV[] enemy;
		private TextureInfo	text;
		private TextureInfo	text2;
		private float		width;
	 	private bool hasCollected  	= false;
		private int 		i;
		private Scene theScene;
		private bool isAlive = true;
		float screenX, screenY;
		public Enemy (Scene scene)
		{
			this.theScene = scene;
			i = 1;			
			enemy = new SpriteUV[2];
			
			text = new TextureInfo("/Application/textures/bird.png");
			text2 = new TextureInfo("/Application/textures/bird.png");
			
			//texture for sun
			enemy[0] = new SpriteUV(text);
			enemy[0].Quad.S = text.TextureSizef;
			
			//position sun
			enemy[0].Position = new Vector2(200.0f, 150.0f);
			
			enemy[1] = new SpriteUV(text2);
			enemy[1].Quad.S = text2.TextureSizef;
			
			enemy[1].Position = new Vector2(300.0f, 150.0f);
			
			//Add to the current scene.
			foreach(SpriteUV sprite in enemy)
			scene.AddChild(sprite);
			//scene.AddChild(sun[0]);
			 
			
			 
		}
		
		public void Dispose()
		{
			text.Dispose();
		}
		
		public void Update(float deltaTime, SpriteUV sprite)
		{
			var touches = Touch.GetData(0);
			Rectangle touchRect = new Rectangle();
			//Console.WriteLine(sun[0].Position);
			for(int i = 0; i< touches.Count; i++)
			{
				screenX = (touches[i].X + 0.5f) * AppMain.ScreenWidth;
				screenY = (touches[i].X + 0.5f) * AppMain.ScreenHeight;
				touchRect.X = screenX;
				touchRect.Y = screenY;
				touchRect.Width = 1;
				touchRect.Height = 1;
				//Console.WriteLine(screenX + ":" +screenY);
				
			}
			
			
			Rectangle sunRect = new Rectangle();
			sunRect.X = enemy[0].Position.X;
			sunRect.Y = enemy[0].Position.Y;
			sunRect.Width = enemy[0].CalcSizeInPixels().X;
			sunRect.Height = enemy[0].CalcSizeInPixels().Y;		
			
			if((touchRect.X > sunRect.X) && (touchRect.X < sunRect.X + sunRect.Width) )
			{
				resetEnemy();
			}
			
			enemy[0].Position = new Vector2(enemy[0].Position.X - 2.0f, enemy[0].Position.Y); //setting new position for enemy
			
			if(enemy[0].Position.X < -width) // if enemy is less than width of screen call method
			{
			 resetEnemy();
			}
			else if(sprite.Position.X + sprite.CalcSizeInPixels().X > enemy[0].Position.X
			        && enemy[0].Position.X + enemy[0].CalcSizeInPixels().X > sprite.Position.X //collision detection
			        && !hasCollected)	 // no way for it to be collected
			{
				hasCollected = true; // change when it has collected
				i++;
				Console.WriteLine(i);
				resetEnemy();
				 
			}
			if(!isAlive)
			{
				resetEnemy();
				
			}
						
		}
		
		public void resetEnemy()
		{
			enemy[0].Position =  new Vector2(Director.Instance.GL.Context.GetViewport().Width + 20.0f,  250.0f); //set new position of enemy in middle of screen
			hasCollected = false; // reset enemy
		}
	}
}