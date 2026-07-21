using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyFps
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SceneFader : MonoBehaviour
    {
        #region Variables
        [Header("Fade Settings")]
        [SerializeField] private float _fadeDuration = 1f;
        [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float delayTime = 0f;
        [SerializeField] private bool _autoStart = true;
        private CanvasGroup _fadeCanvasGroup;
        [SerializeField] private bool _isFadingOut = false;
        [Range(0f, 1f)]
        [SerializeField] private float _startAlpha = 1f;

        public float DelayTime { get => delayTime; set => delayTime = value; }
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            _fadeCanvasGroup = GetComponent<CanvasGroup>();
        }
        void Start()
        {
            _fadeCanvasGroup.alpha = _startAlpha;
            if (_autoStart)
            {
                StartCoroutine(FadeIn());
            }
        }
        #endregion

        #region Custom Method
        public void StartFadeIn()
        {
            StartCoroutine(FadeIn());
        }


        private IEnumerator FadeIn()
        {
            if (delayTime > 0f) yield return new WaitForSeconds(delayTime);

            float timer = _fadeDuration; // 1 -> 0

            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                float normalizedTime = Mathf.Clamp01(timer / _fadeDuration);

                _fadeCanvasGroup.alpha = _fadeCurve.Evaluate(normalizedTime);
                yield return null;
            }
        }

        public void FadeTo(string sceneName)
        {
            if (_isFadingOut) return;
            StartCoroutine(FadeOut(sceneName));
        }

        public void FadeTo(int sceneIndex)
        {
            if (_isFadingOut) return;
            StartCoroutine(FadeOutIndex(sceneIndex));
        }

        private IEnumerator FadeOut(string sceneName)
        {
            _isFadingOut = true;
            yield return FadeOutRoutine();
            SceneManager.LoadScene(sceneName);
        }

        private IEnumerator FadeOutIndex(int sceneIndex)
        {
            _isFadingOut = true;
            yield return FadeOutRoutine();
            SceneManager.LoadScene(sceneIndex);
        }

        private IEnumerator FadeOutRoutine()
        {
            float timer = 0f; // 0 -> 1

            while (timer < _fadeDuration)
            {
                timer += Time.deltaTime;
                float normalizedTime = Mathf.Clamp01(timer / _fadeDuration);

                _fadeCanvasGroup.alpha = _fadeCurve.Evaluate(normalizedTime);
                yield return null;
            }
        }
        #endregion
    }
}