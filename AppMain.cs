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
		private static Sce.PlayStation.HighLevel.GameEngine2D.Scene 	gameScene;
		private static Sce.PlayStation.HighLevel.UI.Scene 				uiScene;
		private static Sce.PlayStation.HighLevel.UI.Label				scoreLabel;
		private static Sce.PlayStation.HighLevel.UI.Label				titleLabel;
		private static Sce.PlayStation.HighLevel.UI.Label[]				miscLabels;
		private static Sce.PlayStation.HighLevel.UI.Label[]				scoreBoardLabels;
		
		private static Sce.PlayStation.HighLevel.UI.Button				buttonA;
		
		private static bool 				quitGame;
		private static int 					score;
		private static int					timer;
		private static string				scoreString;
		private static gS					gameState;
		private static int[] 				scoreBoard;
		private static int 					scoreSlotCount;
		
		private static int					screenW, screenH;
		
		private static string				scorePath;			
		
		public static void Main (string[] args)
		{
			quitGame = false;
			Initialize ();
			
			//Game Loop
			while (!quitGame) 
			{
				Update ();
				
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
			
			//Set game scene
			gameScene = new Sce.PlayStation.HighLevel.GameEngine2D.Scene();
			gameScene.Camera.SetViewFromViewport();
			
			//Set the ui scene.
			uiScene = new Sce.PlayStation.HighLevel.UI.Scene();
			
			//Set screenW and screenH to screen width/height respectively for convienience
			
			
			//Setup Panel
			Panel panel  = new Panel();
			panel.Width  = Director.Instance.GL.Context.GetViewport().Width;
			panel.Height = Director.Instance.GL.Context.GetViewport().Height;
			screenW = Director.Instance.GL.Context.GetViewport().Width;
			screenH = Director.Instance.GL.Context.GetViewport().Height;
			
		
			titleLabel = makeLabel(titleLabel, panel, screenW/2 - (100), 100);
			scoreBoardLabels = new Sce.PlayStation.HighLevel.UI.Label[scoreSlotCount];
			for(int i = 0; i < scoreSlotCount - 1; i++)
			{
				scoreBoardLabels[i] = makeLabel(scoreBoardLabels[i], panel, (screenW/2) + 50, 30+i*100);
			}
				//Setup Labels
			scoreLabel = makeLabel(scoreLabel, panel, screenW/2, screenH/2);
			scoreLabel.Visible = false;
			buttonA = new Button();
			buttonA.Name = "buttonPlay";
            buttonA.Text = "Play Game";
            buttonA.Width = 300;
            buttonA.Height = 50;
            buttonA.Alpha = 0.8f;
            buttonA.SetPosition(panel.Width/2 - 150,200.0f);
			buttonA.TouchEventReceived += (sender, e) => {
           Console.WriteLine("It worked");
            };
			panel.AddChildLast(buttonA);
			uiScene.RootWidget.AddChildLast(panel);
			UISystem.SetScene(uiScene);
			
			//Run the scene.
			Director.Instance.RunWithScene(gameScene, true);
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
				}
				
			}
			
			//gs.HSCORE = end of game score screen, loops back to start screen
			if(gameState == gS.HSCORE)
			{
				titleLabel.Text = "High Score Screen";
				if(touch.Count > 0 && timer <= 0)
				{
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
		
		//Sort through the scoreBoard array and see if any scores are less than the new score
		//If yes then sort out the array to store the new score and re arrange the old ones appropriately
		//Else do nothing
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
		
		//Makes a label
		public static Sce.PlayStation.HighLevel.UI.Label makeLabel(Sce.PlayStation.HighLevel.UI.Label l, Panel p, int w, int h)
		{
			l = new Sce.PlayStation.HighLevel.UI.Label();
			l.HorizontalAlignment = HorizontalAlignment.Center;
			l.VerticalAlignment = VerticalAlignment.Top;
			l.SetPosition(w, h);
			l.Text = "";
			p.AddChildLast(l);
			return l;
		}
		
		//Makes a button
		public static Sce.PlayStation.HighLevel.UI.Button makeButton(Sce.PlayStation.HighLevel.UI.Button b, Panel p, int w, int h)
		{
			b = new Button();
			//b.VerticalAlignment = VerticalAlignment.Top;
			//b.SetPosition(w, h);
			//b.Text = "";
			//UIColor u = new UIColor(50.0f, 20.0f, 10.0f, 100.0f);
			//b.BackgroundFilterColor = u;
			//p.AddChildLast(b);
			//uiScene.RootWidget.AddChildLast(p);
			return b;
		}
		
		//saves scores to/from file
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
	}
}
