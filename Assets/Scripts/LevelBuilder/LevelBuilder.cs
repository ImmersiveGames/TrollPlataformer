using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    public static LevelBuilder Instance;

    [Header("Configuração de Fases")]
    [SerializeField] private GameObject levelMenu;
    [SerializeField] private GameObject[] levelPrefabs;
    [SerializeField] private Transform levelHolder;

    private GameObject currentLevelInstance;
    private GameObject levelMenuInstance;
    private Level currentLevel;
    private Level menuLevel;

    public int CurrentLevelIndex { get; private set; }
    public int LastLevelIndex { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        LastLevelIndex = levelPrefabs.Length - 1;
        //Debug.Log("Last Level Index: " + LastLevelIndex);
        
        
        InstanciateLevelMenu();
        TooglelevelMenu(false);
    }

    private void InstanciateLevelMenu()
    {
        levelMenuInstance = Instantiate(levelMenu);
        menuLevel = levelMenuInstance.GetComponent<Level>();
    }

    public void LoadMenuLevel()
    {
        if (levelMenu == null)
        {
            Debug.LogError("Level Menu prefab is missing");
            return;
        }

        // Limpar fase anterior
        ClearScenary();
        GameEvents.CleanTrolls();
        
        if(levelMenuInstance ==null)
            InstanciateLevelMenu();
            
        TooglelevelMenu(true);
    }
    
    public void LoadLevel(int index)
    {
        if (index < 0 || index >= levelPrefabs.Length)
        {
            Debug.LogError("Índice de fase inválido.");
            return;
        }

        TooglelevelMenu(false);
        // Limpar fase anterior
        ClearScenary();
        GameEvents.CleanTrolls();

        CurrentLevelIndex = index;

        // Instanciar novo level
        currentLevelInstance = Instantiate(levelPrefabs[index], levelHolder);
        currentLevel = currentLevelInstance.GetComponent<Level>();

        if (currentLevel == null)
            Debug.LogError("Prefab da fase não possui componente Level.");
    }

    public Vector3 GetStartPoint()
    {
        return currentLevel?.PlayerStartPosition.position ?? Vector3.zero;
    }
    
    public Vector3 GetLevelMenuStartPoint()
    {
        return menuLevel?.PlayerStartPosition.position ?? Vector3.zero; 
            
    }

    public bool GetStartFlipped()
    {
        return currentLevel?.PlayerStartFlipped ?? false;
    }

    public string GetLevelName()
    {
        string number = (CurrentLevelIndex + 1).ToString();
        string line = currentLevel?.LevelLine ?? "";
        return $"Level {number} {line}";
    }

    public int GetBaseScore()
    {
        return currentLevel?.BaseScore ?? 0;
    }
    
    public void ReloadCurrentLevel()
    {
        LoadLevel(CurrentLevelIndex);
    }

    public void ClearScenary()
    {
        if (currentLevelInstance != null)
            Destroy(currentLevelInstance);
    }

    public void TooglelevelMenu(bool isActive)
    {
        levelMenuInstance.SetActive(isActive);
    }
}