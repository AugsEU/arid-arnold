using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold
{
    class TileCollisionResultsSorter : IComparer<TileCollisionResults>
    {
        public int Compare(TileCollisionResults a, TileCollisionResults b)
        {
            return a.result.t.Value.CompareTo(b.result.t.Value);
        }
    }

    class TileCollisionResults
    {
        public TileCollisionResults(Point p, CollisionResults r)
        {
            coord = p;
            result = r;
        }

        public Point coord;
        public CollisionResults result;
    }

    internal class TileManager : Singleton<TileManager>
    {
        //============================================
        //  Members
        //--------------------------------------------
        Vector2 mTileMapPos;
        float mTileSize;

        Tile[,] mTileMap = new Tile[32, 32];

        //============================================
        //  Initialisation
        //--------------------------------------------
        public void Init(Vector2 position, float tileSize)
        {
            mTileMapPos = position;
            mTileSize = tileSize;
        }

        private Tile GetTileFromColour(Color col)
        {
            //Use alpha component as a parameter.
            int param = 255 - col.A;

            if (col.A > 0)
            {
                if (Util.CompareHEX(col, 0x000000))
                {
                    return new WallTile();
                }
                else if (Util.CompareHEX(col, 0xA9A9A9))
                {
                    return new PlatformTile((CardinalDirection)param);
                }
                else if (Util.CompareHEX(col, 0x0000FF))
                {
                    return new WaterTile();
                }
                else if (Util.CompareHEX(col, 0xFF0000))
                {
                    return new FlagTile();
                }
                else if (Util.CompareHEX(col, 0xEA301F))
                {
                    return new HotDogTile();
                }
                else if (Util.CompareHEX(col, 0x404040))
                {
                    return new SpikesTile((CardinalDirection)param);
                }
                else if (Util.CompareHEX(col, 0x2A3F50))
                {
                    return new StalactiteTile();
                }
                else if (Util.CompareHEX(col, 0xFFFF00))
                {
                    return new MirrorTile((CardinalDirection)param);
                }
                else if(Util.CompareHEX(col, 0x00CDF9))
                {
                    return new MushroomTile();
                }
            }

            return new AirTile();
        }

        private void AddEntityFromColour(Color col, Vector2 pos, ContentManager content)
        {
            if (Util.CompareHEX(col, 0xDC143C))
            {
                EntityManager.I.RegisterEntity(new Arnold(pos), content);
            }
            else if (Util.CompareHEX(col, 0x5B2C2C))
            {
                EntityManager.I.RegisterEntity(new Trundle(pos), content);
            }
        }

        public void LoadLevel(ContentManager content, string name)
        {
            EntityManager.I.ClearEntities();
            CollectibleManager.I.ClearAllCollectibles();

            Texture2D tileTexture = content.Load<Texture2D>(name);

            mTileMap = new Tile[tileTexture.Width, tileTexture.Height];

            Color[] colors1D = new Color[tileTexture.Width * tileTexture.Height];
            tileTexture.GetData<Color>(colors1D);

            for (int x = 0; x < tileTexture.Width; x++)
            {
                for (int y = 0; y < tileTexture.Height; y++)
                {
                    int index = x + y * tileTexture.Width;
                    Color col = colors1D[index];

                    mTileMap[x, y] = GetTileFromColour(colors1D[index]);
                    mTileMap[x, y].LoadContent(content);

                    Vector2 entityPos = new Vector2(x * mTileSize, y * mTileSize) + mTileMapPos;
                    AddEntityFromColour(col, entityPos, content);
                }
            }

            CalculateTileAdjacency();
        }

        private void CalculateTileAdjacency()
        {
            for (int x = 0; x < mTileMap.GetLength(0); x++)
            {
                for (int y = 0; y < mTileMap.GetLength(1); y++)
                {
                    if(x + 1 < mTileMap.GetLength(0))
                    {
                        Type type1 = mTileMap[x, y].GetType();
                        Type type2 = mTileMap[x+1, y].GetType();

                        if (mTileMap[x, y].GetType() == mTileMap[x+1,y].GetType())
                        {
                            mTileMap[x, y].SetRightAdjacent(mTileMap[x + 1, y]);
                        }
                    }

                    if (y + 1 < mTileMap.GetLength(1))
                    {
                        if (mTileMap[x, y].GetType() == mTileMap[x, y + 1].GetType())
                        {
                            mTileMap[x, y].SetBottomAdjacent(mTileMap[x, y + 1]);
                        }
                    }
                }
            }
        }

        //============================================
        //  Utility
        //--------------------------------------------
        public Tile GetTile(Point coord)
        {
            return mTileMap[coord.X, coord.Y];
        }


        //============================================
        //  Updates
        //--------------------------------------------
        public void Update(GameTime gameTime)
        {
            Vector2 offset = new Vector2(mTileSize, mTileSize);
            for (int x = 0; x < mTileMap.GetLength(0); x++)
            {
                for (int y = 0; y < mTileMap.GetLength(1); y++)
                {
                    Vector2 newMin = new Vector2(mTileSize * x, mTileSize * y);
                    Rect2f tileRect = new Rect2f(newMin, newMin + offset);

                    mTileMap[x, y].Update(gameTime, tileRect);
                }
            }
        }

        public void EntityTouchTiles(Entity entity)
        {
            Rectangle tileBounds = PossibleIntersectTiles(entity.ColliderBounds());

            for (int x = tileBounds.X; x <= tileBounds.X + tileBounds.Width; x++)
            {
                for (int y = tileBounds.Y; y <= tileBounds.Y + tileBounds.Height; y++)
                {
                    Vector2 tileTopLeft = mTileMapPos + new Vector2(x, y) * mTileSize;

                    if (mTileMap[x, y].Enabled && Collision2D.BoxVsBox(mTileMap[x,y].GetBounds(tileTopLeft, mTileSize), entity.ColliderBounds()))
                    {
                        mTileMap[x, y].OnEntityIntersect(entity);
                    }
                }
            }
        }

        public bool DoesRectTouchTiles(Rect2f rect)
        {
            Rectangle tileBounds = PossibleIntersectTiles(rect);

            for (int x = tileBounds.X; x <= tileBounds.X + tileBounds.Width; x++)
            {
                for (int y = tileBounds.Y; y <= tileBounds.Y + tileBounds.Height; y++)
                {
                    Vector2 tileTopLeft = mTileMapPos + new Vector2(x, y) * mTileSize;

                    if (mTileMap[x, y].Enabled && Collision2D.BoxVsBox(mTileMap[x, y].GetBounds(tileTopLeft, mTileSize), rect))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
       

        //============================================
        //  Draw
        //--------------------------------------------
        public void Draw(DrawInfo info)
        {
            Point offset = new Point((int)mTileMapPos.X, (int)mTileMapPos.Y);

            for (int x= 0; x<mTileMap.GetLength(0); x++)
            {
                for(int y= 0; y<mTileMap.GetLength(1); y++)
                {
                    if (mTileMap[x, y].Enabled)
                    {
                        Rectangle drawRectangle = new Rectangle(offset.X + x * (int)mTileSize, offset.Y + y * (int)mTileSize, (int)mTileSize, (int)mTileSize);

                        DrawTile(info, drawRectangle, mTileMap[x, y]);
                    }
                }
            }
        }

        public int GetDrawWidth()
        {
            return (int)(mTileSize * mTileMap.GetLength(0));
        }

        public int GetDrawHeight()
        {
            return (int)(mTileSize * mTileMap.GetLength(1));
        }

        public void DrawTile(DrawInfo info, Rectangle drawDestination, Tile tile)
        {
            Texture2D tileTexture = tile.GetTexture();

            int tileHeight = tileTexture.Height;

            //Tile index that we will pick from the texture.
            Point tileIndex = new Point(0, 0);

            //Rotation amount so we can fit tiles together. Should be multiples of 90.
            float rotation = 0.0f;

            SpriteEffects effect = SpriteEffects.None;

            //Square texture, draw as is.
            if (tileTexture.Width == tileTexture.Height)
            {
                //Square textures can be rotated freely.
                //Others can't since they need ot be rotated to fit together.
                rotation = tile.GetRotation();
                effect = tile.GetEffect();
            }
            //Otherwise, look for texture with different edge types
            else if (tileTexture.Width == 6 * tileTexture.Height) //Needs rotating
            {
                SetupTileWithRotation(tile.GetAdjacency(), ref rotation, ref tileIndex);
            }
            else if(tileTexture.Width == 4 * tileTexture.Height)
            {
                tileHeight = tileHeight / 2;
                SetupTileNoRotation(tile.GetAdjacency(), ref tileIndex);
            }
            //What is this?
            else
            {
                throw new Exception("Unhandled texture dimensions");
            }


            if (tileHeight % drawDestination.Height != 0)
            {
                throw new Exception("Tile size doesn't match tile map, stretching may be happening. Must be an integer multiple.");
            }

            Rectangle sourceRectangle = new Rectangle(tileIndex.X * tileHeight, tileIndex.Y * tileHeight, tileHeight, tileHeight);

            info.spriteBatch.Draw(tileTexture, drawDestination, sourceRectangle, Color.White, rotation, Util.CalcRotationOffset(rotation, tileHeight), effect, 1.0f);
        }

        private void SetupTileNoRotation(AdjacencyType adjacency, ref Point tileIndex)
        {
            switch (adjacency)
            {
                case AdjacencyType.None:
                    tileIndex.X = 7;
                    tileIndex.Y = 1;
                    break;
                case AdjacencyType.Top:
                    tileIndex.X = 0;
                    tileIndex.Y = 0;
                    break;
                case AdjacencyType.Bottom:
                    tileIndex.X = 2;
                    tileIndex.Y = 0;
                    break;
                case AdjacencyType.Left:
                    tileIndex.X = 3;
                    tileIndex.Y = 0;
                    break;
                case AdjacencyType.Right:
                    tileIndex.X = 1;
                    tileIndex.Y = 0;
                    break;
                case AdjacencyType.TopBottom:
                    tileIndex.X = 5;
                    tileIndex.Y = 1;
                    break;
                case AdjacencyType.TopLeft:
                    tileIndex.X = 0;
                    tileIndex.Y = 1;
                    break;
                case AdjacencyType.TopRight:
                    tileIndex.X = 1;
                    tileIndex.Y = 1;
                    break;
                case AdjacencyType.TopBottomLeft:
                    tileIndex.X = 5;
                    tileIndex.Y = 0;
                    break;
                case AdjacencyType.TopBottomRight:
                    tileIndex.X = 7;
                    tileIndex.Y = 0;
                    break;
                case AdjacencyType.TopLeftRight:
                    tileIndex.X = 6;
                    tileIndex.Y = 0;
                    break;
                case AdjacencyType.BottomRight:
                    tileIndex.X = 2;
                    tileIndex.Y = 1;
                    break;
                case AdjacencyType.BottomLeft:
                    tileIndex.X = 3;
                    tileIndex.Y = 1;
                    break;
                case AdjacencyType.BottomLeftRight:
                    tileIndex.X = 4;
                    tileIndex.Y = 0;
                    break;
                case AdjacencyType.LeftRight:
                    tileIndex.X = 4;
                    tileIndex.Y = 1;
                    break;
                case AdjacencyType.All:
                    tileIndex.X = 6;
                    tileIndex.Y = 1;
                    break;
            }
        }

        private void SetupTileWithRotation(AdjacencyType adjacency, ref float rotation, ref Point tileIndex)
        {
            const float PI2 = MathHelper.PiOver2;
            const float PI = MathHelper.Pi;
            const float PI32 = MathHelper.Pi * 1.5f;

            switch (adjacency)
            {
                case AdjacencyType.None:
                    tileIndex.X = 0;
                    rotation = 0.0f;
                    break;
                case AdjacencyType.Top:
                    tileIndex.X = 1;
                    rotation = PI32;
                    break;
                case AdjacencyType.Bottom:
                    tileIndex.X = 1;
                    rotation = PI2;
                    break;
                case AdjacencyType.Left:
                    tileIndex.X = 1;
                    rotation = PI;
                    break;
                case AdjacencyType.Right:
                    tileIndex.X = 1;
                    rotation = 0.0f;
                    break;
                case AdjacencyType.TopBottom:
                    tileIndex.X = 2;
                    rotation = PI2;
                    break;
                case AdjacencyType.TopLeft:
                    tileIndex.X = 5;
                    rotation = 0.0f;
                    break;
                case AdjacencyType.TopRight:
                    tileIndex.X = 5;
                    rotation = PI2;
                    break;
                case AdjacencyType.TopBottomLeft:
                    tileIndex.X = 3;
                    rotation = PI32;
                    break;
                case AdjacencyType.TopBottomRight:
                    tileIndex.X = 3;
                    rotation = PI2;
                    break;
                case AdjacencyType.TopLeftRight:
                    tileIndex.X = 3;
                    rotation = 0.0f;
                    break;
                case AdjacencyType.BottomRight:
                    tileIndex.X = 5;
                    rotation = PI;
                    break;
                case AdjacencyType.BottomLeft:
                    tileIndex.X = 5;
                    rotation = PI32;
                    break;
                case AdjacencyType.BottomLeftRight:
                    tileIndex.X = 3;
                    rotation = PI;
                    break;
                case AdjacencyType.LeftRight:
                    tileIndex.X = 2;
                    rotation = 0.0f;
                    break;
                case AdjacencyType.All:
                    tileIndex.X = 4;
                    rotation = 0.0f;
                    break;
            }
        }

        public void CentreX(float screenWidth)
        {
            float ourWidth = GetDrawWidth();
            float xOffset = (screenWidth - ourWidth)/2.0f;

            mTileMapPos.X = xOffset;
        }

        //============================================
        //  Collisions
        //--------------------------------------------
        public List<TileCollisionResults> ResolveCollisions(MovingEntity entity, GameTime gameTime)
        {
            List<TileCollisionResults> results = new List<TileCollisionResults>();

            Util.Log("==Resolving==");

            Rect2f playerBounds = entity.ColliderBounds();
            Rect2f futurePlayerBounds = entity.ColliderBounds() + entity.VelocityToDisplacement(gameTime);

            Rectangle tileBounds = PossibleIntersectTiles(playerBounds + futurePlayerBounds);
           
            Util.Log(" Starting vel " + entity.velocity.ToString());

            for (int x = tileBounds.X; x <= tileBounds.X + tileBounds.Width; x++)
            {
                for (int y = tileBounds.Y; y <= tileBounds.Y + tileBounds.Height; y++)
                {
                    if (mTileMap[x, y].Enabled == false)
                    {
                        continue;
                    }

                    Vector2 tileTopLeft = mTileMapPos + new Vector2(x, y) * mTileSize;

                    CollisionResults collisionResults = mTileMap[x, y].Collide(entity, tileTopLeft, mTileSize, gameTime);

                    if (collisionResults.Collided)
                    {
                        results.Add(new TileCollisionResults(new Point(x, y), collisionResults));
                    }
                }
            }
            results.Sort(new TileCollisionResultsSorter());

            Util.Log(" Resolving all " + results.Count + " collisions");

            for (int i = 0; i < results.Count; i++)
            {
                Point point = results[i].coord;
                Vector2 tileTopLeft = mTileMapPos + new Vector2(point.X, point.Y) * mTileSize;

                Tile tile = mTileMap[point.X, point.Y];

                CollisionResults collisionResults = tile.Collide(entity, tileTopLeft, mTileSize, gameTime);

                if (collisionResults.Collided)
                {
                    Vector2 pushVec = collisionResults.normal * new Vector2(Math.Abs(entity.velocity.X), Math.Abs(entity.velocity.Y)) * (1.0f - collisionResults.t.Value) * 1.02f;

                    Util.Log("   " + point.ToString() + "Pushing by normal " + collisionResults.normal.ToString() + "(" + collisionResults.t.Value + ")");

                    entity.velocity += pushVec;
                }

                results[i].result = collisionResults;
            }

            Util.Log(" Final vel " + entity.velocity.X + ", " + entity.velocity.Y);

            return results;
        }

        public Rectangle PossibleIntersectTiles(Rect2f box)
        {
            box.min = (box.min - mTileMapPos) / mTileSize;
            box.max = (box.max - mTileMapPos) / mTileSize;

            Point rMin = new Point(Math.Max((int)box.min.X - 1, 0), Math.Max((int)box.min.Y - 1, 0));
            Point rMax = new Point(Math.Min((int)box.max.X + 2, mTileMap.GetLength(0) - 1), Math.Min((int)box.max.Y + 2, mTileMap.GetLength(1) - 1));

            return new Rectangle(rMin, rMax - rMin);
        }
    }
}
