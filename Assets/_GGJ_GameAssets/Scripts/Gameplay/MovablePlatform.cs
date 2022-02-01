using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovablePlatform : GrabPlatform
{
    [SerializeField] LineRenderer lineRend;
    public Transform startingPosition;
    public Transform endPosition;

    public GameObject platform;

    internal bool inMotion = false;

    AudioSource myAudioSource;
    float lineLocation;
    [SerializeField] float moveVelocity;
    float destination;
    float distance;


    // A platform that moves by being grappled.
    void Start()
    {
        myAudioSource = GetComponent<AudioSource>();
        platform.transform.position = startingPosition.position;
        lineLocation = 0;
        destination = lineLocation;
        distance = Vector3.Distance(startingPosition.position, endPosition.position);

        lineRend.SetPosition(0, startingPosition.position);
        lineRend.SetPosition(1, endPosition.position);
    }

    // Update is called once per frame
    void Update()
    {
        // Draw line between start and end position.
        var lerpDelta = (moveVelocity / distance);
        if (lineLocation > destination) {
            //Debug.Log(lineLocation);
            //Debug.Log(destination);
            lineLocation -= lerpDelta * Time.deltaTime;
        }
        else if (lineLocation < destination) {
            //Debug.Log(lineLocation);
            //Debug.Log(destination);
            lineLocation += lerpDelta * Time.deltaTime;
        }
        platform.transform.position = Vector3.Lerp(startingPosition.position, endPosition.position, lineLocation);

        // Clamp location to beginning and end.
        if (lineLocation <= 0)
        {
            lineLocation = 0;
            platform.transform.position = startingPosition.position;
            inMotion = false;
        }
        else if (lineLocation >= 1)
        {
            lineLocation = 1;
            platform.transform.position = endPosition.position;
            inMotion = false;
        }
        else
            inMotion = true;
    }

    public override void OnActivate()
    {
        myAudioSource.Play();
        if (lineLocation <= 0) {
            destination = 1;
        }
        else if (lineLocation >= 1) {
            destination = 0;
        }
    }
}
