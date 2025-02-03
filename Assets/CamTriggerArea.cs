using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(Collider2D))]
public class CamTriggerArea : MonoBehaviour
{
    public CinemachineCamera vCam;

    Coroutine observeRoutine;

    void OnTriggerEnter2D(Collider2D collider)
    {
        Bomb b = collider.GetComponent<Bomb>();
        if (b != null)
        {
            vCam.enabled = true;

            b.onExplode += ResetCam;
        }
    }

    void ResetCam(Bomb b)
    {
        b.onExplode -= ResetCam;
        if (observeRoutine != null) StopCoroutine(observeRoutine);
        observeRoutine = StartCoroutine(ResetCamAfterTime());
    }

    IEnumerator ResetCamAfterTime()
    {
        yield return new WaitForSeconds(1.0f);
        vCam.enabled = false;
    }
}
