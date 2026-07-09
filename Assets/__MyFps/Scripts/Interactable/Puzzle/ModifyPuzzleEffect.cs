using UnityEngine;

namespace MyFps
{
    public class ModifyPuzzleEffect : MonoBehaviour, IInteractEffect
    {
        [SerializeField] private PlayerStatsData stats;
        [SerializeField] private PuzzlePieceType pieceType;
        [SerializeField, Tooltip("true면 퍼즐 획득, false면 퍼즐 소모(사용)")] 
        private bool isAdding = true;

        public void ExecuteEffect(GameObject interactor)
        {
            if (stats == null) return;

            switch (pieceType)
            {
                case PuzzlePieceType.LeftEye:
                    stats.PuzzleLeftEye = isAdding;
                    Debug.Log(isAdding ? "Acquired Left Puzzle Eye!" : "Used Left Puzzle Eye.");
                    break;
                case PuzzlePieceType.RightEye:
                    stats.PuzzleRightEye = isAdding;
                    Debug.Log(isAdding ? "Acquired Right Puzzle Eye!" : "Used Right Puzzle Eye.");
                    break;
            }
        }
    }
}
