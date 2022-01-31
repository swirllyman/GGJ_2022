using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteCanvas : MonoBehaviour
{
    [SerializeField] GameObject levelList;
    [SerializeField] GameObject selectableLevelTemplate;

    public void LoadNextLevels(LevelConfig[] levels) {
        foreach (var level in levels) {
            var unlockedLevel = level;
            unlockedLevel.levelSettings.unlocked = true;

            var nextLevel = Instantiate(selectableLevelTemplate, levelList.transform);
            nextLevel.GetComponent<SelectableLevel>().config = unlockedLevel;
            nextLevel.GetComponent<SelectableLevel>().SetupLevel();
        }
    }
}
