using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using AridArnold.Screens;

namespace AridArnold
{
    public class Main : Game
    {
        private const double FRAME_RATE = 60d;
        private const int MIN_HEIGHT = 550;
        private const float ASPECT_RATIO = 1.77778f;

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

            // Fix to 60 fps.
            IsFixedTimeStep = true;//false;
            TargetElapsedTime = System.TimeSpan.FromSeconds(1d / FRAME_RATE);

            Window.ClientSizeChanged += OnResize;
        }

        protected override void Initialize()
        {
            SetWindowHeight(MIN_HEIGHT);
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            Window.AllowUserResizing = true;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            FontManager.I.LoadAllFonts(Content);
            ScreenManager.I.LoadAllScreens(Content, _graphics);
            ScreenManager.I.ActivateScreen(ScreenType.LevelStart);
            GhostManager.I.Load(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            gameTime.ElapsedGameTime = TargetElapsedTime;

            //Record elapsed time
            TimeManager.I.Update(gameTime);

            KeyboardState keyboardState = Keyboard.GetState();
            foreach (Keys key in keyboardState.GetPressedKeys())
            {
                HandleKeyPress(key);
            }

            const int updateSteps = 4;

            System.TimeSpan timeInc = gameTime.ElapsedGameTime / updateSteps;
            for(int i = 0; i < updateSteps; i++)
            {
                GameTime stepTime = new GameTime(gameTime.TotalGameTime - (updateSteps - 1 - i) * timeInc, timeInc);

                GameUpdate(stepTime);

            }

            base.Update(gameTime);
        }

        private void OnResize(object sender, EventArgs eventArgs)
        {
            if (_graphics.IsFullScreen)
            {
                return;
            }

            int min_width = (int)(ASPECT_RATIO * MIN_HEIGHT);

            if(Window.ClientBounds.Height >= MIN_HEIGHT && Window.ClientBounds.Width >= min_width)
            {
                return;
            }
            else
            {
                SetWindowHeight(MIN_HEIGHT);
            }
        }

        private void SetWindowHeight(int height)
        {
            _graphics.PreferredBackBufferWidth = (int)(height * ASPECT_RATIO);
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();
        }

        private void GameUpdate(GameTime gameTime)
        {
            //Update Active screen
            Screen screen = ScreenManager.I.GetActiveScreen();

            if (screen != null)
            {
                screen.Update(gameTime);
            }
        }

        private void HandleKeyPress(Keys key)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            bool alt = keyboardState.IsKeyDown(Keys.LeftAlt) || keyboardState.IsKeyDown(Keys.RightAlt);
            if (key == Keys.Enter && alt)
            {
                ToggleFullscreen();
            }

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
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
                RenderTarget2D screenTargetRef = screen.DrawToRenderTarget(frameInfo);

                GraphicsDevice.SetRenderTarget(null);
                _spriteBatch.Begin(SpriteSortMode.Immediate,
                                    BlendState.AlphaBlend,
                                    SamplerState.PointClamp,
                                    DepthStencilState.None,
                                    RasterizerState.CullNone);
                DrawScreenPixelPerfect(frameInfo, screenTargetRef);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        private void DrawScreenPixelPerfect(DrawInfo info, RenderTarget2D screen)
        {
            Rectangle screenRect = info.device.PresentationParameters.Bounds;

            int multiplier = (int)MathF.Min(screenRect.Width / screen.Width, screenRect.Height / screen.Height);

            int finalWidth = screen.Width * multiplier;
            int finalHeight = screen.Height * multiplier;

            Rectangle destRect = new Rectangle((screenRect.Width - finalWidth) / 2, (screenRect.Height - finalHeight) / 2, finalWidth, finalHeight);

            info.spriteBatch.Draw(screen, destRect, Color.White);
        }
    }
}
