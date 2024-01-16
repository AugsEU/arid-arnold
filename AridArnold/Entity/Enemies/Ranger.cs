using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AridArnold
{
	internal class Ranger : AIEntity
	{
		#region rTypes

		enum State
		{
			Wait,
			ChargeAtPlayer,
		}

		#endregion rTypes





		#region rMembers


		#endregion rMembers





		#region rInit

		/// <summary>
		/// Spawn ranger at specific time
		/// </summary>
		/// <param name="pos">Spawn pos</param>
		/// <param name="shootPhase">How far in the shoot cycle to start</param>
		/// <param name="shootFreq">How long the shoot cycle is</param>
		public Ranger(Vector2 pos, float shootPhase, float shootFreq) : base(pos, shootPhase, shootFreq)
		{
		}



		/// <summary>
		/// Load ranger content
		/// </summary>
		public override void LoadContent()
		{
			const float FT = 0.2f;
			mIdleAnim = new Animator(Animator.PlayType.Repeat, ("Enemies/Futron-Gun/Idle1", FT),
															   ("Enemies/Futron-Gun/Idle2", FT));

			mChargeGunAnim = new Animator(Animator.PlayType.OneShot, ("Enemies/Futron-Gun/Charge1", FT),
																	 ("Enemies/Futron-Gun/Charge2", FT),
																	 ("Enemies/Futron-Gun/Charge3", FT),
																	 ("Enemies/Futron-Gun/Charge4", FT),
																	 ("Enemies/Futron-Gun/Charge5", FT),
																	 ("Enemies/Futron-Gun/Charge6", FT),
																	 ("Enemies/Futron-Gun/Charge5", FT),
																	 ("Enemies/Futron-Gun/Charge6", FT));

			mShootGunAnim = new Animator(Animator.PlayType.OneShot, ("Enemies/Futron-Gun/Shoot1", 0.15f),
																	("Enemies/Futron-Gun/Shoot2", 0.15f),
																	("Enemies/Futron-Gun/Shoot3", 0.15f),
																	("Enemies/Futron-Gun/Shoot4", 0.15f));

			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Enemies/Futron-Gun/Idle1");
		}

		#endregion rInit





		#region rUpdate

		protected override void DecideActions()
		{
			throw new NotImplementedException();
		}

		#endregion rUpdate





		#region rDraw

		#endregion rDraw
	}
}
