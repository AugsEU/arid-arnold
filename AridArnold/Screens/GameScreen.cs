using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using AridArnold.Levels;

namespace AridArnold.Screens
{
    internal class GameScreen : Screen
    {
        //============================================
        //  Members
        //--------------------------------------------
        public const int TILE_SIZE = 16;

        private const double END_LEVEL_TIME = 1000.0;
        private const double END_LEVEL_FLASH_TIME = 100.0;
        private const int UI_PANEL_SIZE = 190;

        private List<Level> mLevels;

        private RenderTarget2D mGameArea;
        private RenderTarget2D mLeftUI;
        private RenderTarget2D mRightUI;

        private Texture2D mLifeTexture;
        private Texture2D mUIBG;

        private MonoTimer mLevelEndTimer;

        SpriteFont mPixelFont;

        //============================================
        //  Initialisation
        //--------------------------------------------
        public GameScreen(ContentManager content, GraphicsDeviceManager graphics) : base(content, graphics)
        {
            mLevelEndTimer = new MonoTimer();

            mLevels = new List<Level>();
            mLevels.Add(new CollectWaterLevel("level1-1", 5));
            mLevels.Add(new CollectWaterLevel("level1-2", 2));
            mLevels.Add(new CollectWaterLevel("level1-3", 2));
            mLevels.Add(new CollectFlagLevel("level1-4"));
            mLevels.Add(new CollectWaterLevel("level2-1", 1));
            mLevels.Add(new CollectWaterLevel("level2-2", 3));
            mLevels.Add(new CollectWaterLevel("level2-3", 4));
            mLevels.Add(new CollectFlagLevel("level2-4"));

            mGameArea = null;
            mLeftUI = null;
            mRightUI = null;
        }

        private void LoadLevel(int levelIndex)
        {
            FXManager.I.Clear();
            mLevels[levelIndex].Begin(mContentManager);
        }

        public override void OnActivate()
        {
            TileManager.I.Init(new Vector2(0.0f, TILE_SIZE), TILE_SIZE);
            TileManager.I.CentreX(TileManager.I.GetDrawWidth() + 2 * TILE_SIZE);

            StartLevel();
        }

        public override void OnDeactivate()
        {

        }

        public override void LoadContent(ContentManager content)
        {
            mPixelFont = FontManager.I.GetFont("Pixica Micro-24");
            mLifeTexture = content.Load<Texture2D>("UI/Arnold-Life");
            mUIBG = content.Load<Texture2D>("UI/ui_bg");
        }

        //============================================
        //  Utility
        //--------------------------------------------
        public Level GetCurrentLevel()
        {
            return mLevels[ProgressManager.I.CurrentLevel];
        }

        private void StartLevel()
        {
            LoadLevel(ProgressManager.I.CurrentLevel);
            mLevelEndTimer.FullReset();
            GhostManager.I.StartLevel(mLevels[ProgressManager.I.CurrentLevel]);
        }

        //============================================
        //  Draw
        //--------------------------------------------
        public override RenderTarget2D DrawToRenderTarget(DrawInfo info)
        {
            //Get game rendered as a texture & UI
            RenderGameAreaToTarget(info);
            DrawUIToTarget(info);

            //Draw out the game area
            info.device.SetRenderTarget(mScreenTarget);
            info.device.Clear(new Color(0, 0, 0));

            info.spriteBatch.Begin(SpriteSortMode.Immediate,
                                    BlendState.AlphaBlend,
                                    SamplerState.PointClamp,
                                    DepthStencilState.None,
                                    RasterizerState.CullNone);

            info.spriteBatch.Draw(mUIBG, Vector2.Zero, Color.White);

            Rectangle gameAreaRect = GetGameAreaRect();
            DrawGameArea(info, gameAreaRect);
            DrawUI(info, gameAreaRect);



            info.spriteBatch.End();

            return mScreenTarget;
        }

        private void DrawGameArea(DrawInfo info, Rectangle destRect)
        {
            info.spriteBatch.Draw(mGameArea, destRect, Color.White);
        }

        private Rectangle GetGameAreaRect()
        {
            return new Rectangle((mScreenTarget.Width - mGameArea.Width) / 2, (mScreenTarget.Height - mGameArea.Height) / 2, mGameArea.Width, mGameArea.Height);
        }

