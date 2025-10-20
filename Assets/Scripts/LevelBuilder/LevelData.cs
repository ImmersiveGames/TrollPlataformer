using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "TrollPlatformer/LevelData")]
public class LevelData : ScriptableObject
{
    public string levelLine;
    public int baseScore = 5;
    public GameObject levelPrefab;
    public Vector3 playerStartPosition;
}