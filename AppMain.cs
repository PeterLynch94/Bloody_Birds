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
	LEVELS = 5,
	
	//Game Over Screen
	GAMEOVER = 6
}

namespace Bloody_Birds
{
	public class AppMain
	{
		private static Sce.PlayStation.HighLevel.GameEngine2D.Scene 	gameScene;
		private static Sce.PlayStation.HighLevel.UI.Scene 				uiScene;
		private static Sce.PlayStation.HighLevel.UI.Label				scoreLabel;
		private static Sce.PlayStation.HighLevel.UI.Label[]				scoreBoardLabels;
		private static Sce.PlayStation.HighLevel.UI.Button				optionB, mainGameB, startB, scoreB, highScoreB, musicB, soundB, levelB, infiniteB, retryB;
		private static Sce.PlayStation.HighLevel.UI.Button[]			levels;
		private static TextureInfo										menuTX, menuTX2;
		private static SpriteUV											menuBG, menuBG2;
		private static ImageBox											music1, music2, sound1, sound2, backToMenu, retryLevel, nextLevel, highScoreBox, 
																		infiniteBox;
		private static ImageBox[]										levelIcons;
		
		private static BgmPlayer										musicPlayer;
		private static SoundPlayer[]									soundPlayer;
		private static Bgm[]											tunes;
		private static Sound[]											sounds;
		private static List<enemy> 										enemyList;
		
		private static bool 				quitGame, musicToggle, soundToggle, infiniteMode;
		private static int 					score;
		private static string				scoreString;
		private static gS					gameState;
		private static int[] 				scoreBoard;
		private static int 					scoreSlotCount, screenW, screenH, levelCount, enemyCount, lUnlockCount, currentL;
		private static float				volume;
		private static System.Random 		rand;
		
		private static string				scorePath, levelUnlockPath;
		
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
			levelCount = 10;
			lUnlockCount = 1;
			currentL = 1;
			enemyCount = 10;
			enemyList = new List<enemy>();
			soundToggle = true;
			musicToggle = true;
			volume = 0.20f;
			
			
			scoreSlotCount = 6;
			scoreBoard = new int[scoreSlotCount];
			scoreString = score.ToString(scoreString);
			scorePath = "/Documents/HighScores.txt";
			levelUnlockPath = "/Documents/LevelsUnlocked.txt";
			load (scorePath, true);
			load (levelUnlockPath, false);
			levels = new Button[levelCount];
			//toucher = new Input2.TouchData();
			rand = new Random();
			
			tunes = new Bgm[3];
			
			tunes[0] = new Bgm("/Application/Music/menumusic.mp3");
			tunes[1] = new Bgm("/Application/Music/levelmusic.mp3");
			tunes[2] = new Bgm("/Application/Music/scoremusic.mp3");
			musicPlayer = tunes[0].CreatePlayer();
			musicPlayer.Volume = volume;
			musicPlayer.Loop = true;
			if(musicToggle)
			{
				musicPlayer.Play();
			}
			sounds = new Sound[2];
			soundPlayer = new SoundPlayer[2];
			
			
			sounds[0] = new Sound("/Application/Music/buttonbeep.wav");
			sounds[1] = new Sound("/Application/Music/shoot.wav");
			soundPlayer[0] = sounds[0].CreatePlayer();
			soundPlayer[1] = sounds[1].CreatePlayer();
			soundPlayer[0].Volume = volume;
			soundPlayer[1].Volume = volume;
			
			
			
			
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
			scoreBoardLabels = new Sce.PlayStation.HighLevel.UI.Label[scoreSlotCount];
			for(int i = 0; i < scoreSlotCount - 1; i++)
			{
				scoreBoardLabels[i] = makeLabel(scoreBoardLabels[i], panel, 50, i*100);
				scoreBoardLabels[i].SetPosition(screenW/2 - scoreBoardLabels[i].Width/2, 50 + i*70);
			}
			
			
			
			setupMenuGraphics ();
			//Set buttons up
			initImageBoxes(panel);
			initButtons(panel);
			

			
			
			
			uiScene.RootWidget.AddChildLast(panel);
			
			UISystem.SetScene(uiScene);
			
