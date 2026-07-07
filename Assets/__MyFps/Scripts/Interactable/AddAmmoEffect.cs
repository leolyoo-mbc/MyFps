using MyFps;
using UnityEngine;

namespace MyFps
{
    public class AddAmmoEffect : MonoBehaviour, IInteractEffect
    {
        [SerializeField] private int amount = 7;

        public void ExecuteEffect(GameObject interactor)
        {
            // interactor가 카메라/하위 오브젝트일 수 있으므로 최상위 플레이어 오브젝트까지 탐색합니다.
            var ammo = interactor.GetComponentInParent<IAmmo>();
            if (ammo != null)
            {
                ammo.AmmoCount += amount;
            }
        }
    }
}