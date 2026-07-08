using System.Collections;
using UnityEngine;

namespace MyFps
{
    [RequireComponent(typeof(MeshRenderer))]
    public class HurtEffect : MonoBehaviour
    {
        [Header("Hurt Effect Settings")]
        [SerializeField] private float _fadeDuration = 0.5f;
        [SerializeField] private float _maxAlpha = 0.8f;
        [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.Linear(0, 1, 1, 0);

        private MeshRenderer _effectRenderer;
        private Material _effectMaterial;
        private Coroutine _fadeCoroutine;

        private void Awake()
        {
            _effectRenderer = GetComponent<MeshRenderer>();
            _effectMaterial = _effectRenderer.material;
        }

        private void Start()
        {
            SetAlpha(0f);
        }

        private void SetAlpha(float alpha)
        {
            if (_effectMaterial != null)
            {
                // URP Unlit 셰이더는 _BaseColor를, 일반 셰이더는 _Color를 사용합니다.
                string colorProp = _effectMaterial.HasProperty("_BaseColor") ? "_BaseColor" : "_Color";
                Color c = _effectMaterial.GetColor(colorProp);
                c.a = alpha;
                _effectMaterial.SetColor(colorProp, c);
            }

            if (_effectRenderer != null)
            {
                _effectRenderer.enabled = (alpha > 0.01f);
            }
        }

        public void PlayHurtEffect()
        {
            if (_fadeCoroutine != null)
            {
                StopCoroutine(_fadeCoroutine);
            }
            _fadeCoroutine = StartCoroutine(FadeEffect());
        }

        private IEnumerator FadeEffect()
        {
            // 즉시 최대 알파값으로 시작
            SetAlpha(_maxAlpha);

            float timer = 0f;

            while (timer < _fadeDuration)
            {
                timer += Time.deltaTime;
                float normalizedTime = Mathf.Clamp01(timer / _fadeDuration);
                
                // Curve(0)=1에서 Curve(1)=0으로 서서히 줄어듦
                SetAlpha(_maxAlpha * _fadeCurve.Evaluate(normalizedTime));
                yield return null;
            }

            SetAlpha(0f);
            _fadeCoroutine = null;
        }
    }
}
