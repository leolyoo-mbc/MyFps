using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; // UI(Image)를 다루기 위해 추가

namespace MyFps
{
    [RequireComponent(typeof(AudioSource))]
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        #region Variables
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 20;
        private float currentHealth;
        private bool isDead = false;

        [Header("Damage Effects")]
        [SerializeField] private GameObject damageFlash;
        [SerializeField] private float flashDuration = 0.2f; // 깜빡이는 시간

        [Header("Audio")]
        [SerializeField] private AudioSource[] hurtSounds; // 3개의 사운드를 넣을 배열

        [SerializeField] private UnityEvent onDeath;

        #endregion

        #region Unity Event Method
        void Awake()
        {
            currentHealth = maxHealth;
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

            onDeath.Invoke();
        }

        // 데미지 사운드 3개 중 1개 랜덤 발생
        private void PlayRandomHurtSound()
        {
            if (hurtSounds.Length > 0)
            {
                // 0부터 배열의 크기(3) 미만까지 랜덤 숫자 뽑기 (0, 1, 2)
                int randomIndex = Random.Range(0, hurtSounds.Length);
                hurtSounds[randomIndex].Play();
            }
        }

        // 화면 빨간색 플래쉬 효과 (코루틴 사용)
        private void TriggerDamageFlash()
        {
            if (damageFlash == null) return;

            // 코루틴 시작
            StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            if (damageFlash.activeSelf) damageFlash.SetActive(false);
            damageFlash.SetActive(true);

            yield return new WaitForSeconds(flashDuration);
            damageFlash.SetActive(false);
        }
        #endregion
    }
}