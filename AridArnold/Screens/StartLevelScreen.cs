using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold.Screens
{
    internal class StartLevelScreen : Screen
    {
        MonoTimer mTimer;

        public StartLevelScreen(ContentManager content, GraphicsDeviceManager graphics) : base(content, graphics)
        {
            mTimer = new MonoTimer();
        }

        public override void LoadContent(ContentManager content)
        {

        }

        public override void OnActivate()
        {
            mTimer.FullReset();
            mTimer.Start();
        }

        public override void OnDeactivate()
        {

        }

        public override void Draw(DrawInfo info)
        {
            SpriteFont pixelFont = FontManager.I.GetFont("Pixica-24");

            Rectangle screenRect = info.device.PresentationParameters.Bounds;
            Vector2 centre = new Vector2(screenRect.Width / 2, screenRect.Height / 2);

            //Draw out the game area
            info.device.Clear(new Color(0, 0, 0));

            info.spriteBatch.Begin(SpriteSortMode.Immediate,
                                    BlendState.AlphaBlend,
                                    SamplerState.PointClamp,
                                    DepthStencilState.None,
                                    RasterizerState.CullNone);
            Util.DrawStringCentred(info.spriteBatch, pixelFont, centre, Color.Gold, (ProgressManager.I.GetWorldData().name));

            centre.Y += 25.0f;

            Util.DrawStringCentred(info.spriteBatch, pixelFont, centre, Color.White, "Level " + (ProgressManager.I.CurrentLevel + 1));

            info.spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            if (mTimer.GetElapsedMs() > 2000 || state.IsKeyDown(Keys.Enter) || state.IsKeyDown(Keys.Space))
            {
                ScreenManager.I.ActivateScreen(ScreenType.Game);
            }
        }
    }
}
