using UnityEngine;

namespace MyFps
{
    [RequireComponent(typeof(CharacterInput))]
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMove : MonoBehaviour
    {
        #region Variables
        private CharacterInput input;
        private CharacterController controller;

        [Header("Player")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float sprintSpeed = 7f;
        private float moveSpeed;

        #endregion

        #region Unity Event Method
        private void Awake()
        {
            input = GetComponent<CharacterInput>();
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Move();
        }
        #endregion

        #region Custom Method
        private void Move()
        {
            moveSpeed = walkSpeed;

            if (input.Move == Vector2.zero) moveSpeed = 0f;

            Vector3 inputDirection = Vector3.zero;

            if (input.Move != Vector2.zero)
            {
                inputDirection = transform.right * input.Move.x + transform.forward * input.Move.y;

            }

            controller.Move(moveSpeed * Time.deltaTime * inputDirection.normalized);
        }
        #endregion
    }
}