using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;


namespace AridArnold
{
    class GhostArnold : Arnold
    {
        public GhostArnold(Vector2 startPos) : base(startPos)
        {
           
        }

        public void StartLevel()
        {
            mPrevDirection = WalkDirection.Right;
            mVelocity = Vector2.Zero;
        }

        public override void Update(GameTime gameTime)
        {
            SetDirFromVelocity();
            mRunningAnimation.Update(gameTime);
        }

        public void SetGhostInfo(GhostInfo info)
        {
            mPosition = info.position;
            mVelocity = info.velocity;
            mOnGround = info.grounded;
            SetGravity(info.gravity);
        }

        protected override Color GetDrawColour()
        {
            return new Color(0.0f, 0.4f, 0.0f, 0.9f);
        }
    }
}
