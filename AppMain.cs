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
using Sce.PlayStation.Core.Audio;

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
	OPTION = 4,
	
	//Level Select Screen
	LEVELS = 5
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
		private static Sce.PlayStation.HighLevel.UI.Button				optionB, mainGameB, startB, scoreB, highScoreB, musicB, soundB, levelB;
		private static Sce.PlayStation.HighLevel.UI.Button[]			levels;
		
		private static Input2.TouchData									toucher;
		private	static TouchData										touchT;
		
		private static BgmPlayer										musicPlayer;
		private static SoundPlayer[]									soundPlayer;
		private static Bgm[]											tunes;
		private static Sound[]											sounds;
		
		private static enemy[]											enemies;
		
		private static bool 				quitGame, musicToggle, soundToggle;
		private static int 					score, timer;
		private static string				scoreString;
		private static gS					gameState;
		private static int[] 				scoreBoard;
		private static int 					scoreSlotCount, screenW, screenH, levelCount, enemyCount;
		
		private static System.Random 		rand;
		
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
			levelCount = 10;
			enemyCount = 10;
			soundToggle = true;
			musicToggle = true;
			scoreSlotCount = 6;
			scoreBoard = new int[scoreSlotCount];
			scoreString = score.ToString(scoreString);
			scorePath = "/Documents/HighScores.txt";
			load (scorePath, scoreBoard);
			levels = new Button[levelCount];
			//toucher = new Input2.TouchData();
			touchT = new TouchData();
			rand = new Random();
			
			tunes = new Bgm[3];
			
			tunes[0] = new Bgm("/Application/Music/menumusic.mp3");
			tunes[1] = new Bgm("/Application/Music/levelmusic.mp3");
			tunes[2] = new Bgm("/Application/Music/scoremusic.mp3");
			musicPlayer = tunes[0].CreatePlayer();
			musicPlayer.Volume = 0.05f;
			musicPlayer.Loop = true;
			musicPlayer.Play ();
			
			sounds = new Sound[2];
			soundPlayer = new SoundPlayer[2];
			
			
			sounds[0] = new Sound("/Application/Music/buttonbeep.wav");
			sounds[1] = new Sound("/Application/Music/shoot.wav");
			soundPlayer[0] = sounds[0].CreatePlayer();
			soundPlayer[1] = sounds[1].CreatePlayer();
			soundPlayer[0].Volume = 0.05f;
			soundPlayer[1].Volume = 0.05f;
			
			
			
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
			initButtons(panel);
		

			
			
			
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
				var touchT = Touch.GetData(0).ToArray();
				int touchX = -100;
				int touchY = -100;

				
				if(touchT.Length > 0)
				{
					touchX = (int)((touchT[0].X + 0.5f) * screenW);
					touchY = screenH-(int)((touchT[0].Y + 0.5f) * screenH);
				}
				if(touchT.Length >0 && touchT[0].Status == TouchStatus.Down)
			    {
			        Console.WriteLine(touchX);
					Console.WriteLine(touchY);
			    }
				for(int i = 0; i <= enemyCount - 1; i++)
				{
					enemies[i].Update(0.0f);
					if(touchX <= (enemies[i].getWidth() + enemies[i].getX()) && touchX >= enemies[i].getX() && touchY <= (enemies[i].getHeight() + enemies[i].getY())
					   && touchY >= enemies[i].getY() && enemies[i].getDead() == false)
					{
						enemies[i].setDead(true);
						soundPlayer[1].Play ();
						score++;
					}
				}
				if(score >= enemyCount)
				{
					scoreScreenSetup ();
					killEnemies();
				}
				
				
				
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
		
		
		
		public static void levelSetup(int l)
		{
			if(l == 1)
			{
				makeEnemies(5);
			} else if (l == 2)
			{
				makeEnemies(10);
			} else if(l == 3)
			{
				makeEnemies(20);
			} else if(l == 4)
			{
				makeEnemies(30);
			} else if(l == 5)
			{
				makeEnemies(40);
			} else if(l == 6)
			{
				makeEnemies(50);
			} else if(l == 7)
			{
				makeEnemies(70);
			} else if(l == 8)
			{
				makeEnemies(90);
			} else if(l == 9)
			{
				makeEnemies(100);
			}else if (l == 10)
			{
				makeEnemies(200);
			}else if (l < 1 || l > 10)
			{
				makeEnemies(2);
			}
			
		}
		
		
		
		public static void makeEnemies(int n)
		{
			enemies = new enemy[n];
			enemyCount = n;
			for(int i = 0; i <= enemyCount - 1; i++)
			{
				enemies[i] = new enemy(gameScene, rand);
			}
		}
		
		public static void killEnemies()
		{
			for(int i = 0; i <= enemyCount - 1; i++)
			{
				enemies[i].Dispose();
			}
			gameScene.RemoveAllChildren(true);
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
		
		public static Sce.PlayStation.HighLevel.UI.Button makeButton(Sce.PlayStation.HighLevel.UI.Button b, Panel p, int height, int width, float x, float y, String name, String text, bool vis)
		{
			b = new Sce.PlayStation.HighLevel.UI.Button();
			b.HorizontalAlignment = HorizontalAlignment.Center;
			b.VerticalAlignment = VerticalAlignment.Middle;
			b.SetPosition(x, y);
			b.Name = name;
			b.Text = text;
			b.Visible = vis;
			b.Height = height;
			b.Width = width;
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
		
		
		public static void scoreScreenSetup()
		{
			gameState = gS.SCORE;
			scoreB.Visible = false;
			highScoreB.Visible = true;
			titleLabel.Text = "Score Screen";
			scoreCalc();
			save (scorePath, scoreBoard);
			if(musicToggle)
			{
				musicPlayer.Dispose();
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
				if(soundToggle)
				{
					soundPlayer[0].Play();
				}
				if(gameState != gS.OPTION)
				{
					if(musicToggle)
					{
						changeSong(tunes[0]);
					}
				}
				gameState = gS.START;
				musicB.Visible = false;
				soundB.Visible = false;
				startB.Visible = false;
				mainGameB.Visible = true;
				optionB.Visible = true;
				titleLabel.Text = "Start Screen";
				levelB.Visible = true;
				score = 0;
				for(int i = 0; i < scoreSlotCount - 1; i++)
				{
					scoreBoardLabels[i].Visible = false;
				}
				
					
            };
			
			mainGameB.ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
				gameState = gS.GAME;
				startB.Visible = false;
				optionB.Visible = false;
				mainGameB.Visible = false;
				titleLabel.Text = "Main Game Screen";
				scoreLabel.Visible = true;
				levelB.Visible = false;
				if(musicToggle)
				{
					changeSong (tunes[1]);
				}
				levelSetup (1);
					
            };
			
			scoreB.ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
				gameState = gS.SCORE;
				scoreB.Visible = false;
				highScoreB.Visible = true;
				titleLabel.Text = "Score Screen";
				scoreCalc();
				save (scorePath, scoreBoard);
				if(musicToggle)
				{
					musicPlayer.Dispose();
				}
            };
			
			highScoreB.ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
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
				if(musicToggle)
					{
						changeSong (tunes[2]);
					}
			};
			
			musicB.ButtonAction += (sender, e) => 
			{
				musicToggle = !musicToggle;
				if(musicToggle)
				{
					musicB.Text = "Music ON";
					musicPlayer.Play();
					if(soundToggle)
					{
						soundPlayer[0].Play ();
					}
				} else {
					musicB.Text = "Music OFF";
					musicPlayer.Stop();
					if(soundToggle)
					{
						soundPlayer[0].Play ();
					}
				}
			};
		
			soundB.ButtonAction += (sender, e) =>
			{
				soundToggle = !soundToggle;
				if(soundToggle)
				{
					soundB.Text = "Sound ON";
					soundPlayer[0].Play();
				} else {
					soundB.Text = "Sound OFF";
				}
			};
			
			optionB.ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
				gameState = gS.OPTION;
				mainGameB.Visible = false;
				optionB.Visible = false;
				levelB.Visible = false;
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
			
			levelB.ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
				gameState = gS.LEVELS;
				mainGameB.Visible = false;
				optionB.Visible = false;
				levelB.Visible = false;
				for(int i = 0; i < levelCount; i++)
				{
					levels[i].Visible = true;
				}
				titleLabel.Text = "Level Select";
			};
		}
		
		public static void initButtons(Panel panel)
		{
			mainGameB = makeButton (mainGameB, panel, 50, 200, screenW/2, screenH/2, "Button", "Start Game", true);
			mainGameB.SetPosition(screenW/2 - mainGameB.Width/2, screenH/2 - mainGameB.Height/2);
			
			scoreB = makeButton (scoreB, panel, 50, 200, screenW/2, screenH/2, "Score", "Go to Score", false);
			scoreB.SetPosition(screenW/2 - mainGameB.Width/2, screenH/2 - mainGameB.Height/2);
			
			highScoreB = makeButton (highScoreB, panel, 50, 200, screenW/2, screenH/2, "High Score", "Go to High Score", false);
			highScoreB.SetPosition(screenW/2 - mainGameB.Width/2, screenH/2 - mainGameB.Height/2);
			
			startB = makeButton (startB, panel, 50, 200, screenW/2, screenH/2, "Start", "Go to Start", false);
			startB.SetPosition(screenW/2 - mainGameB.Width/2, screenH/2 - mainGameB.Height/2);
			
			musicB = makeButton (musicB, panel, 50, 200, screenW/2, screenH/2, "Music", "Music ON", false);
			musicB.SetPosition(screenW - (mainGameB.Width + 100), screenH/2 - mainGameB.Height/2);
			
			soundB = makeButton (soundB, panel, 50, 200, screenW/2, screenH/2, "Sound", "Sound ON", false);
			soundB.SetPosition(100, screenH/2 - mainGameB.Height/2);
			
			optionB = makeButton (optionB, panel, 50, 200, screenW/2, screenH/2, "Options", "Options", true);
			optionB.SetPosition(screenW - (optionB.Width + 50), 50);
			
			levelB = makeButton(levelB, panel, 50, 200, screenW/2, screenH/2, "LevelSelect", "Level Select", true);
			levelB.SetPosition(screenW/2 - levelB.Width/2, screenH/1.5f - levelB.Height/2);
			
			int j = 0;
			int k = 0;
			for(int i = 0; i <= levelCount - 1; i++)
			{
				levels[i] = makeButton(levels[i], panel, 50, 200, screenW/2, screenH/2, "Level", "Level " + (i+1).ToString(), false);
				if(i < 3)
				{
					levels[i].SetPosition (screenW/30 + (levels[i].Width/2 + i*250), screenH/4);
				} else if (i < 6)
				{
					levels[i].SetPosition (screenW/30 + (levels[i].Width/2 + j*250), screenH/2.5f - levels[i].Height/4);
					j++;
				} else if (i < 9)
				{
					levels[i].SetPosition (screenW/30 + (levels[i].Width/2 + k*250), screenH/2);
					k++;
				} else if (i == 9)
				{
					levels[i].SetPosition (screenW/2 - levels[i].Width/2, screenH/1.5f);
				}
			}
			
		}
		
		public static void changeSong(Bgm m)
		{
				musicPlayer.Dispose();
				musicPlayer = m.CreatePlayer();
				musicPlayer.Loop = true;
				musicPlayer.Play ();
				musicPlayer.Volume = 0.05f;
		}
	}
}
