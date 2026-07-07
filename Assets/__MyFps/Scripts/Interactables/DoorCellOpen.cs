using System.Collections;
using TMPro;
using UnityEngine;

namespace MyFps
{
    [RequireComponent(typeof(Animator))]
    /// <summary>
    /// 플레이어와 인터렉티브 기능 구현
    /// 가까이 가서 마우스 가져가면 액션 UI 보여준다
    /// 액션: 문을 연다
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class DoorCellOpen : MonoBehaviour, IInteractable
    {


        #region Variables
        private Animator animator;
        private AudioSource audioSource;
        [SerializeField] private GameObject actionUI;
        [SerializeField] private TMP_Text actionText;

        [SerializeField] private Collider doorCollider;

        [SerializeField] private GameObject extraCross;

        private bool isOpened;

        private static readonly int IsOpenHash = Animator.StringToHash("IsOpen");

        [SerializeField] private string InteractTextWhenClosed = "OPEN THE DOOR";
        [SerializeField] private string InteractTextWhenOpened = "CLOSE THE DOOR";

        string IInteractable.ActionText => isOpened ? InteractTextWhenOpened : InteractTextWhenClosed;
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
        #endregion

        #region Custom Method
        //public void OnFocus()
        //{
        //    if (extraCross != null) extraCross.SetActive(true);
        //    if (actionUI != null && actionText != null)
        //    {
        //        actionText.SetText(isOpened ? InteractTextWhenOpened : InteractTextWhenClosed);
        //        actionUI.SetActive(true);
        //    }
        //}

        public void OnInteract(GameObject interactor)
        {
            if (doorCollider != null) doorCollider.enabled = false;
            isOpened = !isOpened;
            animator.SetBool(IsOpenHash, isOpened);
            if (audioSource != null) audioSource.Play();
            StartCoroutine(EnableColliderAfterAnimation());
        }

        private IEnumerator EnableColliderAfterAnimation()
        {
            yield return null; // Wait for animator to start transition

            while (animator.IsInTransition(0))
            {
                yield return null;
            }

            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }

            if (doorCollider != null) doorCollider.enabled = true;
        }

        //public void OnLostFocus()
        //{
        //    if (extraCross != null) extraCross.SetActive(false);
        //    if (actionUI != null) actionUI.SetActive(false);
        //}
        #endregion
    }
}