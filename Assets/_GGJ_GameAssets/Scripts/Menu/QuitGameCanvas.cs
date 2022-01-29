using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuitGameCanvas : MonoBehaviour
{
    public void QuitGame() {
        // TODO: Can set up scene transition for quitting.
        Debug.Log("Quit game");
        Quit();
    }

    private void Quit() {
        #if (UNITY_EDITOR)
            UnityEditor.EditorApplication.isPlaying = false;
        #elif (UNITY_STANDALONE) 
            Application.Quit();
        #elif (UNITY_WEBGL)
            SceneManager.LoadScene("MainMenu_v1");
        #endif
    }
}
