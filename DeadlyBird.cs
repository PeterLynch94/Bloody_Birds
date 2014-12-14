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
			text = new TextureInfo("/Application/textures/bird.png");
			
			
			
			enemy = new SpriteUV(text);
			enemy.Quad.S = text.TextureSizef;
			
			int xPos = r.Next(0, 900); //moving birds off screen
			int yPos = r.Next(50, 480);
			speed = r.Next(1, 3);
			enemy.Position = new Vector2(xPos, yPos);
			scene.AddChild(enemy);
		}
		
		public void Update(float deltaTime)
		{
			if(enemy.Position.X < 920)
			{
			enemy.Position = new Vector2(enemy.Position.X + speed, enemy.Position.Y); //setting new position for enemy
			}
			else
			{
					enemy.Position = new Vector2(r.Next(0, 100), r.Next(0, 520)); //re-popping birds		
			}
			
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
			
			if((touchRect.X > sunRect.X) && (touchRect.X < sunRect.X + sunRect.Width)|| (touchRect.Y > sunRect.Y) && (touchRect.Y < sunRect.Y + sunRect.Height))
			{
				 
				removeBird();  
 
			}
			
						
		}
		
		public void removeBird()
		{
			AppMain.EnemyList.Remove(this);
			theScene.RemoveChild(enemy, true);
		}
	}
}

