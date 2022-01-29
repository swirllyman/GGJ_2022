using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    const string PAUSE_SCENE = "pause_additive";
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

    public void CompleteLevel() {
        // Mark level as completed in save
        // Unlock next level
        // Display option to go to next level
        // Possible stretch goal to connect levels and indicate save at side of screen
        Debug.Log("Level complete");
    }

    public void SetGoalComplete(string goalId) {
        CompleteLevel();
    }
}
