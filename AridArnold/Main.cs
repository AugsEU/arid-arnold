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
        private Rectangle windowedRect;

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
            _graphics.PreferredBackBufferWidth = 550;  // set this value to the desired width of your window
            _graphics.PreferredBackBufferHeight = 550;   // set this value to the desired height of your window
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            Window.AllowUserResizing = true;

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
            KeyboardState keyboardState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            foreach(Keys key in keyboardState.GetPressedKeys())
            {
                HandleKeyPress(key);
            }

            //Update Active screen
            Screen screen = ScreenManager.I.GetActiveScreen();

            if (screen != null)
            {
                screen.Update(gameTime);
            }

            base.Update(gameTime);
        }

        private void HandleKeyPress(Keys key)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            bool alt = keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt);
            if (key == Keys.Enter && alt)
            {
                ToggleFullscreen();
            }
        }

        private void ToggleFullscreen()
        {
            if (_graphics.IsFullScreen)
            {
                _graphics.IsFullScreen = false;
                _graphics.PreferredBackBufferWidth = windowedRect.Width;
                _graphics.PreferredBackBufferHeight = windowedRect.Height;
            }
            else
            {
                windowedRect = GraphicsDevice.PresentationParameters.Bounds;
                _graphics.IsFullScreen = true;
                
                _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }

            _graphics.ApplyChanges();
        }

        protected override void Draw(GameTime gameTime)
        {
            DrawInfo frameInfo;

            frameInfo.graphics = _graphics;
            frameInfo.spriteBatch = _spriteBatch;
            frameInfo.gameTime = gameTime;
            frameInfo.device = GraphicsDevice;

            //Draw active screen.
            Screen screen = ScreenManager.I.GetActiveScreen();

            if(screen != null)
            {
                screen.Draw(frameInfo);
            }

            base.Draw(gameTime);
        }
    }
}
