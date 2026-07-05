using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MyFps
{
    [RequireComponent(typeof(MeshRenderer))]
    public class SceneFader : MonoBehaviour
    {
        #region Variables
        [Header("Fade Settings")]
        private MeshRenderer _faderRenderer;
        [SerializeField] private float _fadeDuration = 1f;
        [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private float _delayTime = 0f;
        [SerializeField] private bool _autoStart = false;

        private Material _fadeMaterial;
        private bool _isFading = false;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 인스펙터에 할당 안 했을 경우 자기 자신에서 찾아보기


            _faderRenderer = GetComponent<MeshRenderer>();

            _fadeMaterial = _faderRenderer.material;

        }

        void Start()
        {
            SetAlpha(1f);
            if (_autoStart)
            {
                StartCoroutine(FadeIn(_delayTime));
            }
        }
        #endregion

        #region Custom Method
        private void SetAlpha(float alpha)
        {
            if (_fadeMaterial != null)
            {
                Color c = _fadeMaterial.color;
                c.a = alpha;
                _fadeMaterial.color = c;
            }

            // 최적화: 투명도가 0일 땐 렌더러를 끕니다.
            if (_faderRenderer != null)
            {
                _faderRenderer.enabled = (alpha > 0.01f);
            }
        }

        public void StartFadeIn(float delayTime)
        {
            StartCoroutine(FadeIn(delayTime));
        }

        private IEnumerator FadeIn(float delayTime)
        {
            _isFading = true;
            if (delayTime > 0f) yield return new WaitForSeconds(delayTime);

            float timer = _fadeDuration; // 1 -> 0

            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                float normalizedTime = Mathf.Clamp01(timer / _fadeDuration);

                SetAlpha(_fadeCurve.Evaluate(normalizedTime));
                yield return null;
            }

            SetAlpha(0f);
            _isFading = false;
        }

        public void FadeTo(string sceneName)
        {
            if (_isFading) return;
            StartCoroutine(FadeOut(sceneName));
        }

        private IEnumerator FadeOut(string sceneName)
        {
            _isFading = true;
            float timer = 0f; // 0 -> 1

            while (timer < _fadeDuration)
            {
                timer += Time.deltaTime;
                float normalizedTime = Mathf.Clamp01(timer / _fadeDuration);

                SetAlpha(_fadeCurve.Evaluate(normalizedTime));
                yield return null;
            }

            SetAlpha(1f);
            SceneManager.LoadScene(sceneName);
        }
        #endregion
    }
}