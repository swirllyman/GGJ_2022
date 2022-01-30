using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guide : MonoBehaviour
{
    public ParticleSystem disappaerParticles;
    public ParticleSystem appearParticles;
    public SpriteRenderer myRend;

    public float disappearTime, appearTime;

    Coroutine currentRoutine;
    
    [ContextMenu("Play Guide diappear Effect")]
    public void PlayDisappear()
    {
        disappaerParticles.Play();
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(DisappearRoutine());
    }

    IEnumerator DisappearRoutine()
    {
        yield return new WaitForSeconds(disappearTime);
        myRend.enabled = false;
    }

    [ContextMenu("Play Guide appear Effect")]
    public void PlayAppear()
    {
        appearParticles.Play();
        if (currentRoutine != null) StopCoroutine(currentRoutine);
        currentRoutine = StartCoroutine(AppearRoutine());
    }

    IEnumerator AppearRoutine()
    {
        yield return new WaitForSeconds(appearTime);
        myRend.enabled = true;
    }
}
