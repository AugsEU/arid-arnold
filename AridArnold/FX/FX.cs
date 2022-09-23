namespace AridArnold
{
    abstract class FX
    {
        public abstract void Update(GameTime gameTime);

        public abstract void Draw(DrawInfo info);

        public abstract bool Finished();
    }
}
