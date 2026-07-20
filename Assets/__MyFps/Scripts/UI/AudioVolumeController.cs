using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace MyFps
{
    public class AudioVolumeController : MonoBehaviour
    {
        private const string SAVED_BGM_VOLUME_KEY = "SavedBGMVolume";
        private const string SAVED_SFX_VOLUME_KEY = "SavedSFXVolume";

        [Header("Audio Mixer")]
        public AudioMixer mainMixer;

        [Header("UI Sliders")]
        public Slider bgmSlider;
        public Slider sfxSlider;

        [Header("Mixer Parameter Names")]
        public string bgmParameterName = "BGMVolume";
        public string sfxParameterName = "SFXVolume";

        private void Start()
        {
            if (bgmSlider != null) bgmSlider.onValueChanged.AddListener(SetBGMVolume);
            if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(SetSFXVolume);

            if (bgmSlider != null) bgmSlider.value = PlayerPrefs.GetFloat(SAVED_BGM_VOLUME_KEY, 1f);
            if (sfxSlider != null) sfxSlider.value = PlayerPrefs.GetFloat(SAVED_SFX_VOLUME_KEY, 1f);
        }
        private void OnDestroy()
        {
            if (bgmSlider != null) bgmSlider.onValueChanged.RemoveListener(SetBGMVolume);
            if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(SetSFXVolume);
        }

        public void SetBGMVolume(float sliderValue)
        {
            if (mainMixer != null)
            {
                float dbVolume = sliderValue <= 0.001f ? -80f : Mathf.Log10(sliderValue) * 20f;
                mainMixer.SetFloat(bgmParameterName, dbVolume);

                // 플레이어가 슬라이더를 조작할 때마다 그 값을 즉시 저장합니다.
                PlayerPrefs.SetFloat(SAVED_BGM_VOLUME_KEY, sliderValue);
            }
        }

        public void SetSFXVolume(float sliderValue)
        {
            if (mainMixer != null)
            {
                float dbVolume = sliderValue <= 0.001f ? -80f : Mathf.Log10(sliderValue) * 20f;
                mainMixer.SetFloat(sfxParameterName, dbVolume);

                // 플레이어가 슬라이더를 조작할 때마다 그 값을 즉시 저장합니다.
                PlayerPrefs.SetFloat(SAVED_SFX_VOLUME_KEY, sliderValue);
            }
        }
    }
}
