using System.Collections;
using UnityEngine;

public class LevelGoal : MonoBehaviour
{
    public LevelConfig currentLevel;
    public LevelConfig nextLevelConfig;
    public string levelToLoadString;
    bool goalReached = false;

    int specialCount = 1;
    float timer = 0.0f;

    private void Start()
    {
        specialCount = FindObjectsOfType<HiddenWalls>().Length;
        timer = 0.0f;
    }

    void Update()
    {
        if(!goalReached)
            timer += Time.deltaTime;
    }

    public virtual void SetComplete(PlayerLevelStats stats) 
    {
        if (!goalReached) 
        {
            goalReached = true;
            if (currentLevel.levelData.fastestTime > timer || currentLevel.levelData.fastestTime == 0)
            {
                currentLevel.levelData.fastestTime = timer;
            }

            float  completionPerc = (float)stats.currentSpecialCount / specialCount;
            if (currentLevel.levelData.percentCompleted < completionPerc || currentLevel.levelData.percentCompleted == 0)
            {
                currentLevel.levelData.percentCompleted = completionPerc;
            }

            StartCoroutine(PlayEndSequence());
            if(nextLevelConfig != null)
                nextLevelConfig.levelData.unlocked = true;
        }
    }

    IEnumerator PlayEndSequence()
    {
        yield return new WaitForSeconds(.1f);
        GameManager.singleton.PlaySceneLoad(levelToLoadString);
    }

    public void OnTriggerEnter2D(Collider2D collider) 
    {
        if (collider.gameObject.tag == "Player") 
        {
            SetComplete(collider.GetComponent<PlayerLevelStats>());
        }
    }
}
