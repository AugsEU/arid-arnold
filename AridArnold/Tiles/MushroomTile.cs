namespace AridArnold
{
    class MushroomTile : InteractableTile
    {
		#region rMembers

		Animator mBounceAnim;

		#endregion rMembers





		#region rInitialisation

        /// <summary>
        /// Mushroom tile constructor
        /// </summary>
        /// <param name="rotation">Rotation of mushroom</param>
		public MushroomTile(CardinalDirection rotation) : base()
        {
            mRotation = rotation;
        }



        /// <summary>
        /// Load all textures and assets
        /// </summary>
        /// <param name="content">Monogame content manager</param>
        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Tiles/mushroom");
            mBounceAnim = new Animator(Animator.PlayType.OneShot);

            mBounceAnim.LoadFrame(content, "Tiles/mushroom-bounce1", 0.05f);
            mBounceAnim.LoadFrame(content, "Tiles/mushroom-bounce2", 0.1f);
            mBounceAnim.LoadFrame(content, "Tiles/mushroom-bounce1", 0.05f);
            mBounceAnim.LoadFrame(content, "Tiles/mushroom-bounce3", 0.05f);
            mBounceAnim.LoadFrame(content, "Tiles/mushroom-bounce4", 0.05f);
            mBounceAnim.LoadFrame(content, "Tiles/mushroom-bounce3", 0.05f);
        }

        #endregion rInitialisation





        #region rUpdate

        /// <summary>
        /// Update the tile
        /// </summary>
        /// <param name="gameTime">Frame time</param>
        /// <param name="bounds">Bounds of the tile. TO DO: Store this in the tile</param>
        public override void Update(GameTime gameTime, Rect2f bounds)
        {
            mBounceAnim.Update(gameTime);
        }

        #endregion rUpdate




        #region rDraw

        /// <summary>
        /// Get texture for this tile
        /// </summary>
        /// <returns>Texture reference</returns>
        public override Texture2D GetTexture()
        {
            if (mBounceAnim.IsPlaying())
            {
                return mBounceAnim.GetCurrentTexture();
            }

            return mTexture;
        }

        #endregion rDraw





        #region rCollision

        /// <summary>
        /// Make the intersecting entity bounce.
        /// </summary>
        /// <param name="entity">Entity that intersected us</param>
        /// <param name="bounds">Our tile bounds</param>
        public override void OnEntityIntersect(Entity entity, Rect2f bounds)
        {
            const float alpha = 1.4f;
            const float minVel = 19.5f;

            if (entity is PlatformingEntity)
            {
                PlatformingEntity platformingEntity = (PlatformingEntity)entity;
                Rect2f entityBounds = platformingEntity.ColliderBounds();

                bool didBounce = false;

                switch (mRotation)
                {
                    case CardinalDirection.Up:
                    {
                        if (platformingEntity.pGrounded == false)
                        {
                            if (platformingEntity.pVelocity.Y > minVel)
                            {
                                platformingEntity.pVelocity = new Vector2(platformingEntity.pVelocity.X, -platformingEntity.pVelocity.Y * alpha);
                                didBounce = true;
                            }
                            else if (platformingEntity.pVelocity.Y > 0.0f)
                            {
                                platformingEntity.pVelocity = new Vector2(platformingEntity.pVelocity.X, -minVel * alpha);
                                didBounce = true;
                            }

                            if (didBounce)
                            {
                                float newY = bounds.min.Y - entityBounds.Height;
                                platformingEntity.pPosition = new Vector2(platformingEntity.pPosition.X, newY);
                                platformingEntity.pGrounded = true;
                            }
                        }
                    }
                    break;
                    case CardinalDirection.Left:
                    case CardinalDirection.Right:
                    {
                        bool valid = (CardinalDirection.Left == mRotation) != (platformingEntity.pVelocity.X < 0.0f);

                        if (valid)
                        {
                            if (platformingEntity.pGrounded == false)
                            {
                                platformingEntity.pVelocity = new Vector2(-platformingEntity.pVelocity.X, platformingEntity.pVelocity.Y);
                                platformingEntity.ReverseWalkDirection();
                            }
                            else
                            {
                                platformingEntity.pVelocity = new Vector2(-platformingEntity.pVelocity.X, -minVel * alpha);
                                platformingEntity.ReverseWalkDirection();
                            }

                            didBounce = true;
                        }
                    }
                    break;
                    case CardinalDirection.Down:
                    {
                        if (platformingEntity.pGrounded == false)
                        {
                            if (platformingEntity.pVelocity.Y < 0.0f)
                            {
                                platformingEntity.pVelocity = new Vector2(platformingEntity.pVelocity.X, -platformingEntity.pVelocity.Y * alpha);
                                didBounce = true;
                            }
                        }
                    }

                    break;
                }

                if (didBounce)
                {
                    mBounceAnim.Play();
                }
            }
        }



        /// <summary>
        /// Get bounds of this tile.
        /// </summary>
        /// <param name="topLeft">Top left position of tile.</param>
        /// <param name="sideLength">Side length of tile</param>
        /// <returns>Collision rectangle</returns>
        /// <exception cref="NotImplementedException">Requires a valid cardinal direction.</exception>
        public override Rect2f GetBounds(Vector2 topLeft, float sideLength)
        {
            float heightReduction = 6.0f;

            switch (mRotation)
            {
                case CardinalDirection.Up:
                    return new Rect2f(topLeft + new Vector2(0.0f, heightReduction), topLeft + new Vector2(sideLength, sideLength));
                case CardinalDirection.Right:
                    return new Rect2f(topLeft, topLeft + new Vector2(sideLength - heightReduction, sideLength));
                case CardinalDirection.Down:
                    return new Rect2f(topLeft, topLeft + new Vector2(sideLength, sideLength - heightReduction));
                case CardinalDirection.Left:
                    return new Rect2f(topLeft + new Vector2(heightReduction, 0.0f), topLeft + new Vector2(sideLength, sideLength));
            }

            throw new NotImplementedException();
        }

        #endregion rCollision
    }
}
