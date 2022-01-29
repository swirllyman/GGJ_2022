using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelConfig", 
    menuName = "ScriptableObjects/LevelConfig", 
    order = 3)]
public class LevelConfig : ScriptableObject
{
    [SerializeField]
    public LevelData levelSettings;

}

[System.Serializable]
public struct LevelData
{
    [Tooltip("Name of the level.")]
    public string levelName;
    
    [Tooltip("The scene that's loaded when the level is played.")]
    public string internalSceneName;

    [Tooltip("Thumbnail for level select.")]
    public Sprite levelThumbnail;

    [Tooltip("Indicates if level is unlocked.")]
    public bool unlocked;
}
