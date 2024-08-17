namespace AridArnold
{
	/// <summary>
	/// These are all files but exist as an enum to solve the issue
	/// </summary>
	enum AridArnoldSFX
	{
		// Debug
		[FilePath("Sound/SFX/Test")]						Test,

		// Menu
		[FilePath("Sound/SFX/Menu/Confirm")]				MenuConfirm,
		[FilePath("Sound/SFX/Menu/Select")]					MenuSelect,

		// Arnold
		[FilePath("Sound/SFX/Arnold/Jump")]					ArnoldJump,
		[FilePath("Sound/SFX/Arnold/Walk")]					ArnoldWalk,
		[FilePath("Sound/SFX/Arnold/Death")]				ArnoldDeath,
		[FilePath("Sound/SFX/Arnold/Land")]					ArnoldLand,

		// Voice
		[FilePath("Sound/SFX/NPC/A")]						VowelA,
		[FilePath("Sound/SFX/NPC/E")]						VowelE,
		[FilePath("Sound/SFX/NPC/I")]						VowelI,
		[FilePath("Sound/SFX/NPC/O")]						VowelO,
		[FilePath("Sound/SFX/NPC/U")]						VowelU,
		[FilePath("Sound/SFX/NPC/C")]						Consonant,
		[FilePath("Sound/SFX/NPC/AltA")]					AltVowelA,
		[FilePath("Sound/SFX/NPC/AltE")]					AltVowelE,
		[FilePath("Sound/SFX/NPC/AltI")]					AltVowelI,
		[FilePath("Sound/SFX/NPC/AltO")]					AltVowelO,
		[FilePath("Sound/SFX/NPC/AltU")]					AltVowelU,
		[FilePath("Sound/SFX/NPC/AltC")]					AltConsonant,
		[FilePath("Sound/SFX/NPC/RobotA")]					RobotVowelA,
		[FilePath("Sound/SFX/NPC/RobotE")]					RobotVowelE,
		[FilePath("Sound/SFX/NPC/RobotI")]					RobotVowelI,
		[FilePath("Sound/SFX/NPC/RobotO")]					RobotVowelO,
		[FilePath("Sound/SFX/NPC/RobotU")]					RobotVowelU,
		[FilePath("Sound/SFX/NPC/RobotC")]					RobotConsonant,
		[FilePath("Sound/SFX/NPC/Beep")]					Beep,
		[FilePath("Sound/SFX/NPC/Chisel")]					Chisel,

		// Library
		[FilePath("Sound/SFX/Library/BlockLand")]			LibraryBlockLand,
		[FilePath("Sound/SFX/Library/Rotate")]				LibraryRotate,
		[FilePath("Sound/SFX/Library/Farry")]				FarryScream,

		// Cave
		[FilePath("Sound/SFX/Cave/Bounce")]					MushroomBounce,
		[FilePath("Sound/SFX/Cave/Squeak1")]				RatSqueak1,
		[FilePath("Sound/SFX/Cave/Squeak2")]				RatSqueak2,
		[FilePath("Sound/SFX/Cave/Squeak3")]				RatSqueak3,
		[FilePath("Sound/SFX/Cave/Squeak4")]				RatSqueak4,
		[FilePath("Sound/SFX/Cave/Squeak5")]				RatSqueak5,
		[FilePath("Sound/SFX/Cave/Squeak6")]				RatSqueak6,
		[FilePath("Sound/SFX/Cave/Squeak7")]				RatSqueak7,

		// Lab
		[FilePath("Sound/SFX/Lab/NukeAlarm")]				NukeAlarm,
		[FilePath("Sound/SFX/Lab/NukeExplode")]				NukeExplode,
		[FilePath("Sound/SFX/Lab/RobotWalk")]				RobotWalk,
		[FilePath("Sound/SFX/Lab/DoorOpen")]				LabDoorOpen,
		[FilePath("Sound/SFX/Lab/DoorClose")]				LabDoorClose,
		[FilePath("Sound/SFX/Lab/ButtonClick")]				LabButton,
		[FilePath("Sound/SFX/Lab/AndroldBorn")]				AndroldBorn,

		// Mirror
		[FilePath("Sound/SFX/Mirror/MirrorThrough")]		MirrorThrough,
		[FilePath("Sound/SFX/Mirror/MirrorStep")]			MirrorStep,

		// Mountain
		[FilePath("Sound/SFX/Mountain/AgeForward")]			AgeForward,
		[FilePath("Sound/SFX/Mountain/QuickAgeForward")]	QuickAgeForward,
		[FilePath("Sound/SFX/Mountain/AgeBackward")]		AgeBackward,
		[FilePath("Sound/SFX/Mountain/QuickAgeBackward")]	QuickAgeBackward,
		[FilePath("Sound/SFX/Mountain/MamalAngry")]			MamalAngry,
		[FilePath("Sound/SFX/Mountain/BlockPush")]			BlockPush,

		// WW7
		[FilePath("Sound/SFX/WW7/TeleportEmerge")]			TeleportEmerge,
		[FilePath("Sound/SFX/Futron/LaserShoot")]			FutronLaserShoot,
		[FilePath("Sound/SFX/Futron/Laser")]				FutronLaser,
		[FilePath("Sound/SFX/Futron/LaserLand")]			FutronLaserLand,
		[FilePath("Sound/SFX/Futron/BombShoot")]			FutronBombShoot,
		[FilePath("Sound/SFX/Futron/Bomb")]					FutronBomb,
		[FilePath("Sound/SFX/Futron/BombLand")]				FutronBombLand,


		// Misc
		[FilePath("Sound/SFX/Misc/Collect")]				Collect,
		[FilePath("Sound/SFX/Misc/CollectKey")]				CollectKey,
		[FilePath("Sound/SFX/Misc/OneUp")]					OneUp,
		[FilePath("Sound/SFX/Misc/DoorOpen")]				DoorOpen,
		[FilePath("Sound/SFX/Misc/Unlock")]					Unlock,

		// Arcade
		[FilePath("Sound/SFX/Misc/ArcadeGameOver")] ArcadeGameOver,

		[FilePath("Sound/SFX/DeathRide/Convert")]	DeathRideConvert,
		[FilePath("Sound/SFX/DeathRide/PlayerHit")] DeathRidePlayerHit,

		[FilePath("Sound/SFX/HorsesAndGun/GetHerDone")] HorseGetHerDone,
		[FilePath("Sound/SFX/HorsesAndGun/GunShoot")] HorseGunShoot,
		[FilePath("Sound/SFX/HorsesAndGun/LevelUp")] HorseLevelUp,
		[FilePath("Sound/SFX/HorsesAndGun/Reload")] HorseReload,

		[FilePath("Sound/SFX/WormWarp/NewDimension")] WormNewDimension,
		[FilePath("Sound/SFX/WormWarp/PointGet")] WormPointGet,
	}
}
