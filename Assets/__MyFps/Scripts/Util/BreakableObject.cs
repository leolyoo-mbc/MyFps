using UnityEngine;

namespace MyFps
{
    public class BreakableObject : MonoBehaviour, IDamageable
    {
        #region Variables
        [SerializeField] private GameObject unbrokenStateGameObject;
        [SerializeField] private GameObject brokenStateGameObject;
        private bool isBroken = false;

        [SerializeField] private GameObject hiddenItem;
        #endregion

        #region Unity Event Method
        private void Start()
        {
            // 게임 시작 시 초기 상태 보장 (부서지기 전 모습 활성화, 부서진 모습 비활성화)
            if (unbrokenStateGameObject != null) unbrokenStateGameObject.SetActive(true);
            if (brokenStateGameObject != null) brokenStateGameObject.SetActive(false);
            if (hiddenItem != null) hiddenItem.SetActive(false);
        }
        #endregion

        #region Custom Method
        #endregion

        #region IDamageable Implementation
        public void TakeDamage(float damage, UnityEngine.Vector3 hitDirection = default)
        {
            // 이미 부서진 상태라면 더 이상 처리하지 않음
            if (isBroken) return;

            isBroken = true;

            // 온전한 모델 비활성화
            if (unbrokenStateGameObject != null)
                unbrokenStateGameObject.SetActive(false);

            // 파괴된 모델 활성화
            if (brokenStateGameObject != null)
                brokenStateGameObject.SetActive(true);

            if (hiddenItem != null) hiddenItem.SetActive(true);
        }
        #endregion
    }
}