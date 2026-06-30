using TMPro;
using UnityEngine;

namespace MyFps
{
    public class PlayerStats : MonoBehaviour
    {
        #region Variables
        [SerializeField] private TMP_Text ammoText;
        private int ammoCount = 0;
        #endregion

        #region Property
        public int AmmoCount
        {
            get
            {
                return ammoCount;
            }
            set
            {
                if (value != ammoCount)
                {
                    ammoCount = value;
                    if (ammoText != null) ammoText.text = value.ToString();
                }
            }
        }
        #endregion

        #region Unity Event Method
        void Awake()
        {
            AmmoCount = 0;
        }
        #endregion
    }
}