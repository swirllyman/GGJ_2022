using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseQuitChecker : MonoBehaviour
{
    bool paused = false;
    public GameObject pauseCanvasObject;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape")) {
            if (paused) {
                Unpause();
            }
            else {
                Pause();
            }
        }
    }

    public void Pause() {
        paused = true;
        Time.timeScale = 0;
        pauseCanvasObject.SetActive(true);
    }

    public void Unpause() {
        paused = false;
        Time.timeScale = 1;
        pauseCanvasObject.SetActive(false);
    }
}
