using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentEffectsManager : MonoBehaviour
{
    public static PersistentEffectsManager Instance;
    [SerializeField] private MusicManager musicManager;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        AudioProperties.Init();

        SceneManager.sceneLoaded += ReactToSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= ReactToSceneLoaded;
    }

    private void ReactToSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        musicManager.AdjustStateToScene(scene);
    }
    
    public void PlayGameplayMusic() => musicManager.PlayGameplayMusic();
    public void PlayGameOverSoundEffects() => musicManager.PlayGameOverSoundEffects();
}
