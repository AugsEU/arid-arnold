using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold
{
    class TileCollisionResultsSorter : IComparer<Tuple<Point, CollisionResults>>
    {
        public int Compare(Tuple<Point, CollisionResults> a, Tuple<Point, CollisionResults> b)
        {
            return a.Item2.t.Value.CompareTo(b.Item2.t.Value);
        }
    }

    internal class TileManager : Singleton<TileManager>
    {
        Vector2 mTileMapPos;
        float mTileSize;
        Texture2D mDefaultTileTexture;

        Tile[,] mTileMap = new Tile[32, 32];

        Dictionary<TileType, Texture2D> mTileTextures = new Dictionary<TileType, Texture2D>();

        public void Init(Vector2 position, float tileSize)
        {
            mTileMapPos = position;
            mTileSize = tileSize;
        }

        public void LoadContent(ContentManager content)
        {
            LoadTileType(content, TileType.Air, "air");
            LoadTileType(content, TileType.Square, "blank");
            LoadTileType(content, TileType.Wall, "wall");
            LoadTileType(content, TileType.Platform, "platform");

            mDefaultTileTexture = content.Load<Texture2D>("Tiles/blank");
        }

        private void LoadTileType(ContentManager content, TileType type, string textureName)
        {
            mTileTextures.Add(type, content.Load<Texture2D>("Tiles/" + textureName));
        }

        private Texture2D GetTileTexture(Tile tile)
        {
            Texture2D tex;
            if(mTileTextures.TryGetValue(tile.GetTileType(), out tex))
            {
                return tex;
            }

            return mDefaultTileTexture;
        }


        public void LoadLevel(ContentManager content, string name)
        {
            Texture2D tileTexture = content.Load<Texture2D>(name);

            Color[] colors1D = new Color[tileTexture.Width * tileTexture.Height];
            tileTexture.GetData<Color>(colors1D);

            for (int x = 0; x < tileTexture.Width; x++)
            {
                for (int y = 0; y < tileTexture.Height; y++)
                {
                    int index = x + y * tileTexture.Width;

                    if (colors1D[index] == Color.Black)
                    {
                        mTileMap[x, y] = new WallTile();
                    }
                    else if (colors1D[index] == Color.Red)
                    {
                        mTileMap[x, y] = new PlatformTile();
                    }
                    else
                    {
                        mTileMap[x, y] = new AirTile();
                    }
                }
            }
        }

        private void Draw(DrawInfo info)
        {
            Point offset = new Point((int)mTileMapPos.X, (int)mTileMapPos.Y);

            for (int x= 0; x<mTileMap.GetLength(0); x++)
            {
                for(int y= 0; y<mTileMap.GetLength(1); y++)
                {
                    Texture2D texture = GetTileTexture(mTileMap[x,y]);

                    Rectangle drawRectangle = new Rectangle(offset.X + x * (int)mTileSize, offset.Y + y * (int)mTileSize, (int)mTileSize, (int)mTileSize);

                    info.spriteBatch.Draw(texture, drawRectangle, Color.White);
                }
            }
        }

        public void DrawCentredX(DrawInfo info)
        {
            float ourWidth = mTileSize * mTileMap.GetLength(0);
            float xOffset = (info.graphics.PreferredBackBufferWidth - ourWidth)/2.0f;

            mTileMapPos.X = xOffset;

            Draw(info);
        }

        //Collisions
        public void ResolveCollisions(MovingEntity entity, GameTime gameTime)
        {
            List<Tuple<Point, CollisionResults>> results = new List<Tuple<Point, CollisionResults>>();

            Util.Log("==Resolving==");

            //TO DO: OPTIMISE THIS FOR THE LOVE OF GOD
            for (int x = 0; x < mTileMap.GetLength(0); x++)
            {
                for (int y = 0; y < mTileMap.GetLength(1); y++)
                {
                    Vector2 tileTopLeft = mTileMapPos + new Vector2(x, y) * mTileSize;

                    CollisionResults collisionResults = mTileMap[x, y].Collide(entity, tileTopLeft, mTileSize, gameTime);

                    if(collisionResults.t.HasValue)
                    {
                        results.Add(new Tuple<Point, CollisionResults>(new Point(x,y), collisionResults));
                    }
                }
            }
            results.Sort(new TileCollisionResultsSorter());

            for (int i = 0; i < results.Count; i++)
            {
                Point point = results[i].Item1;
                Vector2 tileTopLeft = mTileMapPos + new Vector2(point.X, point.Y) * mTileSize;

                Tile tile = mTileMap[point.X, point.Y];

                CollisionResults collisionResults = tile.Collide(entity, tileTopLeft, mTileSize, gameTime);

                if (collisionResults.t.HasValue)
                {
                    Util.Log("   Pushing by normal " + collisionResults.normal.X + ", " + collisionResults.normal.Y + "(" + collisionResults.t.Value + ")");

                    entity.velocity += collisionResults.normal * new Vector2(Math.Abs(entity.velocity.X), Math.Abs(entity.velocity.Y)) * (1.0f - collisionResults.t.Value) * 1.01f;

                    entity.ReactToCollision(Collision2D.GetCollisionType(collisionResults.normal));
                }
            }
        }
    }
}
