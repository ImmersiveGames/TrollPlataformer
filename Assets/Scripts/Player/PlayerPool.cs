using UnityEngine;

public class PlayerPool : MonoBehaviour
{
    [Header("Referência do prefab")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerWaitPosition;

    private PlayerController playerInstance;

    public PlayerController Player => playerInstance;
    
    private PlayerAnimation animations;

    private void Awake()
    {
        if (animations == null && Player != null)
            animations = Player.GetComponent<PlayerAnimation>();
    }

    private void Start()
    {
        InitializePlayer();
        StorePlayer();
    }
    
    private void OnEnable()
    {
        GameEvents.OnStartGame += EnablePlayerControl;
        GameEvents.OnPlayerDie += OnPlayerDie;
        GameEvents.OnLevelComplete += DisablePlayerControl;
        GameEvents.OnPlayerRespawn += OnGameEventsOnPlayerRespawn;
    }

    private void OnDisable()
    {
        GameEvents.OnStartGame -= EnablePlayerControl;
        GameEvents.OnPlayerDie -= OnPlayerDie;
        GameEvents.OnLevelComplete -= DisablePlayerControl;
        GameEvents.OnPlayerRespawn -= OnGameEventsOnPlayerRespawn;
    }

    public void InitializePlayer()
    {
        if (playerInstance == null)
        {
            GameObject obj = Instantiate(playerPrefab);
            playerInstance = obj.GetComponent<PlayerController>();
            if (animations == null)
                animations = Player.GetComponent<PlayerAnimation>();

            if (playerInstance == null)
                Debug.LogError("Player prefab não possui PlayerController.");
        }
    }

    private void ResetPlayerStartPosition(Vector3 position)
    {
        if (Player == null)
        {
            //Debug.LogWarning("Player ainda não foi instanciado. Chamando InitializePlayer.");
            InitializePlayer();
        }

        Player.TeleportTo(position);
        Player.Flip(LevelBuilder.Instance.GetStartFlipped());
    }

    public void EnablePlayerControl()
    {
        if (Player != null)
            Player.ToggleCharacterController(true);
    }
    
    public void DisablePlayerControl()
    {
        if (Player != null)
            Player.ToggleCharacterController(false);
    }

    public void OnPlayerDie()
    {
        Debug.Log("Morri");
        //playerGPX.SetActive(false);
        Player.ToggleCharacterController(false);
        animations.MovementAnimation(0);
        animations.PlayDeathNormal();
        
    }

    public void OnGameEventsOnPlayerRespawn()
    {
        Vector3 startPoint = LevelBuilder.Instance.GetStartPoint();
        //Debug.Log(startPoint);
        ResetPlayerStartPosition(startPoint);
        animations.Respawn();
    }

    public void StorePlayer()
    {
        if (Player != null)
        {
            Player.TeleportTo(playerWaitPosition.position);
            DisablePlayerControl();
        }
    }
}