using UnityEngine;
using System;

namespace MyFps
{
    [RequireComponent(typeof(Animator))]
    public class SlidingDoorController : MonoBehaviour
    {
        #region Variables
        private Animator animator;
        private static readonly int IsOpenHash = Animator.StringToHash("IsOpen");

        [SerializeField] private float openSpeed = 2f;

        [SerializeField] private float currentOpenAmount = 0f;
        public float targetOpenAmount = 0f; // 외부에서 여닫을 수 있도록 Public
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Update()
        {
            if (animator == null) return;

            // 목표값(0 또는 1)을 향해 부드럽게 값을 변경
            if (Mathf.Abs(currentOpenAmount - targetOpenAmount) > 0.001f)
            {
                currentOpenAmount = Mathf.MoveTowards(currentOpenAmount, targetOpenAmount, Time.deltaTime * openSpeed);
                animator.SetFloat(IsOpenHash, currentOpenAmount);
            }
        }
        #endregion
    }
}