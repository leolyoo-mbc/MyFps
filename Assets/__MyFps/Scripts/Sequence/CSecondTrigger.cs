using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

namespace MyFps
{
    [RequireComponent(typeof(Collider))]
    public class CSecondTrigger : MonoBehaviour
    {
        #region Variables
        [Header("References")]
        [SerializeField] private SlidingDoorController slidingDoor;
        [SerializeField] private CharacterInput playerInput;
        [SerializeField] private GameObject robot;
        [SerializeField] private Transform look;

        [Header("Camera Effect")]
        [SerializeField] private Transform playerCameraRoot;
        [SerializeField] private CinemachineCamera cmCamera;

        [Header("Audio")]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip enemyAppearSound;

        // 중복 실행을 막기 위한 플래그
        private bool hasTriggered = false;
        #endregion

        #region Property
        #endregion

        #region Unity Event Method
        private void Start()
        {
            if (robot != null) robot.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            // 1. 이미 발동되었다면 무시
            if (hasTriggered) return;

            // 2. 플레이어가 닿았을 때만 코루틴 실행 (태그 대신 컴포넌트로 검사)
            if (other.TryGetComponent<CharacterInput>(out _))
            {
                hasTriggered = true;
                StartCoroutine(CSecondSequence());
            }
        }
        #endregion

        #region Custom Method
        private IEnumerator CSecondSequence()
        {
            if (playerInput != null) playerInput.enabled = false;

            float originalFOV = 60f;

            // 시네머신 카메라 자동 찾기
            if (cmCamera == null && Camera.main != null)
            {
                if (Camera.main.TryGetComponent<CinemachineBrain>(out var brain))
                {
                    cmCamera = brain.ActiveVirtualCamera as CinemachineCamera;
                }
            }

            // 문 열기, 적 활성화 및 사운드 재생
            if (slidingDoor != null) slidingDoor.TargetOpenAmount = 1;

            if (robot != null) robot.SetActive(true);
            if (audioSource != null && enemyAppearSound != null) audioSource.PlayOneShot(enemyAppearSound);

            // --- 시점 회전 및 확대 연출 시작 ---
            Quaternion originalRotation = Quaternion.identity;
            if (playerCameraRoot != null && look != null)
            {
                originalRotation = playerCameraRoot.rotation; // 원래 보던 방향 저장

                // 목표(로봇) 방향 계산
                Vector3 directionToRobot = look.transform.position - playerCameraRoot.position;
                Quaternion targetRotation = Quaternion.LookRotation(directionToRobot);

                // 시점 확대 (Zoom in)
                if (cmCamera != null)
                {
                    originalFOV = cmCamera.Lens.FieldOfView;
                    StartCoroutine(ChangeFOVSmoothly(cmCamera, originalFOV, originalFOV - 20f, 0.5f));
                }

                // 동시에 시점 회전 (끝날 때까지 대기)
                yield return RotateCameraSmoothly(originalRotation, targetRotation, 0.5f);
            }

            // 연출을 감상할 시간 부여
            yield return new WaitForSeconds(1.5f);

            // --- 시점 원상 복구 연출 시작 ---
            if (playerCameraRoot != null)
            {
                // 시점 축소 (Zoom out)
                if (cmCamera != null)
                {
                    StartCoroutine(ChangeFOVSmoothly(cmCamera, cmCamera.Lens.FieldOfView, originalFOV, 0.5f));
                }

                // 시점 복구 대기
                yield return RotateCameraSmoothly(playerCameraRoot.rotation, originalRotation, 0.5f);
            }

            // 플레이어 조작 다시 활성화
            if (playerInput != null) playerInput.enabled = true;
        }

        private IEnumerator RotateCameraSmoothly(Quaternion startRot, Quaternion targetRot, float duration)
        {
            if (playerCameraRoot == null) yield break;

            float time = 0f;
            while (time < duration)
            {
                time += Time.deltaTime;
                playerCameraRoot.rotation = Quaternion.Slerp(startRot, targetRot, time / duration);
                yield return null;
            }
            playerCameraRoot.rotation = targetRot;
        }

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