        private void RenderGameAreaToTarget(DrawInfo info)
        {
            if(mGameArea == null)
            {
                mGameArea = new RenderTarget2D(info.device, TileManager.I.GetDrawWidth() + 2 * TILE_SIZE, TileManager.I.GetDrawHeight() + TILE_SIZE);
            }

            info.device.SetRenderTarget(mGameArea);

            Color clearCol = ProgressManager.I.GetWorldData().worldColor;

            if(mLevelEndTimer.IsPlaying())
            {
                double timeSinceDeath = mLevelEndTimer.GetElapsedMs();

                if ((int)(timeSinceDeath / END_LEVEL_FLASH_TIME) % 2 == 0)
                {
                    Util.BrightenColour(ref clearCol, 0.05f);
                }
            }

            info.device.Clear(clearCol);

            info.spriteBatch.Begin(SpriteSortMode.Immediate,
                                    BlendState.AlphaBlend,
                                    SamplerState.PointClamp,
                                    DepthStencilState.None,
                                    RasterizerState.CullNone);

            GhostManager.I.Draw(info);
            EntityManager.I.Draw(info);
            TileManager.I.Draw(info);
            FXManager.I.Draw(info);

            info.spriteBatch.End();
        }

        private void DrawUI(DrawInfo info, Rectangle gameAreaRect)
        {
            Rectangle leftRectangle = new Rectangle((gameAreaRect.X - mLeftUI.Width) / 2, gameAreaRect.Y, mLeftUI.Width, mLeftUI.Height);
            Rectangle rightRectangle = new Rectangle((mScreenTarget.Width + gameAreaRect.X + gameAreaRect.Width - mRightUI.Width) / 2, gameAreaRect.Y, mRightUI.Width, mRightUI.Height);

            info.spriteBatch.Draw(mLeftUI, leftRectangle, Color.White);
            info.spriteBatch.Draw(mRightUI, rightRectangle, Color.White);
        }

        private void DrawUIToTarget(DrawInfo info)
        {
            if (mLeftUI == null && mRightUI == null)
            {
                mLeftUI = new RenderTarget2D(info.device, UI_PANEL_SIZE, mGameArea.Height);
                mRightUI = new RenderTarget2D(info.device, UI_PANEL_SIZE, mGameArea.Height);
            }

            DrawLeftUI(info);
            DrawRightUI(info);
        }

        private void DrawLeftUI(DrawInfo info)
        {
            info.device.SetRenderTarget(mLeftUI);
            info.device.Clear(Color.Transparent);

            info.spriteBatch.Begin(SpriteSortMode.Immediate,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp,
                        DepthStencilState.None,
                        RasterizerState.CullNone);

            int lives = ProgressManager.I.Lives;

            int texScale = 4;
            int texWidth = mLifeTexture.Width * texScale;
            int texHeight = mLifeTexture.Height * texScale;

            Rectangle lifeRect = new Rectangle((mLeftUI.Width - texWidth) / 2, 32, texWidth, texHeight);

            for(int i = 0; i < lives; i++)
            {
                info.spriteBatch.Draw(mLifeTexture, lifeRect, Color.White);
                lifeRect.Y += texHeight + 10;
            }

            Util.DrawStringCentred(info.spriteBatch, mPixelFont, new Vector2(mLeftUI.Width / 2, 485.0f), Color.Yellow, ProgressManager.I.GetWorldData().name);
            Util.DrawStringCentred(info.spriteBatch, mPixelFont, new Vector2(mLeftUI.Width / 2, 505.0f), Color.White, "Level " + (ProgressManager.I.CurrentLevel + 1));

            info.spriteBatch.End();
        }

