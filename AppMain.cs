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
		private static Sce.PlayStation.HighLevel.UI.Button				optionB, mainGameB, startB, scoreB, highScoreB, musicB, soundB;
		
		
		private static bool 				quitGame, touched, musicToggle, soundToggle;
		private static int 					score, timer;
		private static string				scoreString;
		private static gS					gameState;
		private static int[] 				scoreBoard;
		private static int 					scoreSlotCount, screenW, screenH;
		
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
			soundToggle = true;
			musicToggle = true;
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
			screenH = (int)panel.Height;
			screenW = (int)panel.Width;
			
			//Setup Labels
			scoreLabel = makeLabel(scoreLabel, panel, (screenW/2) - 300, (screenH/2) + 2);
			scoreLabel.Visible = false;
			scoreLabel.SetPosition(screenW/2 - scoreLabel.Width/2, screenH/3 - scoreLabel.Height/2);
			titleLabel = makeLabel(titleLabel, panel, panel.Width/2, 50);
			titleLabel.SetPosition(screenW/2 - titleLabel.Width/2, 50);
			titleLabel.Text = "Start Screen";
			scoreBoardLabels = new Sce.PlayStation.HighLevel.UI.Label[scoreSlotCount];
			for(int i = 0; i < scoreSlotCount - 1; i++)
			{
				scoreBoardLabels[i] = makeLabel(scoreBoardLabels[i], panel, 50, i*100);
				scoreBoardLabels[i].SetPosition(screenW/1.5f - scoreBoardLabels[i].Width/2, 50 + i*100);
			}
			
			
			
			
			//Set buttons up
			mainGameB = makeButton (mainGameB, panel, screenW/2, screenH/2);
			mainGameB.Name = "ButtonB";
			mainGameB.Text = "Start Game";
			mainGameB.SetPosition(screenW/2 - mainGameB.Width/2, screenH/2 - mainGameB.Height/2);
			
			scoreB = makeButton (scoreB, panel, screenW/2, screenH/2);
			scoreB.Name = "Score";
			scoreB.Text = "Go to Score";
			scoreB.Visible = false;
			scoreB.SetPosition(screenW/2 - mainGameB.Width/2, screenH/2 - mainGameB.Height/2);
			
			highScoreB = makeButton (highScoreB, panel, screenW/2, screenH/2);
			highScoreB.Name = "High Score";
			highScoreB.Text = "Go to High Score";
			highScoreB.Visible = false;
			highScoreB.SetPosition(screenW/2 - mainGameB.Width/2, screenH/2 - mainGameB.Height/2);
			
			startB = makeButton (startB, panel, screenW/2, screenH/2);
			startB.Name = "Start";
			startB.Text = "Go to Start";
			startB.Visible = false;
			startB.SetPosition(screenW/2 - mainGameB.Width/2, screenH/2 - mainGameB.Height/2);
			
			musicB = makeButton (musicB, panel, 750, 50);
			musicB.Name = "Start";
			musicB.Text = "Music ON";
			musicB.Visible = false;
			musicB.SetPosition(screenW - (mainGameB.Width + 100), screenH/2 - mainGameB.Height/2);
			
			soundB = makeButton (soundB, panel, 50, 50);
			soundB.Name = "TestA";
			soundB.Text = "Sound ON";
			soundB.Visible = false;
			soundB.SetPosition(100, screenH/2 - mainGameB.Height/2);
			
			optionB = makeButton (optionB, panel, screenW + 300, 50);
			optionB.Name = "TestA";
			optionB.Text = "Options";
			optionB.SetPosition(screenW - (optionB.Width + 50), 50);

			
			
			
			uiScene.RootWidget.AddChildLast(panel);
			
			UISystem.SetScene(uiScene);
			
			//Set what the buttons do when they are pressed in this function
			initButtonActions(panel);
			
			
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
				

			}
			
			//gs.GAME = main game screen
			if(gameState == gS.GAME)
			{
				score++;
			}
			
			//gs.SCORE = post defeat/victory score screen
			if(gameState == gS.SCORE)
			{

				
			}
			
			//gs.HSCORE = end of game score screen, loops back to start screen
			if(gameState == gS.HSCORE)
			{
				
				

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
			l.Height = 50;
			l.Width = 200;
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
			b.Width = 200;
			p.AddChildLast(b);
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
		
		public static void initButtonActions(Panel panel)
		{
			startB.ButtonAction += (sender, e) => 
			{
					gameState = gS.START;
					musicB.Visible = false;
					soundB.Visible = false;
					startB.Visible = false;
					mainGameB.Visible = true;
					optionB.Visible = true;
					titleLabel.Text = "Start Screen";
					gameState = gS.START;
					score = 0;
					for(int i = 0; i < scoreSlotCount - 1; i++)
					{
						scoreBoardLabels[i].Visible = false;
					}
            };
			
			mainGameB.ButtonAction += (sender, e) => 
			{
					gameState = gS.GAME;
					startB.Visible = false;
					optionB.Visible = false;
					mainGameB.Visible = false;
					scoreB.Visible = true;
					titleLabel.Text = "Main Game Screen";
					scoreLabel.Visible = true;
					
            };
			
			scoreB.ButtonAction += (sender, e) => 
			{
					gameState = gS.SCORE;
					scoreB.Visible = false;
					highScoreB.Visible = true;
					titleLabel.Text = "Score Screen";
					scoreCalc();
					save (scorePath, scoreBoard);
            };
			
			highScoreB.ButtonAction += (sender, e) => 
			{
				gameState = gS.HSCORE;
				scoreLabel.Visible = false;
				for(int i = 0; i < scoreSlotCount - 1; i++)
				{
					scoreBoardLabels[i].Visible = true;
					scoreBoardLabels[i].Text = scoreBoard[i].ToString ();
				}

				titleLabel.Text = "High Score Screen";
				highScoreB.Visible = false;
				startB.Visible = true;
			};
			
			musicB.ButtonAction += (sender, e) => 
			{
				musicToggle = !musicToggle;
				if(musicToggle)
				{
					musicB.Text = "Music ON";
				} else {
					musicB.Text = "Music OFF";
				}
			};
		
			soundB.ButtonAction += (sender, e) =>
			{
				soundToggle = !soundToggle;
				if(soundToggle)
				{
					soundB.Text = "Sound ON";
				} else {
					soundB.Text = "Sound OFF";
				}
			};
			
			optionB.ButtonAction += (sender, e) => 
			{
				gameState = gS.OPTION;
				mainGameB.Visible = false;
				optionB.Visible = false;
				startB.Visible = true;
				musicB.Visible = true;
				soundB.Visible = true;
				if(musicToggle)
				{
					musicB.Text = "Music ON";
				} else {
					musicB.Text = "Music OFF";
				}
				if(soundToggle)
				{
					soundB.Text = "Sound ON";
				} else {
					soundB.Text = "Sound OFF";
				}
			};
		}
	}
}
