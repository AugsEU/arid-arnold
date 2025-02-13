﻿namespace AridArnold
{
	internal class FallingFarry : PlatformingEntity
	{
		Animator mFallingAnim;
		SpacialSFX mScreamSFX;

		public FallingFarry(Vector2 pos) : base(pos, 0.0f, 0.0f, 0.3f)
		{
		}

		public override void LoadContent()
		{
			const float FT = 0.1f;
			mFallingAnim = new Animator(Animator.PlayType.Repeat,
											("NPC/FallinFarry/Farry1", FT),
											("NPC/FallinFarry/Farry2", FT),
											("NPC/FallinFarry/Farry3", FT),
											("NPC/FallinFarry/Farry4", FT));
			mFallingAnim.Play();

			mTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/FallinFarry/FarryStand");

			mScreamSFX = new SpacialSFX(AridArnoldSFX.FarryScream, mPosition, 0.6f);
			mScreamSFX.GetBuffer().SetLoop(true);
			mScreamSFX.SetDistanceCutoff(550.0f);
		}

		public override void Update(GameTime gameTime)
		{
			if(!mOnGround)
			{
				if(mScreamSFX.GetBuffer().GetState() != SoundState.Playing)
				{
					SFXManager.I.PlaySFX(mScreamSFX);
				}
			}
			else
			{
				if (mScreamSFX.GetBuffer().GetState() == SoundState.Playing)
				{
					mScreamSFX.Stop(90.0f);
				}
			}

			mScreamSFX.SetPosition(mPosition);
			mFallingAnim.Update(gameTime);
			base.Update(gameTime);
		}

		public override Texture2D GetDrawTexture()
		{
			return mOnGround ? mTexture : mFallingAnim.GetCurrentTexture();
		}

		/// <summary>
		/// This entity can't be killed.
		/// </summary>
		public override void Kill()
		{
		}
	}
}
