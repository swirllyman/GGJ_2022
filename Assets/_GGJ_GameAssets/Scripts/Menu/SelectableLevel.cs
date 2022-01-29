using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableLevel : MonoBehaviour
{
    public LevelConfig config;

    Image levelThumbnailImage;
    Button levelThumbnailButton;
    TMPro.TMP_Text levelText;

    // Start is called before the first frame update
    void Start()
    {
        levelThumbnailImage = GetComponentInChildren<Image>();
        levelThumbnailButton = GetComponentInChildren<Button>();
        levelText = GetComponentInChildren<TMPro.TMP_Text>();

        levelThumbnailImage.sprite = config.levelSettings.levelThumbnail;
        levelThumbnailButton.interactable = config.levelSettings.unlocked;
        levelText.SetText(config.levelSettings.levelName);

        levelThumbnailButton.onClick.AddListener(delegate {
            var managers = GameObject.FindGameObjectsWithTag("LevelSelectManager");
            foreach (var manager in managers) {
                var levelManager = manager.GetComponent<LevelSelect>();
                levelManager.LoadLevel(config.levelSettings.internalSceneName);
            }
        });
    }
}