			//Set what the buttons do when they are pressed in this function
			initButtonActions(panel);

			//Run the scene.
			Director.Instance.RunWithScene(gameScene, true);
		}
		
		public static void initImageBoxes(Panel panel)
		{
			levelIcons = new ImageBox[10];
			music1 = makeImageAsset(panel, "Application/Graphics/music_on.png");
			music2 = makeImageAsset(panel, "Application/Graphics/music_off.png");
			sound1 = makeImageAsset(panel, "Application/Graphics/sound_on.png");
			sound2 = makeImageAsset(panel, "Application/Graphics/sound_off.png");
			retryLevel = makeImageAsset(panel, "Application/Graphics/Retry_Level_Button.png");
			nextLevel = makeImageAsset(panel, "Application/Graphics/Next_Level_Button.png");
			backToMenu = makeImageAsset(panel, "Application/Graphics/Menu_Button.png");
			highScoreBox = makeImageAsset(panel, "Application/Graphics/highScore.png");
			infiniteBox = makeImageAsset (panel, "Application/Graphics/infiniteMode.png");
			for(int i = 0; i < 10; i++)
			{
				string path = "Application/Graphics/level" + ((i+1).ToString()) + ".png";
				levelIcons[i] = makeImageAsset(panel, path);
				levelIcons[i].Visible = false;
			}
		}
		
		public static ImageBox makeImageAsset(Panel panel, string path)
		{
			ImageAsset IA = new ImageAsset(path);
			ImageBox IB = new ImageBox();
			IB.Image = IA;
			IB.Visible = true;
			IB.Height = IB.Image.Height;
			IB.Width = IB.Image.Width;
			IB.SetPosition(50,50);
			panel.AddChildLast(IB);
			IB.Visible = false;
			return IB;
			
		}
		
		public static void setupMenuGraphics()
		{
			gameScene.RemoveAllChildren(true);
			menuTX = new TextureInfo("Application/Graphics/Game_Menu.png");
			menuBG = new SpriteUV(menuTX);
			menuBG.Quad.S = menuTX.TextureSizef;
			menuBG.Position = new Vector2(0, 0);
			gameScene.AddChild(menuBG);
		}
		
		public static void setupLevelGraphics(int p)
		{
			gameScene.RemoveAllChildren(true);
			menuTX = new TextureInfo();
			if(p == 0)
			{
				menuTX = new TextureInfo("Application/Graphics/Background.png");
			} else if (p == 1)
			{
				menuTX = new TextureInfo("Application/Graphics/bad_end.png");	
			} else if (p == 2)
			{
				menuTX = new TextureInfo("Application/Graphics/scoreBack.png");
			}
			menuBG = new SpriteUV(menuTX);
			menuBG.Quad.S = menuTX.TextureSizef;
			menuBG.Position = new Vector2(0, 0);
			gameScene.AddChild(menuBG);
		}
		
		public static void setupScoreGraphics()
		{
			gameScene.RemoveAllChildren(true);
			menuTX = new TextureInfo("Application/Graphics/scoreBack.png");
			menuBG = new SpriteUV(menuTX);
			menuBG.Quad.S = menuTX.TextureSizef;
			menuBG.Position = new Vector2(0, 0);
			
			menuTX2 = new TextureInfo("Application/Graphics/score.png");
			menuBG2 = new SpriteUV(menuTX2);
			menuBG2.Quad.S = menuTX2.TextureSizef;
			menuBG2.Position = new Vector2(screenW/2 - menuBG2.TextureInfo.Texture.Width/2, screenH/2 - menuBG2.TextureInfo.Texture.Height/2);
			scoreLabel.SetPosition(menuBG2.Position.X +scoreLabel.Width, menuBG2.Position.Y + scoreLabel.Height*2);
			gameScene.AddChild(menuBG);
			gameScene.AddChild(menuBG2);
		}
		
