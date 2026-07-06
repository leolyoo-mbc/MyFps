using UnityEngine;

namespace MySample
{
    public class SoundTest : MonoBehaviour
    {
        #region Variables
        // 재생할 오디오 소스 속성
        [SerializeField] private AudioClip clip;

        [Range(0, 1)]
        [SerializeField] private float volume = 1f;
        [Range(-3, 3)]
        [SerializeField] private float pitch = 1f;
        [SerializeField] private bool loop = false;
        [SerializeField] private bool playOnAwake = false;

        private AudioSource audioSource;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (TryGetComponent<AudioSource>(out var audioSource))
            {
                this.audioSource = audioSource;
            }
            else
            {
                this.audioSource = gameObject.AddComponent<AudioSource>();
            }


            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.loop = loop;
            audioSource.playOnAwake = playOnAwake;
        }

        private void Start()
        {
            audioSource.Play();
        }
        #endregion
    }
}