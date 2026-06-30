using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace MyFps
{
    /// <summary>
    /// 씬 전환 시 페이드 효과를 관리하는 클래스
    /// 씬 시작 시 페이드 인, 씬 종료 시 페이드 아웃을 담당
    /// 페이드 아웃 시 다음 씬으로 전환하는 기능도 포함
    /// </summary>
    public class SceneFader : MonoBehaviour
    {
        #region Variables
        [SerializeField] private Image _faderImage;
        [SerializeField] private float _fadeDuration = 1f;
        [SerializeField] private AnimationCurve _fadeCurve;
        [SerializeField] private float _delayTime = 0f;

        // 중복 실행 방지를 위한 상태 변수
        private bool _isFading = false;

        [SerializeField] private bool _autoStart = false;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            if (_autoStart) StartCoroutine(FadeIn(_delayTime));
            Color color = _faderImage.color;
            color.a = 1f;
            _faderImage.color = color;
        }
        #endregion

        #region Custom Method
        public void StartFadeIn(float delayTime)
        {
            StartCoroutine(FadeIn(delayTime));
        }

        private IEnumerator FadeIn(float delayTime)
        {
            _isFading = true;
            if (delayTime > 0f)
            {
                yield return new WaitForSeconds(delayTime);
            }

            float timer = _fadeDuration; // 1(Duration)에서 0으로 감소

            while (timer > 0f)
            {
                timer -= Time.deltaTime;
                float normalizedTime = timer / _fadeDuration; // 1 -> 0

                Color color = _faderImage.color;
                color.a = _fadeCurve.Evaluate(normalizedTime);
                _faderImage.color = color;

                yield return null;
            }
            _isFading = false;
        }

        public void FadeTo(string sceneName)
        {
            if (_isFading) return; // 이미 페이드 중이면 무시
            StartCoroutine(FadeOut(sceneName));
        }

        private IEnumerator FadeOut(string sceneName)
        {
            _isFading = true;

            float timer = 0f; // 0에서 1(Duration)로 증가

            while (timer < _fadeDuration)
            {
                timer += Time.deltaTime;
                float normalizedTime = timer / _fadeDuration; // 0 -> 1

                Color color = _faderImage.color;
                color.a = _fadeCurve.Evaluate(normalizedTime);
                _faderImage.color = color;

                yield return null;
            }

            // 페이드 아웃 완료 후 씬 이동
            SceneManager.LoadScene(sceneName);
        }
        #endregion
    }
}