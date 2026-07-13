using UnityEngine;

namespace MySample
{
    public class MovingWall : MonoBehaviour
    {
        #region Variables
        public float moveSpeed = 2f;
        public float moveDistance = 3f;
        public bool reverseDirection = false;
        private Vector3 startPosition;
        #endregion

        #region Unity Event Method
        private void Awake()
        {
            startPosition = transform.position;
        }

        private void Update()
        {
            float directionMultiplier = reverseDirection ? -1f : 1f;
            float offset = Mathf.Sin(Time.time * moveSpeed) * moveDistance * directionMultiplier;
            transform.position = startPosition + transform.right * offset;
        }
        #endregion

        #region Custom Method
        #endregion
    }
}