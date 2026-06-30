using System.Collections;
using UnityEngine;
using UnityEngine.UI; // UI(Image)를 다루기 위해 추가

namespace MyFps
{
    [RequireComponent(typeof(AudioSource))]
    [RequireComponent(typeof(CharacterInput))]
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        #region Variables
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 20;
        private float currentHealth;
        private bool isDead = false;

        [Header("Damage Effects")]
        [SerializeField] private Image damageFlashImage; // 인스펙터에서 넣을 UI 이미지
        [SerializeField] private float flashDuration = 0.2f; // 깜빡이는 시간
        [SerializeField] private Color flashColor = new(1f, 0f, 0f, 0.5f); // 반투명한 빨간색
        private Coroutine flashCoroutine;

        [Header("Audio")]
        private AudioSource audioSource;
        [SerializeField] private AudioClip[] hurtSounds; // 3개의 사운드를 넣을 배열

        [SerializeField] private SceneFader fader;
        private CharacterInput playerInput;
        #endregion

        #region Unity Event Method
        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            playerInput = GetComponent<CharacterInput>();

            currentHealth = maxHealth;

            // 게임 시작 시 화면 플래쉬 패널을 비활성화 (최적화)
            if (damageFlashImage != null)
            {
                damageFlashImage.color = Color.clear;
                damageFlashImage.gameObject.SetActive(false);
            }
        }
        #endregion

        #region Custom Method
        public void TakeDamage(float damage)
        {
            if (isDead) return;

            currentHealth -= damage;

            // 데미지 효과 실행
            PlayRandomHurtSound();
            TriggerDamageFlash();

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            isDead = true;

            //플레이어 조작 비활성화
            if (playerInput != null) playerInput.enabled = false;

            // 게임오버 씬으로 이동
            fader.FadeTo("GameOver");
        }

        // 데미지 사운드 3개 중 1개 랜덤 발생
        private void PlayRandomHurtSound()
        {
            if (audioSource != null && hurtSounds.Length > 0)
            {
                // 0부터 배열의 크기(3) 미만까지 랜덤 숫자 뽑기 (0, 1, 2)
                int randomIndex = Random.Range(0, hurtSounds.Length);
                audioSource.PlayOneShot(hurtSounds[randomIndex]);
            }
        }

        // 화면 빨간색 플래쉬 효과 (코루틴 사용)
        private void TriggerDamageFlash()
        {
            if (damageFlashImage == null) return;

            // 이미 화면이 빨개지는 중이라면 초기화
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            // 코루틴 시작
            flashCoroutine = StartCoroutine(FlashRoutine());
        }

        // 시간에 따라 서서히 색이 투명해지게 만드는 코루틴
        private IEnumerator FlashRoutine()
        {
            // 이펙트 시작 전 UI 활성화
            damageFlashImage.gameObject.SetActive(true);

            // 처음엔 지정한 반투명 빨간색으로 변경
            damageFlashImage.color = flashColor;

            float elapsedTime = 0f;
            while (elapsedTime < flashDuration)
            {
                elapsedTime += Time.deltaTime;
                // 설정한 빨간색에서 완전 투명(Color.clear)으로 서서히 보간(Lerp)
                damageFlashImage.color = Color.Lerp(flashColor, Color.clear, elapsedTime / flashDuration);

                // 다음 프레임까지 대기
                yield return null;
            }

            // 확실하게 완전 투명으로 마무리한 뒤 UI 비활성화
            damageFlashImage.color = Color.clear;
            damageFlashImage.gameObject.SetActive(false);
        }
        #endregion
    }
}