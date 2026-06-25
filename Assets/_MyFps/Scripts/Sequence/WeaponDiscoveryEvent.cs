using System.Collections;
using TMPro;
using UnityEngine;

namespace MyFps
{
    [RequireComponent(typeof(Collider))]
    public class WeaponDiscoveryEvent : MonoBehaviour
    {
        #region Variables
        [Header("References")]
        [SerializeField] private CharacterInput playerInput;
        [SerializeField] private Transform playerCamera; // 추가: 부드럽게 회전시킬 카메라
        [SerializeField] private GameObject guideArrow;
        [SerializeField] private TMP_Text sequenceText;

        // 중복 실행을 막기 위한 플래그
        private bool hasTriggered = false;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            // 혹시라도 시작할 때 화살표가 켜져 있으면 강제로 끕니다.
            if (guideArrow != null)
            {
                guideArrow.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // 1. 이미 발동되었다면 무시
            if (hasTriggered) return;

            // 2. 플레이어가 닿았을 때만 코루틴 실행 (태그 대신 컴포넌트로 검사)
            if (other.TryGetComponent<CharacterInput>(out _))
            {
                hasTriggered = true;
                StartCoroutine(WeaponEventSequence());
            }
        }
        #endregion

        #region Custom Method
        private IEnumerator WeaponEventSequence()
        {
            // 1. 플레이어 조작 비활성화 (플레이 멈춤)
            if (playerInput != null) playerInput.enabled = false;

            // --- 시점 회전 연출 시작 ---
            Quaternion originalRotation = Quaternion.identity;
            if (playerCamera != null && guideArrow != null)
            {
                originalRotation = playerCamera.rotation; // 원래 보던 방향 저장
                
                // 목표 방향 계산
                Vector3 directionToArrow = guideArrow.transform.position - playerCamera.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToArrow);

                // 함수로 추출한 코루틴 호출 (끝날 때까지 대기)
                yield return RotateCameraSmoothly(originalRotation, targetRotation, 0.5f);
            }
            // --- 시점 회전 연출 끝 ---

            // 2. 대사 출력
            if (sequenceText != null)
            {
                sequenceText.text = "Looks like a weapon on that table.";
                sequenceText.gameObject.SetActive(true);
            }

            // 3. 1초 딜레이
            yield return new WaitForSeconds(1.0f);

            // 4. 화살표 활성화
            if (guideArrow != null) guideArrow.SetActive(true);

            // 5. 1초 딜레이
            yield return new WaitForSeconds(1.0f);

            // --- 시점 원상 복구 연출 시작 ---
            if (playerCamera != null)
            {
                // 함수로 추출한 코루틴 호출 (끝날 때까지 대기)
                yield return RotateCameraSmoothly(playerCamera.rotation, originalRotation, 0.5f);
            }
            // --- 시점 원상 복구 연출 끝 ---

            // 6. 대사 숨기기 및 플레이 캐릭터 다시 활성화
            if (sequenceText != null) sequenceText.gameObject.SetActive(false);
            if (playerInput != null) playerInput.enabled = true;
        }

        // 중복을 제거하기 위해 추출한 공통 회전 코루틴 함수
        private IEnumerator RotateCameraSmoothly(Quaternion startRot, Quaternion targetRot, float duration)
        {
            if (playerCamera == null) yield break;

            float time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                playerCamera.rotation = Quaternion.Slerp(startRot, targetRot, time / duration);
                yield return null; // 한 프레임 대기
            }
            playerCamera.rotation = targetRot; // 마지막에 오차 없이 완벽히 각도 맞춤
        }
        #endregion
    }
}