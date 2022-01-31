using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    const string PAUSE_SCENE = "pause_additive";

    [SerializeField] GameObject levelCompleteCanvas;
    [SerializeField] LevelConfig[] nextLevels;
    bool isSceneLoaded {
        get {
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == PAUSE_SCENE && scene.isLoaded) {
                    return true;
                }
            }
            return false;
        }
    }
    // Contain details about level info
    // Start is called before the first frame update
    void Start()
    {
        SetupPause();
    }

    public void SetupPause() {
        if (!isSceneLoaded) {
            Debug.Log(PAUSE_SCENE);
            SceneManager.LoadScene(PAUSE_SCENE, LoadSceneMode.Additive);
        }
    }

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CompleteLevel() {
        var nextLevelCanvas = Instantiate(levelCompleteCanvas);
        nextLevelCanvas.GetComponent<LevelCompleteCanvas>().LoadNextLevels(nextLevels);
    }

    public void SetGoalComplete(string goalId) {
        CompleteLevel();
    }
}
