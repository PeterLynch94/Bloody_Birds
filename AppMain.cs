using System;
using System.Collections.Generic;
using System.IO;

using Sce.PlayStation.Core;
using Sce.PlayStation.Core.Environment;
using Sce.PlayStation.Core.Graphics;
using Sce.PlayStation.Core.Input;

using Sce.PlayStation.HighLevel.GameEngine2D;
using Sce.PlayStation.HighLevel.GameEngine2D.Base;
using Sce.PlayStation.HighLevel.UI;
//REAL
//enum to control game state
enum gS
{
	//Start screen
	START = 0,
	
	//Screen where main game takes place
	GAME = 1,
	
	//Post game score screen
	SCORE = 2,
	
	//High Score table
	HSCORE = 3,
	
	//Options Screen
	OPTION = 4
}

namespace Bloody_Birds
{
	public class AppMain
	{
		private static Sce.PlayStation.HighLevel.GameEngine2D.Scene 	gameScene, startScene, scoreScene, gameOverScene;
		private static Sce.PlayStation.HighLevel.UI.Scene 				uiScene;
		private static Sce.PlayStation.HighLevel.UI.Label				scoreLabel;
		private static Sce.PlayStation.HighLevel.UI.Label				titleLabel;
		private static Sce.PlayStation.HighLevel.UI.Label[]				scoreBoardLabels;
		
		private static bool 				quitGame;
		private static int 					score;
		private static int					timer;
		private static string				scoreString;
		private static gS					gameState;
		private static int[] 				scoreBoard;
		private static int 					scoreSlotCount; // comment
		
		private static string				scorePath;			
		
		public static void Main (string[] args)
		{
			quitGame = false;
			Initialize ();
			
			//Game Loop
			while (!quitGame) 
			{
				Update ();
				Director.Instance.GL.Context.Clear();
				Director.Instance.Update();
				Director.Instance.Render();
				UISystem.Render();
				
				Director.Instance.GL.Context.SwapBuffers();
				Director.Instance.PostSwap();
			}
			//Game ended, time to clean up
		}
		
		public static void Initialize ()
		{
			
			gameState = gS.START;
			
			//initialise score values
			score = 0;
			timer = 0;
			scoreSlotCount = 6;
			scoreBoard = new int[scoreSlotCount];
			scoreString = score.ToString(scoreString);
			scorePath = "/Documents/HighScores.txt";
			load (scorePath, scoreBoard);
			
			Director.Initialize ();
			UISystem.Initialize(Director.Instance.GL.Context);
			
			
			CreateStartGameScene();
			
			// Create game scren
			CreateGameScene();	 
			
			ScoreScene();
			
			GameOverScene();
			
			//Set the ui scene.
			uiScene = new Sce.PlayStation.HighLevel.UI.Scene();
			
			//Setup Panel
			Panel panel  = new Panel();
			panel.Width  = Director.Instance.GL.Context.GetViewport().Width;
			panel.Height = Director.Instance.GL.Context.GetViewport().Height;
			
			//Setup Labels
			scoreLabel = makeLabel(scoreLabel, panel, -300, 2);
			scoreLabel.Visible = false;
			titleLabel = makeLabel(titleLabel, panel, -100, 50);
			scoreBoardLabels = new Sce.PlayStation.HighLevel.UI.Label[scoreSlotCount];
			for(int i = 0; i < scoreSlotCount - 1; i++)
			{
				scoreBoardLabels[i] = makeLabel(scoreBoardLabels[i], panel, 50, i*100);
			}
			UISystem.SetScene(uiScene);
			
			//Run the scene.
			Director.Instance.RunWithScene(startScene, true);
		 
			
			
		}

