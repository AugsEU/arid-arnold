using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AridArnold.Screens;

namespace AridArnold
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public Main()
        {
            //XNA
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //XNA
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = 524;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = 720;   // set this value to the desired height of your window
            //_graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            TileManager.I.Init(new Vector2(0.0f, 150.0f), 16);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            ScreenManager.I.LoadScreen(new GameScreen(Content, _graphics));
            ScreenManager.I.ActivateScreen(ScreenType.Game);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Update Active screen
            Screen screen = ScreenManager.I.GetActiveScreen();

            if (screen != null)
            {
                screen.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            RenderTarget2D rt = new RenderTarget2D(GraphicsDevice, 200, 200);
            Rectangle rect = new Rectangle(0, 0, 200, 200);
            GraphicsDevice.SetRenderTarget(rt);
            GraphicsDevice.Clear(new Color(0,20,10));

            _spriteBatch.Begin(SpriteSortMode.Immediate,
                                BlendState.AlphaBlend,
                                SamplerState.PointClamp,
                                DepthStencilState.None,
                                RasterizerState.CullNone);

            DrawInfo frameInfo;

            frameInfo.graphics = _graphics;
            frameInfo.spriteBatch = _spriteBatch;
            frameInfo.gameTime = gameTime;
            
            base.Draw(gameTime);

            //Draw active screen.
            Screen screen = ScreenManager.I.GetActiveScreen();

            if(screen != null)
            {
                screen.Draw(frameInfo);
            }

            _spriteBatch.End();
        }
    }
}
