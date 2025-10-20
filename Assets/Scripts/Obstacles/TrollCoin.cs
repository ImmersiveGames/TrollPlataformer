using UnityEngine;

public class TrollCoin : MonoBehaviour
{
    [Header("Configuração de Trollagem")]
    [SerializeField] private float fleeSpeed = 5f;
    [SerializeField] private float fleeDuration = 2f;
    [SerializeField] private AudioClip laughClip;
    [SerializeField] private TrollPhraseSet phraseSet; // ScriptableObject com frases
    [SerializeField] private GameObject floatingTextPrefab;

    private Transform player;
    private AudioSource audioSource;
    private bool isFleeing = false;
    private float fleeTimer = 0f;
    
    void OnEnable()
    {
        GameEvents.OnCleanupTrolls += SelfDestruct;
    }

    void OnDisable()
    {
        GameEvents.OnCleanupTrolls -= SelfDestruct;
    }

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isFleeing && player != null)
        {
            fleeTimer += Time.deltaTime;
            if (fleeTimer > fleeDuration)
            {
                isFleeing = false;
                return;
            }

            Vector3 direction = (transform.position - player.position).normalized;
            transform.position += direction * fleeSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isFleeing) return;

        if (other.CompareTag("Player"))
        {
            player = other.transform;
            isFleeing = true;
            fleeTimer = 0f;

            if (laughClip != null && audioSource != null)
                audioSource.PlayOneShot(laughClip);
            
            ShowTrollPhrase();
        }
    }
    
    void ShowTrollPhrase()
    {
        if (floatingTextPrefab == null || phraseSet == null) return;

        string phrase = phraseSet.GetRandomPhrase();
        Vector3 spawnPos = transform.position + Vector3.up * 2f;

        GameObject textObj = Instantiate(floatingTextPrefab, spawnPos, Quaternion.identity);
        FloatingText floatingText = textObj.GetComponent<FloatingText>();
        if (floatingText != null)
            floatingText.Setup(phrase, Color.red);
    }
    
    private void SelfDestruct()
    {
        Debug.Log("Troll coin destroyed");
        Destroy(gameObject);
    }
}

