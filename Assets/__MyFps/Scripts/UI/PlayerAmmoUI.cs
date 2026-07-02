using TMPro;
using UnityEngine;

namespace MyFps
{
    public class PlayerAmmoUI : MonoBehaviour
    {
        #region Variablels
        [Tooltip("연결할 데이터 SO")]
        [SerializeField] private PlayerStatsData statData;

        [Tooltip("표시할 텍스트 컴포넌트")]
        [SerializeField] private TMP_Text ammoText;
        #endregion


        #region Unity Event Method
        private void OnEnable()
        {
            if (statData != null)
            {
                // 이벤트 구독
                statData.OnAmmoChanged += UpdateUI;
                // 켜질 때 초기 1회 업데이트
                UpdateUI(statData.AmmoCount);
            }
        }

        private void OnDisable()
        {
            if (statData != null)
            {
                // 이벤트 구독 해제 (메모리 누수 방지)
                statData.OnAmmoChanged -= UpdateUI;
            }
        }
        #endregion

        #region Custom Method
        private void UpdateUI(int currentAmmo)
        {
            if (ammoText != null)
            {
                ammoText.text = currentAmmo.ToString();
            }
        }
        #endregion
    }
}
