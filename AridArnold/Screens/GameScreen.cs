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


        private Color mBGCol;
        private List<Level> mLevels;

        private RenderTarget2D mGameArea;

        private MonoTimer mLevelEndTimer;

        //============================================
        //  Initialisation
        //--------------------------------------------
        public GameScreen(ContentManager content, GraphicsDeviceManager graphics) : base(content, graphics)
        {
            mBGCol = Color.Black;

            mLevelEndTimer = new MonoTimer();

            mLevels = new List<Level>();
            mLevels.Add(new CollectWaterLevel("level1-1", 5));
            mLevels.Add(new CollectWaterLevel("level1-2", 2));
            mLevels.Add(new CollectWaterLevel("level1-3", 2));
            mLevels.Add(new CollectFlagLevel("level1-4"));
            mLevels.Add(new CollectWaterLevel("level2-1", 2));
        }

        private void LoadLevel(int levelIndex)
        {
            mLevels[levelIndex].Begin(mContentManager);
        }

        public override void OnActivate()
        {
            TileManager.I.Init(new Vector2(0.0f, TILE_SIZE), TILE_SIZE);
            TileManager.I.CentreX(TileManager.I.GetDrawWidth() + 2 * TILE_SIZE);

            if (mGameArea != null)
            {
                mGameArea.Dispose();
            }
            mGameArea = null;

            StartLevel();
        }

        public override void OnDeactivate()
        {

        }

        public override void LoadContent(ContentManager content)
        {

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
            mBGCol = GetBGColor();
            LoadLevel(ProgressManager.I.CurrentLevel);
            mLevelEndTimer.FullReset();
        }

        private Color GetBGColor()
        {
            return new Color(0, 20, 10);
        }

        //============================================
        //  Draw
        //--------------------------------------------
        public override void Draw(DrawInfo info)
        {
            Rectangle screenRect = info.device.PresentationParameters.Bounds;

            //Get game rendered as a texture
            RenderGameAreaToTarget(info);

            //Draw out the game area
            info.device.SetRenderTarget(null);
            info.device.Clear(new Color(0, 0, 0));

            info.spriteBatch.Begin(SpriteSortMode.Immediate,
                                    BlendState.AlphaBlend,
                                    SamplerState.PointClamp,
                                    DepthStencilState.None,
                                    RasterizerState.CullNone);

            DrawGameArea(info, mGameArea, screenRect);

            info.spriteBatch.End();
        }

        private void DrawGameArea(DrawInfo info, RenderTarget2D gameArea, Rectangle screenRect)
        {
            int multiplier = (int)MathF.Min(screenRect.Width/ gameArea.Width, screenRect.Height / gameArea.Height);

            int finalWidth = gameArea.Width * multiplier;
            int finalHeight = gameArea.Height * multiplier;

            if(multiplier == 0)
            {
                float screenAspectRatio = (float)screenRect.Width / (float)screenRect.Height;
                float gameAspectRatio = (float)gameArea.Width / (float)gameArea.Height;

                if (screenAspectRatio > gameAspectRatio)
                {
                    finalHeight = screenRect.Height;
                    finalWidth = (int)(finalHeight * gameAspectRatio);
                    
                }
                else
                {
                    finalWidth = screenRect.Width;
                    finalHeight = (int)(finalWidth / gameAspectRatio);
                }
            }

            info.spriteBatch.Draw(gameArea, new Rectangle((screenRect.Width - finalWidth)/2, 0, finalWidth, finalHeight), Color.White);
        }


        private void RenderGameAreaToTarget(DrawInfo info)
        {
            if(mGameArea == null)
            {
                mGameArea = new RenderTarget2D(info.device, TileManager.I.GetDrawWidth() + 2 * TILE_SIZE, TileManager.I.GetDrawHeight() + TILE_SIZE);
            }

            info.device.SetRenderTarget(mGameArea);

            Color clearCol = mBGCol;

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



            EntityManager.I.Draw(info);
            TileManager.I.Draw(info);

            DrawUI(info);

            info.spriteBatch.End();
        }

        private void DrawUI(DrawInfo info)
        {

        }

        //============================================
        //  Update
        //--------------------------------------------
        public override void Update(GameTime gameTime)
        {
            if(mLevelEndTimer.IsPlaying())
            {
                if(mLevelEndTimer.GetElapsedMs() > END_LEVEL_TIME)
                {
                    MoveToNextLevel();
                }

                return;
            }

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
        }

        private void MoveToNextLevel()
        {
            ProgressManager.I.ReportLevelWin();

            ScreenManager.I.ActivateScreen(ScreenType.LevelStart);
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
    }
}
