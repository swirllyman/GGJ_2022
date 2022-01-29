using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoal : MonoBehaviour
{
    bool goalReached = false;
    public void SetComplete() 
    {
        if (!goalReached) {
            goalReached = true;
            var managers = GameObject.FindGameObjectsWithTag("LevelManager");
            foreach (var manager in managers) {
                var levelManager = manager.GetComponent<LevelManager>();
                levelManager.SetGoalComplete("area reached");
            }
            Debug.Log("Goal reached");
        }
    }

    public void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.tag == "Player") {
            SetComplete();
        }
    }
}
