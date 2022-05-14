using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold.Screens
{
    internal class GameOverScreen : Screen
    {
        public GameOverScreen(ContentManager content, GraphicsDeviceManager graphics) : base(content, graphics)
        {

        }

        public override void OnActivate()
        {

        }

        public override void OnDeactivate()
        {

        }

        public override void LoadContent(ContentManager content)
        {

        }

        public override void Draw(DrawInfo info)
        {
            SpriteFont pixelFont = FontManager.I.GetFont("Pixica-24");

            Rectangle screenRect = info.device.PresentationParameters.Bounds;
            Vector2 centre = new Vector2(screenRect.Width/2, screenRect.Height / 2);

            //Draw out the game area
            info.device.Clear(new Color(0, 0, 0));

            info.spriteBatch.Begin(SpriteSortMode.Immediate,
                                    BlendState.AlphaBlend,
                                    SamplerState.PointClamp,
                                    DepthStencilState.None,
                                    RasterizerState.CullNone);

            Util.DrawStringCentred(info.spriteBatch, pixelFont, centre, Color.White, "GAME OVER");

            info.spriteBatch.End();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();

            if(state.IsKeyDown(Keys.Enter) || state.IsKeyDown(Keys.Space))
            {
                ScreenManager.I.ActivateScreen(ScreenType.Game);
            }
        }

    }
}