		public static void Update ()
		{
		
			//Set scorelabel to the current value of Score
			scoreLabel.Text = score.ToString ();

			
			//gs.GAME = main game screen
			if(gameState == gS.GAME)
			{
				var touchT = Touch.GetData(0).ToArray();
				int touchX = -100;
				int touchY = -100;

				
				if(touchT.Length > 0 && touchT[0].Status == TouchStatus.Up)
				{
					touchX = (int)((touchT[0].X + 0.5f) * screenW);
					touchY = screenH-(int)((touchT[0].Y + 0.5f) * screenH);
				}
				foreach(enemy e in enemyList)
				{
					e.Update(0.0f, rand);
					if(touchX <= (e.getWidth() + e.getX()) && touchX >= e.getX() && touchY <= (e.getHeight() + e.getY())
					   && touchY >= e.getY() && e.getDead() == false)
					{
						e.setDead(true);
						if(soundToggle)
						{
							soundPlayer[1].Play ();
						}
						score++;
					}
					if(e.getX() < (0 - e.getWidth()) && infiniteMode == false)
					{
						gameState = gS.GAMEOVER;
						score = 0;
						killEnemies();
						setupGameOver();
						break;
					} else if (e.getX() < (0 - e.getWidth()) && infiniteMode == true)
					{
						infiniteMode = false;
						scoreScreenSetup ();
						killEnemies();
						setupScoreGraphics();
						break;
					}
				}
				if(score >= enemyCount && infiniteMode == false)
				{
					scoreScreenSetup ();
					killEnemies();
					if(currentL >= lUnlockCount)
					{
						lUnlockCount++;
						save (levelUnlockPath, false);
					}
					setupScoreGraphics();
				}		
			}
				
		}
		
