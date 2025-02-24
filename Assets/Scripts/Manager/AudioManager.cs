using System;
using _Scripts.Extension;
using DG.Tweening;
using UnityEngine;

namespace _Scripts
{
    [Serializable]
    public class SoundAndEffect
    {
        public ParticleSystem effect;
        public AudioClip audioClip;
    }
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Audio Souce")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource soundSource;

        [Header("Audio Clip")]
        [SerializeField] private AudioClip buttonClick;
        [SerializeField] private AudioClip toggleClickOn;
        [SerializeField] private AudioClip toggleClickOff;

        [Space] [Header("Audio in Game")] 
        [SerializeField] private AudioClip soundClickCan;
        [SerializeField] private AudioClip[] soundConnect;
        [SerializeField] private AudioClip soundWin;
        [SerializeField] private AudioClip soundLose;
        [Space]
        [Header("Audio awsome")]
        [SerializeField] SoundAndEffect[] soundAndEffectAwsomes;
        [Header("Audio no awsome")]
        [SerializeField] SoundAndEffect[] soundAndEffectNoAwsomes;
        
        protected override void Awake()
        {
            base.KeepAlive(false);
            base.Awake();
        }
        
        void Start()
        {
            SetMuteSounds();
            SetMuteMusic();
        }

        public void PlaySoundClickObject()
        {
            PlaySfx(soundClickCan);
        }

        public void PlaySoundConnect(int index)
        {
            PlaySfx(soundConnect[index]);
        }
        private bool canPlayMusic = true;
        public void PlaySoundDoanDung()
        {
            int randomIndex = UnityEngine.Random.Range(0, soundAndEffectAwsomes.Length);
            var selectedSound = soundAndEffectAwsomes[randomIndex];

            if (!selectedSound.effect.isPlaying && canPlayMusic)
            {
                selectedSound.effect.Play();
                PlaySfx(selectedSound.audioClip);
                canPlayMusic = false;
                DOVirtual.DelayedCall(selectedSound.audioClip.length + 0.3f, () => canPlayMusic = true);
            }
        }
        public void PlaySoundDoanSai()
        {
            int randomIndex = UnityEngine.Random.Range(0, soundAndEffectNoAwsomes.Length);
            var selectedSound = soundAndEffectNoAwsomes[randomIndex];

            if (!selectedSound.effect.isPlaying && !IsAudioPlaying(selectedSound.audioClip))
            {
                selectedSound.effect.Play();
                PlaySfx(selectedSound.audioClip);
            }
        }
        private bool IsAudioPlaying(AudioClip clip)
        {
            return soundSource.isPlaying && soundSource.clip == clip;
        }
        public void SetMuteSounds()
        {
            if (UIController.instance.UISetting.IsMuteSound)
            {
                soundSource.mute = true;
                return;
            }
            soundSource.mute = false;
        }
        public void SetMuteMusic()
        {
            if (UIController.instance.UISetting.IsMuteMusic)
            {
                musicSource.mute = true;
                return;
            }
            musicSource.mute = false;
        }

        public void SetVolume(float volume)
        {
            musicSource.volume = volume;
            soundSource.volume = volume;
        }

        public void PlaySoundButtonClick()
        {
            PlaySfx(buttonClick);
            Vibration.Vibrate(28);
        }

        public void PlaySoundToggleClickOn()
        {
            PlaySfx(toggleClickOn);
            Vibration.Vibrate(28);
        }

        public void PlaySoundToggleClickOff()
        {
            PlaySfx(toggleClickOff);
            Vibration.Vibrate(28);
        }
        public void PlayInGameMusic(AudioClip audioClip)
        {
            if (!musicSource.isPlaying)
            {
                if (audioClip != null)
                {
                    musicSource.clip = audioClip;
                    musicSource.DOFade(1f, 0.5f).OnPlay(() =>
                    {
                        musicSource.Play();
                    }).SetUpdate(true);
                }
            }
        }

        public void StopMusic()
        {
            if (musicSource.isPlaying)
            {
                musicSource.DOFade(0f, 0.5f).OnComplete(() =>
                {
                    musicSource.Stop();
                }).SetUpdate(true);
            }
        }

        public void StopSound()
        {
            if (soundSource.isPlaying)
            {
                if (soundSource.loop == true)
                {
                    soundSource.loop = false;
                }
                soundSource.Stop();
            }
        }

        public void PlaySfx(AudioClip audioClip, bool repeat = false)
        {
            if (UIController.instance.UISetting.IsMuteSound) return;

            if (audioClip != null)
            {
                if (repeat)
                {
                    soundSource.loop = true;
                    soundSource.clip = audioClip;
                    soundSource.Play();
                }
                else
                {
                    soundSource.loop = false;
                    soundSource.PlayOneShot(audioClip);
                }

            }
        }
        public void PlaySoundWin()
        {
            PlaySfx(soundWin);
        }

        public void PlaySoundWinWithFireWork()
        {
            
        }

        public void PlaySoundLose()
        {
            PlaySfx(soundLose);
        }
    }
}