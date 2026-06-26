using TMPro;
using UnityEngine;

namespace MyFps
{
    public class PistolPickup : MonoBehaviour, IInteractable
    {
        #region Variables
        [SerializeField] private GameObject actionUI;
        [SerializeField] private TMP_Text actionText;
        [SerializeField] private string InteractText = "PICK UP PISTOL";

        [SerializeField] private GameObject extraCross;

        [SerializeField] private GameObject pistolInPlayerHand;
        [SerializeField] private GameObject guideArrow;
        #endregion

        #region Unity Event Method

        #endregion


        #region Custom Method
        public void OnFocus()
        {
            //-ActionKye("[ E ]")  ActionText("Pick Up Pistol") 화면 출력
            if (actionUI != null && actionText != null)
            {
                actionText.SetText(InteractText);
                actionUI.SetActive(true);
            }
            //-CrossHair - 테두리 화면 출력
            if (extraCross != null) extraCross.SetActive(true);
        }

        public void OnInteract()
        {
            //-테이블 위의 총은 없어지고 - 비활성화
            gameObject.SetActive(false);
            //- 오른손 쪽의 총은 화면 출력 -활성화
            if (pistolInPlayerHand != null) pistolInPlayerHand.SetActive(true);
            //- 책상위의 가이드 화살표는 없어진다
            if (guideArrow != null) guideArrow.SetActive(false);
        }

        public void OnLostFocus()
        {
            //-ActionKye,  ActionText 화면에서 안보이게 한다
            if (actionUI != null) actionUI.SetActive(false);
            //-CrossHair - 테두리 화면에서 안보이게 한다
            if (extraCross != null) extraCross.SetActive(false);

        }
        #endregion
    }
}