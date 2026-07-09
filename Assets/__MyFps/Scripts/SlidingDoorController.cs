using UnityEngine;

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

        private AudioSource slidingDoorSound;

        [SerializeField] private float targetOpenAmount = 0f;
        #endregion

        #region Property
        //public float TargetOpenAmount
        //{
        //    get { return targetOpenAmount; }
        //    set
        //    {
        //        if (value != TargetOpenAmount)
        //        {
        //            targetOpenAmount = value;
        //            slidingDoorSound.Play();
        //        }
        //    }
        //} // 외부에서 여닫을 수 있도록 Public

        public bool Open
        {
            get
            {
                if (targetOpenAmount > 0) { return true; }
                else { return false; }
            }
            set
            {
                targetOpenAmount = value ? 1f : 0f;
                slidingDoorSound.Play();
            }
        }
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            animator = GetComponent<Animator>();
            slidingDoorSound = GetComponent<AudioSource>();
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