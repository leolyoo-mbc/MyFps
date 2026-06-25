using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyFps
{
    public class AOpening : MonoBehaviour
    {
        #region Variables
        [SerializeField] private CharacterInput playerInput;
        [SerializeField] private SceneFader fader;
        [SerializeField] private TMP_Text sequenceText;
        #endregion

        #region Property
        #endregion

        #region Unity Event Method
        private void Start()
        {
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
            yield return new WaitForSeconds(3.0f);
            sequenceText.gameObject.SetActive(false);
            playerInput.enabled = true;

        }
        #endregion
    }
}
