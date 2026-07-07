using UnityEngine;

namespace MyFps
{
    public class FloatingItem : MonoBehaviour
    {
        #region Variables
        [SerializeField] private float floatAmplitude = 0.1f;
        [SerializeField] private float floatFrequency = 3f;

        private Vector3 startPos;
        #endregion

        private void Start()
        {
            startPos = transform.localPosition;
        }

        private void Update()
        {
            transform.localPosition = startPos + new Vector3(0f, Mathf.Sin(Time.time * floatFrequency) * floatAmplitude, 0f);
        }
    }
}