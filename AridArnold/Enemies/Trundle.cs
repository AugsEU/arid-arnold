using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace AridArnold
{
    internal class Trundle : AIEntity
    {
        enum State
        {
            Wait,
            WalkRight,
            WalkLeft,
            ChargeAtPlayer,
            Jump
        }

        const float TRUNDLE_WIDTH_REDUCTION = 6.0f;
        const float TRUNDLE_HEIGHT_REDUCTION = 1.0f;

        const float TRUNDLE_WALK_SPEED = 4.0f;
        readonly float[] TRUNDLE_JUMP_SPEEDS = { 15.0f, 25.0f, 30.0f, 45.0f };

        int mJumpSpeedIdx;
        StateMachine<State> mStateMachine;

        public Trundle(Vector2 pos) : base(pos, TRUNDLE_WALK_SPEED, 0.0f, TRUNDLE_WIDTH_REDUCTION, TRUNDLE_HEIGHT_REDUCTION)
        {
            mJumpSpeedIdx = 0;
            mStateMachine = new StateMachine<State>(State.Wait);
        }

        public override void LoadContent(ContentManager content)
        {
            mTexture = content.Load<Texture2D>("Enemies/trundle/trundle-stand");
            mJumpUpTex = content.Load<Texture2D>("Enemies/trundle/trundle-jump-up");
            mJumpDownTex = content.Load<Texture2D>("Enemies/trundle/trundle-jump-down");

            mRunningAnimation = new Animator();
            mRunningAnimation.LoadFrame(content, "Enemies/trundle/trundle-walk1", 0.12f);
            mRunningAnimation.LoadFrame(content, "Enemies/trundle/trundle-walk2", 0.15f);
            mRunningAnimation.LoadFrame(content, "Enemies/trundle/trundle-walk3", 0.12f);
            mRunningAnimation.LoadFrame(content, "Enemies/trundle/trundle-walk4", 0.12f);
            mRunningAnimation.Play();

            mStandAnimation = new Animator();
            mStandAnimation.LoadFrame(content, "Enemies/trundle/trundle-stand", 0.4f);
            mStandAnimation.LoadFrame(content, "Enemies/trundle/trundle-stand2", 0.7f);
            mStandAnimation.LoadFrame(content, "Enemies/trundle/trundle-stand4", 0.5f);
            mStandAnimation.LoadFrame(content, "Enemies/trundle/trundle-stand3", 0.8f);
            mStandAnimation.Play();

            //Botch position a bit. Not sure what's happening here.
            mPosition.Y -= 2.0f;
        }

        protected override void DecideActions()
        {
            if (mStateMachine.CanMoveState())
            {
                //Walk
                if (mRandom.PercentChance(30.0f))
                {
                    //lmao
                    State newDirection = mRandom.PercentChance(52.0f) ? State.WalkLeft : State.WalkRight;
                    
                    mStateMachine.GoToStateAndWait(newDirection, 1000.0);
                }
                else if(mRandom.PercentChance(30.0f))
                {
                    mStateMachine.GoToStateAndWait(State.Wait, 1500.0);
                }
                else if(mRandom.PercentChance(30.0f))
                {
                    mStateMachine.GoToStateAndWait(State.ChargeAtPlayer, 1500.0);
                }
                else
                {
                    mStateMachine.GoToStateAndWait(State.Jump, 3000.0);
                }
            }

            if (mOnGround)
            {
                Rect2f frontCheck = GetFrontCheckBox();
                bool frontIntersect = TileManager.I.DoesRectTouchTiles(frontCheck);

                if (frontIntersect)
                {
                    mStateMachine.ForceGoToStateAndWait(State.Wait, 500.0);

                    mPrevDirection = mPrevDirection == WalkDirection.Left ? WalkDirection.Right : WalkDirection.Left;
                }
            }

            EnforceState();
        }

        void EnforceState()
        {
            State currentState = mStateMachine.GetState();

            switch (currentState)
            {
                case State.Wait:
                    if(mOnGround) mWalkDirection = WalkDirection.None;
                    break;
                case State.WalkRight:
                    if(mOnGround) mWalkDirection = WalkDirection.Right;
                    break;
                case State.WalkLeft:
                    if(mOnGround) mWalkDirection = WalkDirection.Left;
                    break;
                case State.ChargeAtPlayer:
                    ChargeAtPlayer();
                    break;
                case State.Jump:
                    HandleJumpState();
                    break;
                default:
                    break;
            }
        }

        void ChargeAtPlayer()
        {
            int entityNum = EntityManager.I.GetEntityNum();

            Arnold closestArnold = null;
            float closestDist = float.MaxValue;

            for(int i = 0; i < entityNum; i++)
            {
                Entity entity = EntityManager.I.GetEntity(i);

                if(entity is Arnold)
                {
                    if(closestArnold == null)
                    {
                        closestArnold = (Arnold)entity;
                    }
                    else
                    {
                        float dist = (position - entity.position).LengthSquared();
                        if(dist < closestDist)
                        {
                            closestDist = dist;
                            closestArnold = (Arnold)entity;
                        }
                    }
                }
            }

            float dx = closestArnold.position.X - position.X;

            if (Math.Abs(dx) < 32.0f)
            {
                mWalkDirection = WalkDirection.None;
            }
            else if(dx < 0.0f)
            {
                mWalkDirection = WalkDirection.Left;
            }
            else
            {
                mWalkDirection = WalkDirection.Right;
            }

        }


        void HandleJumpState()
        {
            if(mOnGround)
            {
                if (mWalkDirection == WalkDirection.None)
                {
                    WalkDirection newDirection = mRandom.PercentChance(48.0f) ? WalkDirection.Left : WalkDirection.Right;
                    mWalkDirection = newDirection;
                }

                Rect2f topCheck = GetTopCheckBox();
                bool topIntersect = TileManager.I.DoesRectTouchTiles(topCheck);

                if (topIntersect)
                {
                    mStateMachine.GoToStateAndWait(State.ChargeAtPlayer, 1500.0);
                }
                else
                {
                    mJumpSpeed = TRUNDLE_JUMP_SPEEDS[mJumpSpeedIdx];
                    mJumpSpeedIdx = (mJumpSpeedIdx + 1) % TRUNDLE_JUMP_SPEEDS.Length;
                    Jump();
                }
            }    
        }
    }
}
