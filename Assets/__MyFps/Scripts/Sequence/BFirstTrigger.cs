using System.Collections;
using TMPro;
using UnityEngine;
using Unity.Cinemachine;

namespace MyFps
{
    [RequireComponent(typeof(Collider))]
    public class BFirstTrigger : MonoBehaviour
    {
        #region Variables
        [SerializeField] private CharacterInput playerInput;
        [SerializeField] private Transform playerCameraRoot;
        [SerializeField] private GameObject guideArrow;
        [SerializeField] private Transform weapon;
        [SerializeField] private TMP_Text sequenceText;
        [SerializeField] private CinemachineCamera cmCamera;
        [SerializeField] private AudioSource playerVoiceSource;
        [SerializeField] private AudioClip playerVoice;

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

            float originalFOV = 60f;

            // 시네머신 카메라가 연결되지 않았다면 Main Camera에서 CinemachineBrain을 통해 현재 활성화된 카메라를 찾습니다.
            if (cmCamera == null && Camera.main != null)
            {
                if (Camera.main.TryGetComponent<CinemachineBrain>(out var brain))
                {
                    cmCamera = brain.ActiveVirtualCamera as CinemachineCamera;
                }
            }

            // --- 시점 회전 및 확대 연출 시작 ---
            Quaternion originalRotation = Quaternion.identity;
            if (playerCameraRoot != null && guideArrow != null)
            {
                originalRotation = playerCameraRoot.rotation; // 원래 보던 방향 저장

                // 목표 방향 계산
                Vector3 directionToWeapon = weapon.transform.position - playerCameraRoot.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToWeapon);

                // 시점 확대 (Zoom in) - 시네머신 렌즈 FOV 조절
                if (cmCamera != null)
                {
                    originalFOV = cmCamera.Lens.FieldOfView;
                    // FOV를 20 줄여서 줌인 효과 적용
                    StartCoroutine(ChangeFOVSmoothly(cmCamera, originalFOV, originalFOV - 30f, 0.5f));
                }

                // 동시에 시점 회전 (끝날 때까지 대기)
                yield return RotateCameraSmoothly(originalRotation, targetRotation, 0.5f);

            }
            // --- 시점 회전 연출 끝 ---

            // 2. 대사 출력
            if (sequenceText != null)
            {
                sequenceText.text = "Looks like a weapon on that table.";
                sequenceText.gameObject.SetActive(true);
                if (playerVoiceSource != null && playerVoice != null) playerVoiceSource.PlayOneShot(playerVoice);
            }

            // 3. 1초 딜레이
            yield return new WaitForSeconds(1.0f);

            // 4. 화살표 활성화
            if (guideArrow != null) guideArrow.SetActive(true);

            // 5. 1초 딜레이
            yield return new WaitForSeconds(1.0f);

            // --- 시점 원상 복구 연출 시작 ---
            if (playerCameraRoot != null)
            {
                // 시점 축소 (Zoom out) - 카메라 회전과 동시에 부드럽게 복구
                if (cmCamera != null)
                {
                    StartCoroutine(ChangeFOVSmoothly(cmCamera, cmCamera.Lens.FieldOfView, originalFOV, 0.5f));
                }

                // 함수로 추출한 코루틴 호출 (끝날 때까지 대기)
                yield return RotateCameraSmoothly(playerCameraRoot.rotation, originalRotation, 0.5f);
            }
            // --- 시점 원상 복구 연출 끝 ---

            // 6. 대사 숨기기 및 플레이 캐릭터 다시 활성화
            if (sequenceText != null) sequenceText.gameObject.SetActive(false);
            if (playerInput != null) playerInput.enabled = true;
        }

        // 중복을 제거하기 위해 추출한 공통 회전 코루틴 함수
        private IEnumerator RotateCameraSmoothly(Quaternion startRot, Quaternion targetRot, float duration)
        {
            if (playerCameraRoot == null) yield break;

            float time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                playerCameraRoot.rotation = Quaternion.Slerp(startRot, targetRot, time / duration);
                yield return null; // 한 프레임 대기
            }
            playerCameraRoot.rotation = targetRot; // 마지막에 오차 없이 완벽히 각도 맞춤
        }

        // 시네머신의 시점(FOV)을 부드럽게 조절하는 코루틴
        private IEnumerator ChangeFOVSmoothly(CinemachineCamera cam, float startFOV, float targetFOV, float duration)
        {
            if (cam == null) yield break;

            float time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                var lens = cam.Lens;
                lens.FieldOfView = Mathf.Lerp(startFOV, targetFOV, time / duration);
                cam.Lens = lens;
                yield return null;
            }
            var finalLens = cam.Lens;
            finalLens.FieldOfView = targetFOV;
            cam.Lens = finalLens;
        }
        #endregion
    }
}