        private void DrawRightUI(DrawInfo info)
        {
            info.device.SetRenderTarget(mRightUI);
            info.device.Clear(Color.Transparent);

            info.spriteBatch.Begin(SpriteSortMode.Immediate,
                        BlendState.AlphaBlend,
                        SamplerState.PointClamp,
                        DepthStencilState.None,
                        RasterizerState.CullNone);

            Util.DrawStringCentred(info.spriteBatch, mPixelFont, new Vector2(mRightUI.Width / 2, 223.0f), Color.White, "Time");
            Util.DrawStringCentred(info.spriteBatch, mPixelFont, new Vector2(mRightUI.Width / 2, 238.0f), Color.White, GhostManager.I.GetTime());

            string timeToBeat = GhostManager.I.GetTimeToBeat();

            if (timeToBeat != "")
            {
                Util.DrawStringCentred(info.spriteBatch, mPixelFont, new Vector2(mRightUI.Width / 2, 283.0f), Color.DarkOliveGreen, "Time to beat");
                Util.DrawStringCentred(info.spriteBatch, mPixelFont, new Vector2(mRightUI.Width / 2, 298.0f), Color.DarkOliveGreen, timeToBeat);
            }

            info.spriteBatch.End();
        }

        //============================================
        //  Update
        //--------------------------------------------
        public override void Update(GameTime gameTime)
        {
            FXManager.I.Update(gameTime);
            if (mLevelEndTimer.IsPlaying())
            {
                if(mLevelEndTimer.GetElapsedMs() > END_LEVEL_TIME)
                {
                    MoveToNextLevel();
                }

                return;
            }

            KeyboardState keyboardState = Keyboard.GetState();

            if(keyboardState.IsKeyDown(Keys.R))
            {
                EArgs eArgs;
                eArgs.sender = this;

                EventManager.I.SendEvent(EventType.KillPlayer, eArgs);
            }

            GhostManager.I.Update(gameTime);
            EntityManager.I.Update(gameTime);

            LevelStatus status = GetCurrentLevel().Update(gameTime);

            if(status == LevelStatus.Win)
            {
                LevelWin();
            }
            else if(status == LevelStatus.Loss)
            {
                LevelLose();
            }
        }

        private void LevelWin()
        {
            mLevelEndTimer.Start();
            DisplayLevelEndText();
            GhostManager.I.EndLevel(true);
        }

        private void MoveToNextLevel()
        {
            ProgressManager.I.ReportLevelWin();
            FXManager.I.Clear();
            if(ProgressManager.I.CurrentLevel >= mLevels.Count)
            {
                ScreenManager.I.ActivateScreen(ScreenType.EndGame);
            }
            else
            {
                ScreenManager.I.ActivateScreen(ScreenType.LevelStart);
            }
        }

        private void LevelLose()
        {
            ProgressManager.I.ReportLevelLoss();

            if (ProgressManager.I.Lives == 0)
            {
                ScreenManager.I.ActivateScreen(ScreenType.GameOver);
                ProgressManager.I.ResetGame();
            }
            else
            {
                //Start the level again
                StartLevel();
            }
        }

        private void DisplayLevelEndText()
        {
            for (int i = 0; i < EntityManager.I.GetEntityNum(); i++)
            {
                Entity e = EntityManager.I.GetEntity(i);
                if(e is Arnold)
                {
                    DisplayLevelEndTextOnArnold((Arnold)e);
                }
            }
        }

        private void DisplayLevelEndTextOnArnold(Arnold arnold)
        {
            int? timeDiff = GhostManager.I.GetTimeDifference();

            if(timeDiff.HasValue)
            {
                int frameDiff = timeDiff.Value;

                Color textCol = Color.White;
                string prefix = "+";
                
                if(frameDiff > 0)
                {
                    prefix = "+";
                    textCol = Color.Crimson;
                }
                else if(frameDiff < 0)
                {
                    prefix = "-";
                    textCol = Color.DeepSkyBlue;
                    frameDiff = -frameDiff;
                }

                FXManager.I.AddTextScroller(FontManager.I.GetFont("Pixica Micro-24"), textCol, arnold.position, prefix + GhostManager.I.FrameTimeToString(frameDiff));
            }
            else
            {
                string levelCompleteMsg = "Level complete";

                if(GetCurrentLevel() is CollectFlagLevel)
                {
                    levelCompleteMsg = "Checkpoint!";
                }
                FXManager.I.AddTextScroller(FontManager.I.GetFont("Pixica Micro-24"), Color.Wheat, arnold.position, levelCompleteMsg);
            }
        }
    }
}
