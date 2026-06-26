#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public class ShowFullNameInProjectWindow
{
    static ShowFullNameInProjectWindow()
    {
        EditorApplication.projectWindowItemOnGUI += DrawFullName;
    }

    private static void DrawFullName(string guid, Rect selectionRect)
    {
        // List 뷰(세로폭이 16 정도)에서는 적용하지 않고, 아이콘 뷰(Grid 뷰)일 때만 적용합니다.
        if (selectionRect.height <= 20f) return;

        string path = AssetDatabase.GUIDToAssetPath(guid);
        if (string.IsNullOrEmpty(path)) return;

        // 에셋의 이름을 가져옵니다 (확장자 제외, 단 폴더인 경우는 그대로)
        string fileName = Path.GetFileNameWithoutExtension(path);
        if (AssetDatabase.IsValidFolder(path))
        {
            fileName = Path.GetFileName(path);
        }

        GUIStyle style = new GUIStyle(EditorStyles.miniLabel);
        style.alignment = TextAnchor.UpperCenter;
        style.wordWrap = true; // 핵심: 글자가 길면 줄바꿈을 해줍니다
        
        // 다크/라이트 모드에 따른 글자 색상 자동 맞춤
        style.normal.textColor = EditorGUIUtility.isProSkin ? new Color(0.9f, 0.9f, 0.9f) : Color.black;

        // 표시할 이름의 가로 폭(selectionRect.width)을 기준으로 필요한 세로 높이를 계산합니다.
        float textHeight = style.CalcHeight(new GUIContent(fileName), selectionRect.width);

        // 텍스트를 그릴 영역 설정 (해당 아이템 영역의 맨 아래쪽)
        Rect textRect = new Rect(selectionRect.x, selectionRect.yMax - textHeight, selectionRect.width, textHeight);

        // 원래 출력되어 있던 텍스트(잘린 텍스트)를 안 보이게 살짝 덮어줄 배경색 설정
        Color bgColor = EditorGUIUtility.isProSkin ? new Color(0.22f, 0.22f, 0.22f, 0.95f) : new Color(0.76f, 0.76f, 0.76f, 0.95f);
        EditorGUI.DrawRect(textRect, bgColor);

        // 자동 줄바꿈이 적용된 전체 텍스트 그리기
        GUI.Label(textRect, fileName, style);
    }
}
#endif
