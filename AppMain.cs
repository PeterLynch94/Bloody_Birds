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
		private static Sce.PlayStation.HighLevel.UI.Label[]				scoreBoardLabels;
		private static Sce.PlayStation.HighLevel.UI.Button				testA, testB;
		
		
		private static bool 				quitGame, touched;
		private static int 					score, timer;
		private static string				scoreString;
		private static gS					gameState;
		private static int[] 				scoreBoard;
		private static int 					scoreSlotCount;
		
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
				UISystem.Update(Touch.GetData(0));
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
			
			//Setup Panel
			Panel panel  = new Panel();
			panel.Width  = Director.Instance.GL.Context.GetViewport().Width;
			panel.Height = Director.Instance.GL.Context.GetViewport().Height;
			
			//Setup Labels
			scoreLabel = makeLabel(scoreLabel, panel, (panel.Width/2) - 300, (panel.Height/2) + 2);
			scoreLabel.Visible = false;
			titleLabel = makeLabel(titleLabel, panel, panel.Width/2, 50);
			scoreBoardLabels = new Sce.PlayStation.HighLevel.UI.Label[scoreSlotCount];
			for(int i = 0; i < scoreSlotCount - 1; i++)
			{
				scoreBoardLabels[i] = makeLabel(scoreBoardLabels[i], panel, 50, i*100);
			}
			
			
			
			
			//Setup buttons
			testB = makeButton (testB, panel, panel.Width/2, panel.Height/2);
			testB.Name = "TestB";
			testB.Text = "Test Button";
			//testB.SetPosition(panel.Width/2, panel.Height/2);
			testB.SetPosition((panel.Width/2) - testB.Width/2, panel.Height/2);
			testB.Height = 100;
			testB.Width = 300;
			testB.ButtonAction += (sender, e) => 
			{
					score++;
            };
			panel.AddChildLast(testB);
			
			testA = makeButton (testA, panel, panel.Width/2 + 300, panel.Height/2 - 200);
			testA.Name = "TestA";
			testA.ButtonAction += (sender, e) => 
			{
				gameState = gS.GAME;
				timer = 10;
				scoreLabel.Visible = true;
				titleLabel.Text = "Main Game Screen";
			};
			panel.AddChildLast(testA);
			
			
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
			


			
			//Set scorelabel to the current value of Score
			scoreLabel.Text = score.ToString ();
			
			//Timer controls how often a touch can be registered,
			//a touch is recognised only when timer <= 0
//			if(timer > 0)
//			timer--;
			
			//gs.Start = Start screen
			if(gameState == gS.START)
			{
				titleLabel.Text = "Start Screen";
				testA.Text = "Go to Game";
				testB.Visible = false;
				testA.Visible = true;
				
				testA.ButtonAction += (sender, e) => 
				{
					gameState = gS.GAME;
					timer = 10;
					scoreLabel.Visible = true;
					titleLabel.Text = "Main Game Screen";
					testB.Visible = true;
				};
				
				
			}
			
			//gs.GAME = main game screen
			if(gameState == gS.GAME)
			{
				testA.ButtonAction += (sender, e) => 
				{
					gameState = gS.SCORE;
					timer = 50;
					scoreCalc();
					titleLabel.Text = "Score Screen";
					testB.Visible = false;
				};
			}
			
			//gs.SCORE = post defeat/victory score screen
			if(gameState == gS.SCORE)
			{
				testA.ButtonAction += (sender, e) => 
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
					titleLabel.Text = "High Score Screen";
				};
				
			}
			
			//gs.HSCORE = end of game score screen, loops back to start screen
			if(gameState == gS.HSCORE)
			{
				
				testA.ButtonAction += (sender, e) => 
				{
					gameState = gS.START;
					timer = 50;
					score = 0;
					for(int i = 0; i < scoreSlotCount - 1; i++)
					{
						scoreBoardLabels[i].Visible = false;
					}
				};
			}
			
			if(gameState == gS.OPTION)
			{
				
			}
				
		}
		
		
		//Iterates through each position on the score board checking to see if the new score is higher than the one stored there
		//If yes it is replaced, else it stays
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
		
		public static Sce.PlayStation.HighLevel.UI.Label makeLabel(Sce.PlayStation.HighLevel.UI.Label l, Panel p, float w, float h)
		{
			l = new Sce.PlayStation.HighLevel.UI.Label();
			l.HorizontalAlignment = HorizontalAlignment.Center;
			l.VerticalAlignment = VerticalAlignment.Top;
			l.SetPosition(w, h);
			l.Text = "";
			p.AddChildLast(l);
			uiScene.RootWidget.AddChildLast(p);
			return l;
		}
		
		public static Sce.PlayStation.HighLevel.UI.Button makeButton(Sce.PlayStation.HighLevel.UI.Button b, Panel p, float w, float h)
		{
			b = new Sce.PlayStation.HighLevel.UI.Button();
			b.HorizontalAlignment = HorizontalAlignment.Center;
			b.VerticalAlignment = VerticalAlignment.Middle;
			b.SetPosition(w, h);
			b.Text = "unset";
			b.Height = 50;
			b.Width = 150;
			return b;
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
	}
}
