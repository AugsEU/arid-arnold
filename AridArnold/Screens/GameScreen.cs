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
        private List<Level> mLevels;
        private int mCurrentLevel;
        private RenderTarget2D mGameArea;

        //============================================
        //  Initialisation
        //--------------------------------------------
        public GameScreen(ContentManager content, GraphicsDeviceManager graphics) : base(content, graphics)
        {
            mLevels = new List<Level>();
            mLevels.Add(new CollectWaterLevel("level1-1", 5));
            mLevels.Add(new CollectWaterLevel("level1-2", 2));
            mLevels.Add(new CollectWaterLevel("level1-3", 2));
            mLevels.Add(new CollectWaterLevel("level1-4", 3));
        }

        private void LoadLevel(int levelIndex)
        {
            mContentManager.Unload();
            TileManager.I.CentreX(mGraphics);

            mCurrentLevel = levelIndex;
            mLevels[levelIndex].Begin(mContentManager);
        }

        public override void OnActivate()
        {
            TileManager.I.Init(new Vector2(0.0f, TILE_SIZE), TILE_SIZE);

            LoadLevel(0);
            mGameArea = null;
        }

        public override void OnDeactivate()
        {

        }

        public override void LoadContent(ContentManager content)
        {

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
                mGameArea = new RenderTarget2D(info.device, TileManager.I.GetDrawWidth(), TileManager.I.GetDrawHeight() + TILE_SIZE);
            }

            info.device.SetRenderTarget(mGameArea);

            info.device.Clear(new Color(0, 20, 10));

            info.spriteBatch.Begin(SpriteSortMode.Immediate,
                                    BlendState.AlphaBlend,
                                    SamplerState.PointClamp,
                                    DepthStencilState.None,
                                    RasterizerState.CullNone);



            EntityManager.I.Draw(info);
            TileManager.I.Draw(info);

            info.spriteBatch.End();
        }

        //============================================
        //  Update
        //--------------------------------------------
        public override void Update(GameTime gameTime)
        {
            EntityManager.I.Update(gameTime);

            LevelStatus status = mLevels[mCurrentLevel].Update(gameTime);

            if(status == LevelStatus.Win)
            {
                LoadLevel(mCurrentLevel + 1);
            }
            else if(status == LevelStatus.Loss)
            {
                LoadLevel(mCurrentLevel);
            }
        }
    }
}
