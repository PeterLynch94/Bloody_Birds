using System;
using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Input;

namespace Bloody_Birds
{
	public class DeadlyBird
	{
		private SpriteUV enemy;
		private TextureInfo	text;
		
		private float		width;
	 	private bool hasCollected  	= false;
		private int 		i, enemys_count;
		private Scene theScene;
		private bool isAlive = true;
		private float screenX, screenY;
		private int speed;
		Random r = new Random();
		
		public DeadlyBird (Scene scene)
		{
			this.theScene = scene;
			
			enemy = new SpriteUV();
			text = new TextureInfo("/Application/textures/DeadBird.png");
			
			
			
			enemy = new SpriteUV(text);
			enemy.Quad.S = text.TextureSizef;
			
			int xPos = r.Next(0, 70);
			int yPos = r.Next(50, 480);
			//speed = r.Next(1, 10);
			enemy.Position = new Vector2(xPos, yPos);
			scene.AddChild(enemy);
		}
		
		public void Update(float deltaTime, SpriteUV sprite)
		{
			enemy.Position = new Vector2(enemy.Position.X + 0.5f, enemy.Position.Y); //setting new position for enemy
			
			
			var touches = Touch.GetData(0);
			Rectangle touchRect = new Rectangle();
			//Console.WriteLine(enemy[0].Position);
			for(int i = 0; i< touches.Count; i++)
			{
				screenX = (touches[i].X + 0.5f) * AppMain.ScreenWidth;
				screenY = (touches[i].X + 0.5f) * AppMain.ScreenHeight;
				touchRect.X = screenX;
				touchRect.Y = screenY;
				touchRect.Width = 1;
				touchRect.Height = 1;
				Console.WriteLine(screenX + ":" +screenY);
				
			}
			
			
			Rectangle sunRect = new Rectangle();
			sunRect.X = enemy.Position.X;
			sunRect.Y = enemy.Position.Y;
			sunRect.Width = enemy.CalcSizeInPixels().X;
			sunRect.Height = enemy.CalcSizeInPixels().Y;		
			
			if((touchRect.X > sunRect.X) && (touchRect.X < sunRect.X + sunRect.Width) )
			{
				resetEnemy();
			}
			
		//	
			if(enemy.Position.X < -width) // if enemy is less than width of screen call method
			{
			 resetEnemy();
			}
			else if(sprite.Position.X + sprite.CalcSizeInPixels().X > enemy.Position.X
			        && enemy.Position.X + enemy.CalcSizeInPixels().X > sprite.Position.X //collision detection
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
			enemy.Position =  new Vector2(Director.Instance.GL.Context.GetViewport().Width + 20.0f,  250.0f); //set new position of enemy in middle of screen
			hasCollected = false; // reset enemy
		}
	}
}

