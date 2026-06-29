using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MyFps
{
    [RequireComponent(typeof(Collider))]
    public class CSecondTrigger : MonoBehaviour
    {
        #region Variables
        [SerializeField] private SlidingDoorController slidingDoor;
        [SerializeField] private CharacterInput playerInput;
        [SerializeField] private GameObject robot;
        // 중복 실행을 막기 위한 플래그
        private bool hasTriggered = false;
        #endregion

        #region Property
        #endregion

        #region Unity Event Method
        private void Start()
        {
            if ( robot != null) robot.SetActive(false);
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
            if (slidingDoor != null) slidingDoor.targetOpenAmount = 1;
            if (robot != null) robot.SetActive(true);
            yield return null;
        }
        #endregion
    }
}