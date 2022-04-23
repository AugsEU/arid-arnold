using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AridArnold
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private Arnold mArnold;

        private List<Entity> mRegisteredEntities;

        public Main()
        {
            //XNA
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            //XNA

            mArnold = new Arnold();

            mRegisteredEntities = new List<Entity>();
            RegisterDefaultEntities();
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

            TileManager.I.LoadContent(Content);

            foreach (Entity entity in mRegisteredEntities)
            {
                entity.LoadContent(Content);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            foreach (Entity entity in mRegisteredEntities)
            {
                entity.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            DrawInfo frameInfo;

            frameInfo.graphics = _graphics;
            frameInfo.spriteBatch = _spriteBatch;
            frameInfo.gameTime = gameTime;

            foreach (Entity entity in mRegisteredEntities)
            {
                entity.Draw(frameInfo);
            }

            TileManager.I.DrawCentredX(frameInfo);

            base.Draw(gameTime);

            _spriteBatch.End();
        }

        private void RegisterEntity(Entity entity)
        {
            mRegisteredEntities.Add(entity);
        }

        private void RegisterDefaultEntities()
        {
            RegisterEntity(mArnold);
        }
    }
}
