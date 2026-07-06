using UnityEngine;

public class CCTVController : MonoBehaviour
{
    #region Variables
    [Header("CCTV Settings")]
    public Transform cctvHead;
    public float targetDistance = 2f;

    private InputSystem_Actions _inputActions;
    #endregion

    #region Unity Event Method
    private void Awake()
    {
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

    void Update()
    {
        if (cctvHead == null || Camera.main == null) return;

        // 뉴 인풋 시스템에서 마우스 화면 좌표 읽기
        Vector3 screenPosition = _inputActions.UI.Point.ReadValue<Vector2>();

        // 카메라에서 일정 거리 떨어진 월드 좌표로 변환
        screenPosition.z = targetDistance;
        Vector3 worldTarget = Camera.main.ScreenToWorldPoint(screenPosition);

        // CCTV 헤드가 해당 월드 좌표를 바라보도록 회전
        cctvHead.LookAt(worldTarget);
    }
    #endregion
}
