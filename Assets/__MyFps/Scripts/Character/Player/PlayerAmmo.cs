using UnityEngine;

namespace MyFps
{
    public class PlayerAmmo : MonoBehaviour, IAmmo
    {
        [SerializeField] private PlayerStatsData data;

        public int AmmoCount { get => data.AmmoCount; set => data.AmmoCount = value; }
    }
}