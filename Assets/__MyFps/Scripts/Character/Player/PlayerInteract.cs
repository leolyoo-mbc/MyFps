using TMPro;
using UnityEngine;

namespace MyFps
{
    public class PlayerInteract : MonoBehaviour
    {
        #region Variables
        public bool interactIntent = false;

        [SerializeField] private Transform cameraRoot;

        [SerializeField] private float maxDistance = 100f;
        [SerializeField] private float maxInteractableDistance = 2.0f;

        private Collider lastCollider;
        private IInteractable lastInteractable;

        [SerializeField] private GameObject actionUI;
        [SerializeField] private TMP_Text actionText;
        [SerializeField] private GameObject extraCross;
        #endregion

        #region Unity Event Method
        private void Update()
        {
            Collider currentCollider = null;
            // 1. 이번 프레임에서 거리가 만족되는 콜라이더 찾기
            if (Physics.Raycast(cameraRoot.position, cameraRoot.forward, out RaycastHit hitInfo, maxDistance))
            {
                if (hitInfo.distance < maxInteractableDistance)
                {
                    currentCollider = hitInfo.collider;
                }
            }
            // 2. 만약 시선이 닿은 콜라이더가 예전과 달라졌다면? (시선을 돌렸거나, 거리가 멀어졌거나)
            // 여기서부터 비용이 드는 연산(GetComponent)과 UI 처리를 딱 한 번만 실행합니다.
            if (currentCollider != lastCollider)
            {
                // (1) 예전에 포커스된 오브젝트가 있었다면 꺼줍니다.
                ClearInteractState();
                // (2) 새로 시선이 닿은 물체가 있다면 인터페이스가 있는지 검사합니다.
                if (currentCollider != null)
                {
                    // 이 무거운 함수는 시선이 '바뀔 때'만 호출됩니다!
                    IInteractable interactable = currentCollider.GetComponentInParent<IInteractable>();
                    if (interactable != null)
                    {
                        lastInteractable = interactable;
                        if (actionUI != null) actionUI.SetActive(true);
                        if (actionText != null) actionText.SetText(lastInteractable.ActionText);
                        if (extraCross != null) extraCross.SetActive(true);
                    }
                }
                // (3) 다음 프레임 비교를 위해 콜라이더 갱신
                lastCollider = currentCollider;
            }

            if (interactIntent)
            {
                lastInteractable?.OnInteract(gameObject);

                // 예전에 포커스된 오브젝트 관련 UI를 즉시 꺼줍니다.
                // (만약 상호작용으로 인해 물체가 파괴되어 다음 프레임에 currentCollider가 null이 되면
                // null != null이 되어 UI 끄는 로직이 실행되지 않는 문제를 방지합니다.)
                ClearInteractState();

                // 강제로 다음 프레임에 오브젝트 상태를 다시 체크하고 UI를 갱신하도록 콜라이더를 초기화합니다.
                lastCollider = null;
            }
            interactIntent = false;
        }

        private void OnDrawGizmosSelected()
        {
            if (cameraRoot == null) return; // playerEye가 할당되지 않았다면 그리지 않음 (에러 방지)

            Gizmos.color = Color.red;

            // 레이저가 무언가에 맞았다면 맞은 곳까지만 선을 그리고
            if (Physics.Raycast(cameraRoot.position, cameraRoot.forward, out RaycastHit hitInfo, maxDistance))
            {
                Gizmos.DrawRay(cameraRoot.position, cameraRoot.forward * hitInfo.distance);
            }
            // 맞은 게 없다면 최대 거리(maxDistance)만큼 선을  그린다
            else
            {
                Gizmos.DrawRay(cameraRoot.position, cameraRoot.forward * maxDistance);
            }
        }
        #endregion

        #region Custom Method
        private void ClearInteractState()
        {
            if (lastInteractable != null)
            {
                if (actionUI != null) actionUI.SetActive(false);
                if (extraCross != null) extraCross.SetActive(false);
            }
            lastInteractable = null;
        }
        #endregion
    }
}