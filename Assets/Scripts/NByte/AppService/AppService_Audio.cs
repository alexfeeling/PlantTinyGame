using DG.Tweening;
using UnityEngine;

namespace NByte
{
    public partial class AppService
    {
        [SerializeField] private AudioSource MusicPlayer;
        [SerializeField] private AudioSource SoundPlayer;

        private Tweener Tweener { get; set; }

        public void PlayMusic(AudioClip audioClip, float volume = 1)
        {
            if (MusicPlayer.isPlaying)
            {
                Tweener?.Kill();
                Tweener = MusicPlayer.DOFade(0, 1).OnComplete(Play);
            }
            else
            {
                Play();
            }

            void Play()
            {
                MusicPlayer.Stop();
                MusicPlayer.clip = audioClip;
                MusicPlayer.volume = volume;
                MusicPlayer.Play();
            }
        }
        public void StopMusic()
        {
            MusicPlayer.DOFade(0, 1);
        }

        public void PlaySound(AudioClip audioClip, float volume = 1)
        {
            SoundPlayer.PlayOneShot(audioClip, volume);
        }
    }
}