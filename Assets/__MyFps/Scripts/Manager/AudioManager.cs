using UnityEngine;

namespace MyFps
{
    public class AudioManager : PersistentSingleton<AudioManager>
    {
        #region Variables
        [Header("Audio Sources")]
        public Sound[] sounds;

        [SerializeField] private string bgmSound = "";
        #endregion

        #region Unity Event Method
        protected override void Awake()
        {
            base.Awake();

            foreach (var sound in sounds)
            {
                sound.audioSource = gameObject.AddComponent<AudioSource>();
                sound.audioSource.clip = sound.clip;
                sound.audioSource.volume = sound.volume;
                sound.audioSource.pitch = sound.pitch;
                sound.audioSource.loop = sound.loop;
                sound.audioSource.playOnAwake = sound.playOnAwake;
            }
        }
        #endregion

        #region Custom Method
        public void Play(string name)
        {
            // 이름으로 재생할 사운드 찾기
            Sound sound = null;
            foreach (var s in sounds)
            {
                if (s.name == name)
                {
                    sound = s;
                    break;
                }
            }

            if (sound == null)
            {
                Debug.Log($"Cannot Found {name} Sound");
                return;
            }

            sound.audioSource.Play();
        }

        public void Stop(string name)
        {
            // 이름으로 중지시킬 사운드 찾기
            Sound sound = null;
            foreach (var s in sounds)
            {
                if (s.name == name)
                {
                    sound = s;
                    break;
                }
            }

            if (sound == null)
            {
                Debug.Log($"Cannot Found {name} Sound");
                return;
            }

            sound.audioSource.Stop();
        }

        public void PlayBGM(string name)
        {
            // 현재 플레이되는 배경음 체크
            if (bgmSound == name)
            {
                return;
            }

            Stop(bgmSound);

            // 이름으로 재생할 사운드 찾기
            Sound sound = null;
            foreach (var s in sounds)
            {
                if (s.name == name)
                {
                    sound = s;
                    bgmSound = s.name;
                    break;
                }
            }

            if (sound == null)
            {
                Debug.Log($"Cannot Found {name} Sound");
                return;
            }

            sound.audioSource.Play();
        }
        #endregion
    }
}