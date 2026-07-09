using UnityEngine;

namespace MyFps
{
    public class RequirePuzzleCondition : MonoBehaviour, IInteractCondition
    {
        [SerializeField] private PlayerStatsData stats;
        [SerializeField] private PuzzlePieceType requiredPiece;

        public bool CanInteract(GameObject interactor)
        {
            if (stats == null) return false;

            switch (requiredPiece)
            {
                case PuzzlePieceType.LeftEye:
                    if (!stats.PuzzleLeftEye)
                    {
                        Debug.Log("You need the Left Puzzle Eye.");
                        return false;
                    }
                    break;
                case PuzzlePieceType.RightEye:
                    if (!stats.PuzzleRightEye)
                    {
                        Debug.Log("You need the Right Puzzle Eye.");
                        return false;
                    }
                    break;
            }
            return true;
        }
    }
}
