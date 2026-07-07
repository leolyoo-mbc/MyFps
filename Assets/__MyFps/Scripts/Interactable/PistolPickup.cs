using UnityEngine;

namespace MyFps
{
    public class PistolPickup : MonoBehaviour, IInteractable
    {
        #region Variables
        [SerializeField] private GameObject pistolInPlayerHand;
        [SerializeField] private GameObject guideArrow;
        [SerializeField] private GameObject ammoUI;

        public string ActionText => "PICK UP PISTOL";
        #endregion

        #region Unity Event Method
        #endregion

        #region Custom Method
        public void OnInteract(GameObject interactor)
        {
            //-테이블 위의 총은 없어지고 - 비활성화
            gameObject.SetActive(false);
            //- 오른손 쪽의 총은 화면 출력 -활성화
            if (pistolInPlayerHand != null) pistolInPlayerHand.SetActive(true);
            //- 책상위의 가이드 화살표는 없어진다
            if (guideArrow != null) guideArrow.SetActive(false);
            if (ammoUI != null) ammoUI.SetActive(true);
        }
        #endregion
    }
}