using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace GMTK2023
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
			Convert,
			PlayerHit,
			GameOver
		}

		Dictionary<MusicType, Song> mSongs;
		Dictionary<SFXType, SoundEffect> mSFX;

		public void LoadContent(ContentManager content)
		{
			//Load songs
			mSongs = new Dictionary<MusicType, Song>();
			mSongs.Add(MusicType.MainGame, content.Load<Song>("Sound/GMTK2023"));

			MediaPlayer.IsRepeating = true;
			MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;

			//LoadSFX
			mSFX = new Dictionary<SFXType, SoundEffect>();

			mSFX.Add(SFXType.GameOver, content.Load<SoundEffect>("Sound/GameOverSFX"));
			mSFX.Add(SFXType.PlayerHit, content.Load<SoundEffect>("Sound/PlayerHit"));
			mSFX.Add(SFXType.Convert, content.Load<SoundEffect>("Sound/Convert"));
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
