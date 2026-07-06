using UnityEngine;

namespace MySample
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMove : MonoBehaviour
    {
        #region Variables
        private InputSystem_Actions input;
        private Rigidbody rb;
        [SerializeField] private float forwardForce = 5f;
        [SerializeField] private float sideForce = 5f;
        [SerializeField] private Vector2 moveIntent = Vector2.zero;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            input = new InputSystem_Actions();
            rb = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            input.Enable();
        }

        private void OnDisable()
        {
            input.Disable();
        }

        private void Update()
        {
            moveIntent = input.Player.Move.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {

            rb.AddForce(moveIntent.x * sideForce, 0f, forwardForce, ForceMode.Acceleration);
        }
        #endregion
    }
}