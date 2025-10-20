using UnityEngine;

[CreateAssetMenu(fileName = "TrollPhraseSet", menuName = "TrollPlatformer/TrollPhraseSet")]
public class TrollPhraseSet : ScriptableObject
{
    public string[] phrases;

    public string GetRandomPhrase()
    {
        if (phrases == null || phrases.Length == 0) return "";
        return phrases[Random.Range(0, phrases.Length)];
    }
}