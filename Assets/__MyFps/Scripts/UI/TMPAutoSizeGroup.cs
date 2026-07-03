using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위함

public class TMPAutoSizeGroup : MonoBehaviour
{
    [Tooltip("크기를 똑같이 맞출 텍스트들을 전부 넣어주세요")]
    public TextMeshProUGUI[] texts;

    void Start()
    {
        SyncTextSizes();
    }

    // 인스펙터에서 컴포넌트 이름 우클릭 -> Sync Text Sizes 를 눌러 에디터에서도 바로 실행 가능합니다.
    [ContextMenu("Sync Text Sizes")]
    public void SyncTextSizes()
    {
        if (texts == null || texts.Length == 0) return;

        float minFontSize = float.MaxValue;

        // 1. 모든 텍스트의 AutoSize를 켜서 각자 공간에 맞는 최대/최적의 크기를 계산하게 합니다.
        foreach (var t in texts)
        {
            if (t == null) continue;
            
            t.enableAutoSizing = true;
            t.ForceMeshUpdate(); // 강제로 즉시 크기 계산을 실행
            
            // 계산된 크기 중 '가장 작게 쪼그라든 폰트 사이즈'를 찾습니다.
            // (가장 글자 수가 많은 버튼이 가장 폰트가 작을 것입니다)
            if (t.fontSize < minFontSize)
            {
                minFontSize = t.fontSize;
            }
        }

        // 2. 찾아낸 가장 작은 폰트 사이즈로 모든 텍스트의 크기를 통일합니다.
        foreach (var t in texts)
        {
            if (t == null) continue;
            
            t.enableAutoSizing = false; // 오토사이즈를 끄고
            t.fontSize = minFontSize;   // 고정 사이즈로 일괄 통일!
        }
    }
}
