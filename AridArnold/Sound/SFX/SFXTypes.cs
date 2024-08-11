namespace AridArnold
{
	/// <summary>
	/// These are all files but exist as an enum to solve the issue
	/// </summary>
	enum AridArnoldSFX
	{
		// Debug
		[FilePath("Sound/SFX/Test")]						Test,

		// Arnold
		[FilePath("Sound/SFX/Arnold/Jump")] ArnoldJump,
		[FilePath("Sound/SFX/Arnold/Walk")] ArnoldWalk,
		[FilePath("Sound/SFX/Arnold/Death")] ArnoldDeath,
		[FilePath("Sound/SFX/Arnold/Land")] ArnoldLand,

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

		// Futron
		[FilePath("Sound/SFX/Futron/LaserShoot")]			FutronLaserShoot,
		[FilePath("Sound/SFX/Futron/Laser")]				FutronLaser,
		[FilePath("Sound/SFX/Futron/LaserLand")]			FutronLaserLand,
		[FilePath("Sound/SFX/Futron/BombShoot")]			FutronBombShoot,
		[FilePath("Sound/SFX/Futron/Bomb")]					FutronBomb,
		[FilePath("Sound/SFX/Futron/BombLand")]				FutronBombLand,

		// Library
		[FilePath("Sound/SFX/Library/BlockLand")]			LibraryBlockLand,
		[FilePath("Sound/SFX/Library/Rotate")]				LibraryRotate,

		// Misc
		[FilePath("Sound/SFX/Misc/Collect")]				Collect,
		[FilePath("Sound/SFX/Misc/CollectKey")]				CollectKey,
		[FilePath("Sound/SFX/Misc/DoorOpen")]				DoorOpen,
		[FilePath("Sound/SFX/Misc/Unlock")]					Unlock,
	}


}
