using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 플레이어와 인터렉티브 기능 구현
    /// 가까이 가서 마우스 가져가면 액션 UI 보여준다
    /// 액션: 문을 연다
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class DoorCellOpen : MonoBehaviour, IInteractable
    {

        #region Variables
        private Animator animator;

        [SerializeField] private GameObject actionUI;

        [SerializeField] private Collider doorCollider;

        [SerializeField] private GameObject extraCross;

        private bool isFocused = false;

        private bool isOpened;

        private static readonly int IsOpenHash = Animator.StringToHash("IsOpen");

        private AudioSource audioSource;
        #endregion

        #region Property
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            animator = GetComponent<Animator>();
            isOpened = animator.GetBool(IsOpenHash);
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (actionUI != null && actionUI.activeSelf != isFocused) actionUI.SetActive(isFocused);
        }
        #endregion

        #region Custom Method
        public void OnFocus()
        {
            if (extraCross != null) extraCross.SetActive(true);
            isFocused = true;
        }

        public void OnInteract()
        {
            isOpened = !isOpened;
            animator.SetBool(IsOpenHash, isOpened);
            if (audioSource != null) audioSource.Play();
        }

        public void OnLostFocus()
        {
            if (extraCross != null) extraCross.SetActive(false);
            isFocused = false;
        }
        #endregion
    }
}