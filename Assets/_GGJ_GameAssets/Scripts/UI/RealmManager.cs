using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealmManager : MonoBehaviour
{
    public RealmConfig[] myRealms;
    public GameObject realmPrefab;
    public Transform realmsParent;

    MainMenu mainMenu;
    RealmUI[] realmUI;

    int currentRealm = 0;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu = GetComponent<MainMenu>();
        SetupRealms();
    }

    void SetupRealms()
    {
        //if (!PlayerPrefs.HasKey("RealmData"))
        //    PlayerPrefs.SetString("RealmData", JsonUtility.ToJson(myRealms));

        //string realmData = PlayerPrefs.GetString("RealmData");

        //myRealms = JsonUtility.FromJson<RealmConfig[]>(realmData);

        realmUI = new RealmUI[myRealms.Length];
        for (int i = 0; i < myRealms.Length; i++)
        {
            realmUI[i] = Instantiate(realmPrefab, realmsParent).GetComponent<RealmUI>();
            realmUI[i].SetupRealmUI(myRealms[i].realmData);

            for (int j = 0; j < myRealms[i].realmData.levels.Length; j++)
            {
                string levelName = myRealms[i].realmData.levels[j].levelData.internalSceneName;
                realmUI[i].levelButtons[j].button.onClick.AddListener(delegate { LevelClicked(levelName); });
            }
            realmUI[i].gameObject.SetActive(i == 0);
        }
    }

    void LevelClicked(string levelName)
    {
        Debug.Log("Level name CLicked: " + levelName);
        GameManager.singleton.PlaySceneLoad(levelName);
        mainMenu.SelectPanel(2);
    }

    public void ContinueGame()
    {
        GameManager.singleton.PlaySceneLoad("Tutorial");
        mainMenu.SelectPanel(2);
    }

    public void ChangeRealm(int dir)
    {
        currentRealm = Utils.RealMod(currentRealm + dir, myRealms.Length);
        SelectRealmUI(currentRealm);
    }

    public void SelectRealmUI(int realmPanel)
    {
        for (int i = 0; i < myRealms.Length; i++)
        {
            realmUI[i].gameObject.SetActive(i == realmPanel);
        }
    }

}
