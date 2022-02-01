using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig",
    menuName = "ScriptableObjects/RealmConfig",
    order = 4)]
public class RealmConfig : ScriptableObject
{
    [SerializeField]
    public RealmData realmData;
}
[System.Serializable]
public struct RealmData
{
    [Tooltip("Name of the level.")]
    public string realmName;

    [Tooltip("Indicates if realm is unlocked.")]
    public bool unlocked;

    public LevelConfig[] levels;

    internal float percentCompleted;
}
