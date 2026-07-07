using UnityEngine;

namespace MyFps
{
    /// <summary>
    /// 상태에 따라 동적으로 변하는 상호작용 텍스트를 제공하기 위한 인터페이스
    /// </summary>
    public interface IDynamicActionText
    {
        string GetActionText();
    }
}