		public static void Update ()
		{
			
			/*
			 For now, the game is meant to advance from start > game > score > hscore > start
			 then back through
			 
			 Once this works we have proof that this system for changing game screens/states works
			 and I can then move on to menus and the scoring system in more detail
			 
			 
			 13/11 Update
			 The system detailed above works and a high score table has been implemented along with labels for each screen
			 with its title on, these are not neccesarily final names/screens
			 */
			
			// check to see if screen has been touched
			var touch = Touch.GetData (0);
			
			//Set scoreleval to the current value of Score
			scoreLabel.Text = score.ToString ();
			
			//Timer controls how often a touch can be registered,
			//a touch is recognised only when timer <= 0
			if(timer > 0)
			timer--;
			
			//gs.Start = Start screen
			if(gameState == gS.START)
			{
				titleLabel.Text = "Start Screen";
				
				if(touch.Count > 0 && timer <= 0)
				{
					//startScene.RegisterDisposeOnExitRecursive();
					//startScene.RemoveAllChildren(true);
					//Director.Instance.PopScene();
					
					Director.Instance.ReplaceScene(gameScene);
						 
					gameState = gS.GAME;
					timer = 10;
					scoreLabel.Visible = true;
				}
				
			}
			
			//gs.GAME = main game screen
			if(gameState == gS.GAME)
			{
				
				titleLabel.Text = "Main Game Screen";
				score++;
				if(touch.Count > 0 && timer <= 0)
				{
					gameState = gS.SCORE;
					timer = 50;
					scoreCalc();
					
				}
			}
			
			//gs.SCORE = post defeat/victory score screen
			if(gameState == gS.SCORE)
			{
		
				titleLabel.Text = "Score Screen";
			
				if(touch.Count > 0 && timer <= 0)
				{
				
					gameState = gS.HSCORE;
						
					timer = 50;
					scoreLabel.Visible = false;
					for(int i = 0; i < scoreSlotCount - 1; i++)
					{
						scoreBoardLabels[i].Visible = true;
						scoreBoardLabels[i].Text = scoreBoard[i].ToString ();
					}
					save (scorePath, scoreBoard);
					Director.Instance.ReplaceScene(gameOverScene); //draw score
				
				}
				
			}
			
			//gs.HSCORE = end of game score screen, loops back to start screen
			if(gameState == gS.HSCORE)
			{
				titleLabel.Text = "High Score Screen";
				if(touch.Count > 0 && timer <= 0)
				{
						
						Director.Instance.PushScene(startScene); //replace for start screen
					gameState = gS.START;
					timer = 50;
					score = 0;
					for(int i = 0; i < scoreSlotCount - 1; i++)
					{
						scoreBoardLabels[i].Visible = false;
					}
				}
				
			}
		}
		
		public static void scoreCalc()
		{
					
			for(int i = 0; i < scoreSlotCount - 1; i++)
			{
				int temp;
				int temp2;
				if(scoreBoard[i] < score)
				{
					temp = scoreBoard[i];
					scoreBoard[i] = score;
					while(i < scoreSlotCount - 1)
					{
						i++;
						temp2 = scoreBoard[i];
						scoreBoard[i] = temp;
						temp = temp2;
					}
				}
			}
		}
		
		public static Sce.PlayStation.HighLevel.UI.Label makeLabel(Sce.PlayStation.HighLevel.UI.Label l, Panel p, int w, int h)
		{
			l = new Sce.PlayStation.HighLevel.UI.Label();
			l.HorizontalAlignment = HorizontalAlignment.Center;
			l.VerticalAlignment = VerticalAlignment.Top;
			l.SetPosition(
				Director.Instance.GL.Context.GetViewport().Width/2 + w,
				Director.Instance.GL.Context.GetViewport().Height/8 + h);
			l.Text = "";
			p.AddChildLast(l);
			uiScene.RootWidget.AddChildLast(p);
			return l;
		}
		
