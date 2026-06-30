using UnityEngine;
using System;

namespace MyFps
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(AudioSource))]
    public class SlidingDoorController : MonoBehaviour
    {
        #region Variables
        private Animator animator;
        private static readonly int IsOpenHash = Animator.StringToHash("IsOpen");

        [SerializeField] private float openSpeed = 2f;

        [SerializeField] private float currentOpenAmount = 0f;

        private AudioSource audioSource;

        [SerializeField] private AudioClip slidingDoorSound;

        private float targetOpenAmout = 0f;
        #endregion

        #region Property
        public float TargetOpenAmount
        {
            get { return targetOpenAmout; }
            set
            {
                if (value != TargetOpenAmount)
                {
                    targetOpenAmout = value;
                    if (audioSource != null && slidingDoorSound != null) audioSource.PlayOneShot(slidingDoorSound);
                }
            }
        } // 외부에서 여닫을 수 있도록 Public
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (animator == null) return;

            // 목표값(0 또는 1)을 향해 부드럽게 값을 변경
            if (Mathf.Abs(currentOpenAmount - TargetOpenAmount) > 0.001f)
            {
                currentOpenAmount = Mathf.MoveTowards(currentOpenAmount, TargetOpenAmount, Time.deltaTime * openSpeed);
                animator.SetFloat(IsOpenHash, currentOpenAmount);
            }
        }
        #endregion
    }
}