		public static void setupGameOver()
		{
			if(musicToggle)
			{
				musicPlayer.Dispose();
			}
			setupLevelGraphics(1);
			retryB.Visible = true;
			retryLevel.Visible = true;
			backToMenu.Visible = true;
			startB.Visible = true;
			startB.Width = backToMenu.Width;
			startB.Height = backToMenu.Height;
			startB.SetPosition(screenW/2 - startB.Width/2, screenH - startB.Height);
			startB.Alpha = 0.0f;
			backToMenu.SetPosition(startB.X, startB.Y);
			backToMenu.Visible = true;
			
			
			
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
			}else if (l == 11)
			{
				makeEnemies(50);
			}
			
		}
		
		
		
		public static void makeEnemies(int n)
		{
			enemyList = new List<enemy>();
			enemyCount = n;
			for(int i = 0; i <= enemyCount - 1; i++)
			{
				if(infiniteMode == false)
				{
					enemy en = new enemy(gameScene, rand, false);
					enemyList.Add (en);
				} else if (infiniteMode == true)
				{
					enemy en = new enemy(gameScene, rand, true);
					enemyList.Add (en);
				}
				
			}
		}
		
		public static void killEnemies()
		{
			foreach(enemy e in enemyList)
			{
				e.Dispose();
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
		
		public static void save(string path, bool p)
		{
			byte[] result = new byte[0];
			int bufferSize = 0;
			byte[] buffer = new byte[0];
			if(p)
			{
				result = new byte[scoreBoard.Length * sizeof(int)];
				Buffer.BlockCopy(scoreBoard, 0, result, 0, result.Length);
	
			     bufferSize=sizeof(Int32)* (scoreSlotCount+1);
			     buffer = new byte[bufferSize];
			
			    Int32 sum=0;
			    for(int i=0; i<scoreSlotCount; ++i)
			    {
			        Buffer.BlockCopy(scoreBoard, sizeof(Int32)*i, buffer, sizeof(Int32)*i, sizeof(Int32));
			        sum+=scoreBoard[i];
			    }
			
			    Int32 hash=sum.GetHashCode();
			    Console.WriteLine("sum={0},hash={1}",sum,hash);
			
			    Buffer.BlockCopy(BitConverter.GetBytes(hash), 0, buffer, scoreSlotCount * sizeof(Int32), sizeof(Int32));
			} else if (!p)
			{
				int[] uCount = new int[1];
				uCount[0] = lUnlockCount;
				result = new byte[uCount.Length * sizeof(int)];
				Buffer.BlockCopy(uCount, 0, result, 0, result.Length);
	
			     bufferSize=sizeof(Int32)* (1+1);
			     buffer = new byte[bufferSize];
			
			    Int32 sum=0;
			    for(int i=0; i<1; ++i)
			    {
			        Buffer.BlockCopy(uCount, sizeof(Int32)*i, buffer, sizeof(Int32)*i, sizeof(Int32));
			        sum+=uCount[i];
			    }
			
			    Int32 hash=sum.GetHashCode();
			    Console.WriteLine("sum={0},hash={1}",sum,hash);
			
			    Buffer.BlockCopy(BitConverter.GetBytes(hash), 0, buffer, 1 * sizeof(Int32), sizeof(Int32));
			}
			
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
			scoreCalc();
			save (scorePath, true);
			highScoreBox.Visible = true;
			if(musicToggle)
			{
				musicPlayer.Dispose();
			}
		}
		
		public static void gameScreenSetup(int l)
		{
			gameState = gS.GAME;
			startB.Visible = false;
			optionB.Visible = false;
			mainGameB.Visible = false;
			infiniteB.Visible = false;
			infiniteBox.Visible = false;
			if(infiniteMode == true)
			{
				scoreLabel.Visible = true;
			} else {
				scoreLabel.Visible = false;
			}
			levelB.Visible = false;
			if(musicToggle)
			{
				changeSong (tunes[1]);
			}
			for(int i = 0; i < levelCount; i++)
			{
				levels[i].Visible = false;
				levelIcons[i].Visible = false;
			}
			backToMenu.Visible = false;
			setupLevelGraphics(0);
			levelSetup(l);
		}
		
		public static void levelSelectSetup()
		{
			gameState = gS.LEVELS;
			mainGameB.Visible = false;
			optionB.Visible = false;
			levelB.Visible = false;
			for(int i = 0; i < lUnlockCount; i++)
			{
				levels[i].Visible = true;
				levels[i].Alpha = 0.0f;
				levelIcons[i].Visible = true;
			}
		}
		
		
		
		
		
		
		
		
		
		
		//Function to load data/scores from file
		public static void load(string path, bool p)
		{
			
			if(p)
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
		                    Buffer.BlockCopy(buffer, sizeof(Int32)*i, scoreBoard, sizeof(Int32)*i,  sizeof(Int32));
		                    Console.WriteLine("ranking[i]="+scoreBoard[i]);
		                    sum+=scoreBoard[i];
		                }
	                }
				}	
			} else if (!p) {
				int[] uCount = new int[1];
				using (System.IO.FileStream hStream = System.IO.File.OpenRead(@path))
				{
	                if (hStream != null) 
					{
	                    long size = hStream.Length;
		                byte[] buffer = new byte[size];
		                hStream.Read(buffer, 0, (int)size);
		
		
		                Int32 sum=0;
		                for(int i=0; i<1; ++i)
		                {
		                    Buffer.BlockCopy(buffer, sizeof(Int32)*i, uCount, sizeof(Int32)*i,  sizeof(Int32));
		                    Console.WriteLine("levelsUnlocked="+uCount[i]);
		                    sum+=uCount[i];
		                }
						lUnlockCount = uCount[0];
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
				
				infiniteB.Visible = true;
				infiniteBox.Visible = true;
				retryB.Visible = false;
				retryLevel.Visible = false;
				backToMenu.Visible = false;
				sound1.Visible = false;
				sound2.Visible = false;
				music1.Visible = false;
				music2.Visible = false;
				setupMenuGraphics();
				mainGameB.Visible = true;
				optionB.Visible = true;
				levelB.Visible = true;
				score = 0;
				for(int i = 0; i < scoreSlotCount - 1; i++)
				{
					scoreBoardLabels[i].Visible = false;
				}
				
				for(int i = 0; i < levelCount; i++)
				{
					levels[i].Visible = false;
					levelIcons[i].Visible = false;
				}
				
					
            };
			
			mainGameB.ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
				
				gameScreenSetup(1);
					
            };
			
			retryB.ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
				
				gameScreenSetup(currentL);
				retryB.Visible = false;
				retryLevel.Visible = false;
					
            };
			
			infiniteB.ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
				scoreLabel.SetPosition(screenW/2 - scoreLabel.Width/2, screenH/3 - scoreLabel.Height/2);
				currentL = 11;
				infiniteMode = true;
				gameScreenSetup(11);
				
					
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
				scoreCalc();
				save (scorePath, true);
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
				menuBG2.Visible = false;
				highScoreB.Visible = false;
				highScoreBox.Visible = false;
				startB.Visible = true;
				backToMenu.Visible = true;
				backToMenu.SetPosition(screenW/2 - backToMenu.Width/2, screenH-backToMenu.Height);
				startB.SetPosition(backToMenu.X, backToMenu.Y);
				startB.Visible = true;
				startB.Alpha = 0.0f;
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
					music1.Visible = true;
					music2.Visible = false;
				} else {
					musicB.Text = "Music OFF";
					musicPlayer.Stop();
					if(soundToggle)
					{
						soundPlayer[0].Play ();
					}
					music1.Visible = false;
					music2.Visible = true;
				}
			};
		
			soundB.ButtonAction += (sender, e) =>
			{
				soundToggle = !soundToggle;
				if(soundToggle)
				{
					soundB.Text = "Sound ON";
					sound1.Visible = true;
					sound2.Visible = false;
					soundPlayer[0].Play();
				} else {
					soundB.Text = "Sound OFF";
					sound1.Visible = false;
					sound2.Visible = true;
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
				setupLevelGraphics(0);
				startB.Visible = true;
				startB.Width = backToMenu.Width;
				startB.Height = backToMenu.Height;
				startB.SetPosition(screenW/2 - startB.Width/2, screenH/2 - startB.Height/2 + 25);
				startB.Alpha = 0.0f;
				backToMenu.SetPosition(startB.X, startB.Y);
				backToMenu.Visible = true;
				if(musicToggle)
				{
					musicB.Text = "Music ON";
					music1.Visible = true;
				} else {
					musicB.Text = "Music OFF";
					music2.Visible = true;
				}
				if(soundToggle)
				{
					soundB.Text = "Sound ON";
					sound1.Visible = true;
				} else {
					soundB.Text = "Sound OFF";
					sound2.Visible = true;
				}
				
			};
			
			levelB.ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
				levelSelectSetup();
				setupLevelGraphics(0);
				startB.SetPosition(screenW/1.2f - startB.Width/2, screenH - 90);
				startB.Visible = true;
				startB.Width = backToMenu.Width;
				startB.Height = backToMenu.Height;
				startB.Alpha = 0.0f;
				backToMenu.SetPosition(startB.X, startB.Y);
				backToMenu.Visible = true;
			};
		
			initLevelButtons();
			

		}
		
		public static void initButtons(Panel panel)
		{
			mainGameB = makeButton (mainGameB, panel, 50, 150, screenW/2, screenH/2, "Button", "Start Game", true);
			mainGameB.SetPosition(screenW/2 - mainGameB.Width/2 + 5, screenH/2 - mainGameB.Height-10);
			

			mainGameB.Alpha = 0.000001f;
			
			scoreB = makeButton (scoreB, panel, 50, 200, screenW/2, screenH/2, "Score", "Go to Score", false);
			scoreB.SetPosition(screenW/2 - mainGameB.Width/2, screenH/2 - mainGameB.Height/2);
			
			highScoreB = makeButton (highScoreB, panel, 50, 200, screenW/2, screenH/2, "High Score", "Go to High Score", false);
			highScoreB.SetPosition(screenW/2 - mainGameB.Width/2, screenH/1.2f - mainGameB.Height/2);
			highScoreB.Alpha = 0.0f;
			highScoreBox.Width = highScoreB.Width;
			highScoreBox.Height = highScoreB.Height;
			highScoreBox.SetPosition(highScoreB.X, highScoreB.Y);

			
			
			
			startB = makeButton (startB, panel, 50, 200, screenW/2, screenH/2, "Start", "Go to Start", false);
			startB.SetPosition(screenW/2 - mainGameB.Width/2, screenH/2 - mainGameB.Height/2);
			startB.Width = 200;
			startB.Height = 50;

			
			musicB = makeButton (musicB, panel, 50, 200, screenW/2, screenH/2, "Music", "Music ON", false);
			musicB.SetPosition(screenW - (mainGameB.Width + 150), screenH/2 - musicB.Height/2);
			musicB.Height = music1.Height;
			musicB.Width = music1.Width;
			musicB.Alpha = 0.0f;
			music1.SetPosition(musicB.X, musicB.Y);
			music2.SetPosition(musicB.X, musicB.Y);
			music1.Visible = false;
			music2.Visible = false;
			
			
			
			soundB = makeButton (soundB, panel, 50, 200, screenW/2, screenH/2, "Sound", "Sound ON", false);
			soundB.SetPosition(100, screenH/2 - soundB.Height/2);
			soundB.Height = sound1.Height;
			soundB.Width = sound1.Width;
			soundB.Alpha = 0.0f;
			sound1.SetPosition(soundB.X, soundB.Y);
			sound2.SetPosition(soundB.X, soundB.Y);
			sound1.Visible = false;
			sound2.Visible = false;
			
			optionB = makeButton (optionB, panel, 50, 150, screenW/2, screenH/2, "Options", "Options", true);
			optionB.SetPosition(screenW/2 - optionB.Width/2 + 5, screenH/1.5f - optionB.Height/2+35);
			optionB.Alpha = 0.0f;
			
			levelB = makeButton(levelB, panel, 50, 150, screenW/2, screenH/2, "LevelSelect", "Level Select", true);
			levelB.SetPosition(screenW/2 - levelB.Width/2 + 5, screenH/2 - levelB.Height/2+40);
			levelB.Alpha = 0.0f;
			
			infiniteB = makeButton (infiniteB, panel, 50, 150, screenW/2, screenH/2, "Infinity", "Infinite Mode", true);
			infiniteB.SetPosition(0 + infiniteB.Width/2,  screenH - (infiniteB.Height + 50));
			infiniteB.Alpha = 0.0f;
			infiniteBox.SetPosition(infiniteB.X, infiniteB.Y);
			infiniteBox.Visible = true;
			
			retryB = makeButton(retryB, panel, 50, 150, screenW/2, screenH/2, "Retry", "Retry Level", false);
			retryB.Width = retryLevel.Width;
			retryB.Height = retryLevel.Height;
			retryB.SetPosition(screenW/2 - retryB.Width/2, screenH/2 - retryB.Height/2);
			retryB.Alpha = 0.0f;
			retryLevel.SetPosition(retryB.X, retryB.Y);
			retryLevel.Visible = false;
			
			int j = 0;
			int k = 0;
			for(int i = 0; i <= levelCount - 1; i++)
			{
				levels[i] = makeButton(levels[i], panel, 50, 200, screenW/2, screenH/2, i+1.ToString(), "Level " + (i+1).ToString(), false);
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
				levelIcons[i].Width = levels[i].Width;
				levelIcons[i].Height = levels[i].Height;
				levelIcons[i].SetPosition(levels[i].X, levels[i].Y);
			}
			
				

		}
		
		public static void changeSong(Bgm m)
		{
				musicPlayer.Dispose();
				musicPlayer = m.CreatePlayer();
				musicPlayer.Loop = true;
				musicPlayer.Play ();
				musicPlayer.Volume = volume;
		}
		
		public static void initLevelButtons()
		{
			
			levels[0].ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
				
				gameScreenSetup(1);
				currentL = 1;
				
			};
			
			levels[1].ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
				
				gameScreenSetup(2);
				currentL = 2;
			};
			
			levels[2].ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
				
				gameScreenSetup(3);
				currentL = 3;
			};
			
			levels[3].ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
				
				gameScreenSetup(4);
				currentL = 4;
				
			};
			
			levels[4].ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
				
				gameScreenSetup(5);
				currentL = 5;
				
			};
			
			levels[5].ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
				
				gameScreenSetup(6);
				currentL = 6;
				
			};
			
			levels[6].ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
				
				gameScreenSetup(7);
				currentL = 7;
				
			};
			
			levels[7].ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
				
				gameScreenSetup(8);
				currentL = 8;	
			};
			
			levels[8].ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
				
				gameScreenSetup(9);
				currentL = 9;
				
			};
			
			levels[9].ButtonAction += (sender, e) => 
			{
				if(soundToggle)
				{
					soundPlayer[0].Play ();
				}
				
				gameScreenSetup(10);
				currentL = 10;
				
			};
		}
	}
}