		public static void save(string path, int[] scoreB)
		{
			byte[] result = new byte[scoreB.Length * sizeof(int)];
			Buffer.BlockCopy(scoreB, 0, result, 0, result.Length);
			Console.WriteLine("==SaveData()==");

		    int bufferSize=sizeof(Int32)* (scoreSlotCount+1);
		    byte[] buffer = new byte[bufferSize];
		
		    Int32 sum=0;
		    for(int i=0; i<scoreSlotCount; ++i)
		    {
		        Console.WriteLine("ranking[i]="+scoreB[i]);
		        Buffer.BlockCopy(scoreB, sizeof(Int32)*i, buffer, sizeof(Int32)*i, sizeof(Int32));
		        sum+=scoreB[i];
		    }
		
		    Int32 hash=sum.GetHashCode();
		    Console.WriteLine("sum={0},hash={1}",sum,hash);
		
		    Buffer.BlockCopy(BitConverter.GetBytes(hash), 0, buffer, scoreSlotCount * sizeof(Int32), sizeof(Int32));
		        	
			
			using (System.IO.FileStream hStream = System.IO.File.Open(@path, FileMode.Create))
		    {
		        hStream.SetLength((int)bufferSize);
		        hStream.Write(buffer, 0, (int)bufferSize);
		        hStream.Close();
		    }
			
		}
		
		//Function to load data/scores from file
		public static void load(string path, int[] scoreB)
		{
			

            using (System.IO.FileStream hStream = System.IO.File.OpenRead(@path))
			{
                if (hStream != null) 
				{
                    long size = hStream.Length;
	                byte[] buffer = new byte[size];
	                hStream.Read(buffer, 0, (int)size);
	
	
	                Int32 sum=0;
	                for(int i=0; i<scoreSlotCount; ++i)
	                {
	                    Buffer.BlockCopy(buffer, sizeof(Int32)*i, scoreB, sizeof(Int32)*i,  sizeof(Int32));
	                    Console.WriteLine("ranking[i]="+scoreB[i]);
	                    sum+=scoreB[i];
	                }
                }
            }
         }
		
		private static void CreateGameScene()
		{
			//Set game scene
			gameScene = new Sce.PlayStation.HighLevel.GameEngine2D.Scene();
			gameScene.Camera.SetViewFromViewport();
			
			
			TextureInfo text = new TextureInfo("/Application/textures/Background.png");
			SpriteUV background = new SpriteUV(text);
			background.Quad.S = text.TextureSizef; // set size of background
			background.Position = new Vector2(0, 0);
			
			gameScene.AddChild(background);	
		}
		
		private static void CreateStartGameScene()
		{
			startScene = new Sce.PlayStation.HighLevel.GameEngine2D.Scene();
			startScene.Camera.SetViewFromViewport();
			
			TextureInfo startSceneBackground = new TextureInfo("/Application/textures/GameMenu.png");
			SpriteUV sprBackground = new SpriteUV(startSceneBackground);
			sprBackground.Quad.S = startSceneBackground.TextureSizef;
			
			sprBackground.Position = new Vector2(0,0);
			startScene.AddChild(sprBackground);
		}
		
		private static void ScoreScene()
		{
			scoreScene = new Sce.PlayStation.HighLevel.GameEngine2D.Scene();
			scoreScene.Camera.SetViewFromViewport();
			
			TextureInfo scor = new TextureInfo("/Application/textures/Score.png");
			SpriteUV scoreImage = new SpriteUV(scor);
			scoreImage.Quad.S = scor.TextureSizef;
			
			scoreImage.Position = new Vector2(0,0);
			scoreScene.AddChild(scoreImage);
		}
		
		private static void GameOverScene()
		{
			gameOverScene = new Sce.PlayStation.HighLevel.GameEngine2D.Scene();
			gameOverScene.Camera.SetViewFromViewport();
			
			TextureInfo g_score = new TextureInfo("/Application/textures/ScoreBoard.png");
			SpriteUV sprSCore = new SpriteUV(g_score);
			sprSCore.Quad.S = g_score.TextureSizef;
			
			sprSCore.Position = new Vector2(180,150);
			gameOverScene.AddChild(sprSCore);
		}
	}
}
