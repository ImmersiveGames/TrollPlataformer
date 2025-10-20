using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private string levelLine;
    [SerializeField] private int baseScore = 5;
    [SerializeField] private Transform playerStartPosition;
    [SerializeField] private bool playerStartFlipped = false;
    
    public string LevelLine => levelLine;
    public int BaseScore => baseScore;
    public Transform PlayerStartPosition => playerStartPosition;
    public bool PlayerStartFlipped => playerStartFlipped;
}
