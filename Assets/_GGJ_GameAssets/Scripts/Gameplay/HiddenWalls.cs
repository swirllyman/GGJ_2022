using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenWalls : MonoBehaviour
{
    public GameObject wallExplosionObject;
    public GameObject hiddenVisuals;

    BoxCollider2D myCollider;

    private void Awake()
    {
        myCollider = GetComponent<BoxCollider2D>();
    }

    public void ExplodeWall()
    {
        myCollider.enabled = false;
        wallExplosionObject.SetActive(true);
        StartCoroutine(WallExplosionRoutine());
    }

    IEnumerator WallExplosionRoutine()
    {
        yield return new WaitForSeconds(.15f);
        hiddenVisuals.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        wallExplosionObject.SetActive(false);

    }
}
