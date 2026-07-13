using UnityEngine;
using UnityEngine.AI;
namespace MySample
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class AgentController : MonoBehaviour
    {
        #region Variables
        [SerializeField] private LayerMask _groundLayer;
        private NavMeshAgent _agent;
        private InputSystem_Actions _inputActions;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _inputActions = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            _inputActions.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Disable();
        }

        private void Update()
        {
            if (Camera.main == null) return;

            if (_inputActions.UI.Click.WasPressedThisFrame())
            {
                Vector3 screenPosition = _inputActions.UI.Point.ReadValue<Vector2>();
                Ray ray = Camera.main.ScreenPointToRay(screenPosition);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
                {
                    _agent.SetDestination(hit.point);
                }
            }
        }
        #endregion

        #region Custom Method

        #endregion
    }
}