using System.Collections.Generic;

namespace HorsesAndGun
{
	internal class SoundManager : Singleton<SoundManager>
	{
		public enum MusicType
		{
			MainMenu,
			MainGame
		}

		public enum SFXType
		{
			GetHerDone,
			GunReload,
			GunShoot,
			TileActivate,
			GameOver
		}

		Dictionary<MusicType, Song> mSongs;
		Dictionary<SFXType, SoundEffect> mSFX;

		public void LoadContent(ContentManager content)
		{
			//Load songs
			mSongs = new Dictionary<MusicType, Song>();
			mSongs.Add(MusicType.MainGame, content.Load<Song>("Arcade/HorsesAndGun/Music/Horses and gun"));

			MediaPlayer.IsRepeating = true;
			MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;

			//LoadSFX
			mSFX = new Dictionary<SFXType, SoundEffect>();

			mSFX.Add(SFXType.GetHerDone, content.Load<SoundEffect>("Arcade/HorsesAndGun/SFX/getherdone"));
			mSFX.Add(SFXType.GunReload, content.Load<SoundEffect>("Arcade/HorsesAndGun/SFX/reloadsfx"));
			mSFX.Add(SFXType.GunShoot, content.Load<SoundEffect>("Arcade/HorsesAndGun/SFX/gunsfx"));
			mSFX.Add(SFXType.TileActivate, content.Load<SoundEffect>("Arcade/HorsesAndGun/SFX/levelupsfx"));
			mSFX.Add(SFXType.GameOver, content.Load<SoundEffect>("Arcade/HorsesAndGun/SFX/GameOverSFX"));
		}


		void MediaPlayer_MediaStateChanged(object sender, System.EventArgs e)
		{
		}

		public void PlayMusic(MusicType musicType, float volume)
		{
			if (mSongs.ContainsKey(musicType))
			{
				MediaPlayer.Play(mSongs[musicType]);
				MediaPlayer.Volume = volume;
			}
		}

		public void StopMusic()
		{
			MediaPlayer.Stop();
		}

		public void PlaySFX(SFXType sfx, float volume, float pan = 0.0f)
		{
			if (mSFX.ContainsKey(sfx))
			{
				mSFX[sfx].Play(volume, 0.0f, pan);
			}
		}

		public void StopAllSFX()
		{

		}

		public void StopAllSFXOfType(SFXType sfx)
		{

		}
	}
}
