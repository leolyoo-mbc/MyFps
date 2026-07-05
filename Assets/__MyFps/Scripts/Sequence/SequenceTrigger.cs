using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class SequenceTrigger : MonoBehaviour
{
    [Tooltip("부딪힐 대상의 태그")]
    public string targetTag = "Player";


    [Header("순차적으로 대기하며 실행될 시퀀스")]
    public List<SequenceStep> sequenceSteps = new();

    private Collider triggerCollider;

    void Awake()
    {
        triggerCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 부딪힌 대상의 태그가 타겟 태그와 같으면 실행
        if (other.CompareTag(targetTag))
        {
            // 2. 시퀀스 목록이 하나라도 있다면 코루틴 시작
            if (sequenceSteps.Count > 0)
            {
                StartCoroutine(PlaySequenceRoutine());
                triggerCollider.enabled = false;
            }
        }
    }

    // 코루틴: 시간의 흐름(대기)을 제어할 수 있는 특수한 함수
    private IEnumerator PlaySequenceRoutine()
    {
        // 인스펙터에서 추가한 리스트를 순서대로 하나씩 꺼내옵니다.
        foreach (var step in sequenceSteps)
        {
            // 설정한 대기 시간이 0보다 크면 그 시간만큼 대기합니다.
            if (step.waitTime > 0f)
            {
                yield return new WaitForSeconds(step.waitTime);
            }

            // 대기가 끝나면 해당 스텝의 이벤트를 실행합니다.
            step.stepEvent.Invoke();
        }
    }
}

// 인스펙터에서 리스트 형태로 보기 위해 [System.Serializable]을 꼭 붙여야 합니다.
[System.Serializable]
public class SequenceStep
{
    [Tooltip("이 이벤트를 실행하기 전 대기할 시간 (초)")]
    public float waitTime = 0f;

    public UnityEvent stepEvent;
}