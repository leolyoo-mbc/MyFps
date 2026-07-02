using UnityEngine;

namespace MyFps
{
    [RequireComponent(typeof(AudioSource))]
    public class BGMManager : Singleton<BGMManager>
    {
        #region Variables
        AudioSource audioSource;
        [SerializeField] private AudioClip mainBgm;
        [SerializeField] private AudioClip combatBgm;
        #endregion

        #region Property
        public AudioClip MainBgm => mainBgm;
        public AudioClip CombatBgm => combatBgm;
        #endregion

        #region Unity Event Method
        protected override void Awake()
        {
            base.Awake();
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            Play(MainBgm);
        }
        #endregion

        #region Custom Method
        public void Play(AudioClip clip)
        {
            audioSource.Stop();
            if (clip == null) return;
            audioSource.clip = clip;
            audioSource.Play();
        }
        #endregion
    }
}