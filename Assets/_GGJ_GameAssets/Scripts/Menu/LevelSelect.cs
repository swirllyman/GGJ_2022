using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    // TODO have level assets

    public void LoadLevel(string levelName) {
        SceneManager.LoadScene(levelName);
    }
}
