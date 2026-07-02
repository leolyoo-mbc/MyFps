using TMPro;
using UnityEngine;

namespace MyFps
{
    public class AmmoBoxPickup : MonoBehaviour, IInteractable
    {
        #region Variables
        [SerializeField] private GameObject actionUI;
        [SerializeField] private TMP_Text actionText;
        [SerializeField] private string InteractText = "PICK UP AMMO";

        [SerializeField] private GameObject extraCross;

        [SerializeField] private GameObject ammoUI;
        [SerializeField] private PlayerStatsData playerStats; // SO 참조 추가

        #endregion

        #region Unity Event Method
        #endregion

        #region Custom Method
        public void OnFocus()
        {
            if (actionUI != null && actionText != null)
            {
                actionText.SetText(InteractText);
                actionUI.SetActive(true);
            }
            //-CrossHair - 테두리 화면 출력
            if (extraCross != null) extraCross.SetActive(true);
        }

        public void OnLostFocus()
        {
            //-ActionKye,  ActionText 화면에서 안보이게 한다
            if (actionUI != null) actionUI.SetActive(false);
            //-CrossHair - 테두리 화면에서 안보이게 한다
            if (extraCross != null) extraCross.SetActive(false);
        }

        public void OnInteract(GameObject interactor)
        {
            ammoUI.SetActive(true);
            
            // 싱글플레이이므로 바로 SO의 총알 개수를 증가시킵니다.
            if (playerStats != null) playerStats.AmmoCount += 7;

            gameObject.SetActive(false);
        }
        #endregion
    }
}