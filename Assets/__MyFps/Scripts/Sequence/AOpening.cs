using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyFps
{
    public class AOpening : MonoBehaviour
    {
        #region Variables
        [SerializeField] private PlayerStatsData playerStats;
        [SerializeField] private CharacterInput playerInput;
        [SerializeField] private SceneFader fader;
        [SerializeField] private TMP_Text sequenceText;
        [SerializeField] private AudioSource playerVoiceSource;
        [SerializeField] private AudioClip playerVoice;
        #endregion

        #region Property
        #endregion

        #region Unity Event Method
        private void Start()
        {
            // 게임의 첫 오프닝 씬이 켜질 때 전역 데이터(SO)를 모두 0(초기값)으로 밀어줍니다!
            if (playerStats != null) playerStats.AmmoCount = 0;

            sequenceText.gameObject.SetActive(false);
            StartCoroutine(OpeningSequence());
        }
        #endregion

        #region Custom Method
        private IEnumerator OpeningSequence()
        {

            playerInput.enabled = false;
            fader.StartFadeIn(1.0f);
            yield return new WaitForSeconds(1.0f);
            sequenceText.text = "I need to get out of here.";
            sequenceText.gameObject.SetActive(true);
            if (playerVoiceSource != null && playerVoice != null) playerVoiceSource.PlayOneShot(playerVoice);
            yield return new WaitForSeconds(3.0f);
            sequenceText.gameObject.SetActive(false);
            playerInput.enabled = true;

        }
        #endregion
    }
}
