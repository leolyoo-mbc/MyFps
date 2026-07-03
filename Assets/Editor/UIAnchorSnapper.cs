using UnityEditor;
using UnityEngine;

public class UIAnchorSnapper
{
    // 단축키: Ctrl + ] (맥은 Cmd + ])
    [MenuItem("GameObject/UI/현재 위치로 앵커 맞추기 (Snap Anchors) %]")]
    [MenuItem("CONTEXT/RectTransform/현재 위치로 앵커 맞추기")]
    static void SnapAnchors()
    {
        // 선택된 모든 UI 오브젝트에 대해 실행
        foreach (GameObject go in Selection.gameObjects)
        {
            RectTransform rect = go.GetComponent<RectTransform>();
            if (rect == null || rect.parent == null) continue;

            // Ctrl+Z(실행 취소)가 가능하도록 기록을 남김
            Undo.RecordObject(rect, "Snap Anchors");
            
            RectTransform parent = rect.parent as RectTransform;
            
            // 현재 위치를 비율로 계산하여 앵커 위치를 새로 설정
            Vector2 newAnchorsMin = new Vector2(
                rect.anchorMin.x + rect.offsetMin.x / parent.rect.width,
                rect.anchorMin.y + rect.offsetMin.y / parent.rect.height);
            
            Vector2 newAnchorsMax = new Vector2(
                rect.anchorMax.x + rect.offsetMax.x / parent.rect.width,
                rect.anchorMax.y + rect.offsetMax.y / parent.rect.height);
            
            rect.anchorMin = newAnchorsMin;
            rect.anchorMax = newAnchorsMax;
            
            // 앵커가 모서리에 맞았으므로 Offset(여백)은 0으로 초기화
            rect.offsetMin = rect.offsetMax = Vector2.zero;
        }
    }
}
