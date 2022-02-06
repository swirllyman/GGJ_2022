using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Aimer2D))]
public class PlayerGrabber : MonoBehaviour
{
    [SerializeField] Transform grabberTip;
    [SerializeField] Transform grabberHandsTransform;
    [SerializeField] float attachDistance = .7f;
    [SerializeField] float throwForce = 1.5f;
    [SerializeField] float minDistance = .2f;
    [SerializeField] float shotSpeed = .7f;
    [SerializeField] float shotDistanceCheck = .15f;
    [SerializeField] LayerMask collisionMask;
    [SerializeField] LineRenderer lineRend;
    [SerializeField] LineRenderer trajectoryRend;
    [SerializeField] LineRenderer preCastLineRend;
    [SerializeField] Animator myAnim;
    [SerializeField] AudioClip shootClip;
    [SerializeField] AudioClip attachClip;
    [SerializeField] AudioClip pullInClip;
    [SerializeField] AudioClip detachClip;
    [SerializeField] AudioClip throwClip;

    internal Grabbable attachedGrabbable;
    internal bool holding = false;
    AudioSource audioSource;
    Rigidbody2D mybody;
    Aimer2D aimer;
    Vector2 currentHitPos;
    RaycastHit2D hit;
    bool attached = false;
    bool justShot = false;
    bool canHit = false;

    float grabberCD = .05f;
    bool onCD = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        mybody = GetComponent<Rigidbody2D>();
        aimer = GetComponent<Aimer2D>();
        grabberTip.parent = null;
        lineRend.enabled = false;
        trajectoryRend.enabled = false;
        preCastLineRend.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        if (onCD)
        {
            grabberCD -= Time.deltaTime;
            if(grabberCD <= 0.0f)
            {
                grabberCD = .05f;
                onCD = false;
            }
        }
    }

    private void LateUpdate()
    {
        UpdateAim();
        if (justShot)
        {
            Vector3 moveDir = (hit.point - (Vector2)grabberTip.position).normalized * Time.deltaTime * shotSpeed;
            grabberTip.position += moveDir;
            lineRend.SetPosition(0, grabberHandsTransform.position);
            lineRend.SetPosition(1, grabberTip.position);
            if (Vector3.Distance(grabberTip.position, hit.point) < shotDistanceCheck)
            {
                AttachGrabber();
            }
        }

        if (attached)
        {
            if(attachedGrabbable == null)
            {
                DropAttachedGrabbable();
            }
            else
            {
                if (Vector3.Distance(grabberTip.position, transform.position) > minDistance)
                {
                    Vector3 moveDir = ((Vector2)transform.position - (Vector2)grabberTip.position).normalized * Time.deltaTime * shotSpeed;
                    grabberTip.position += moveDir;
                    attachedGrabbable.transform.position = grabberTip.position;
                    lineRend.SetPosition(0, grabberHandsTransform.position);
                    lineRend.SetPosition(1, grabberTip.position);
                }
                else
                {
                    HoldItem();
                }
            }
        }
        if (holding)
        {
            if (attachedGrabbable == null)
            {
                DropAttachedGrabbable();
            }
            else
            {
                lineRend.SetPosition(0, grabberHandsTransform.position);
                lineRend.SetPosition(1, grabberTip.position);
                Vector3[] trajectory = Plot(mybody, attachedGrabbable.transform.position, aimer.aimDirection * throwForce, 100);
                trajectoryRend.positionCount = trajectory.Length;
                for (int i = 0; i < trajectoryRend.positionCount; i++)
                {
                    trajectoryRend.SetPositions(trajectory);
                }
            }
        }
    }

    void UpdateAim()
    {
        if (!justShot)
        {
            hit = Physics2D.Raycast(transform.position, aimer.aimDirection, attachDistance, collisionMask);
            if (hit.collider != null && hit.collider.CompareTag("Grabbable"))
            {
                if (!attached)
                {
                    aimer.aimDistance = hit.distance;
                }

                canHit = !attached & !justShot & !onCD;
            }
            else
            {
                canHit = false;
                aimer.aimDistance = attachDistance;
            }
        }
        else
        {
            canHit = false;
        }

        if (canHit)
        {
            preCastLineRend.enabled = true;
            preCastLineRend.SetPosition(0, hit.point);
            preCastLineRend.SetPosition(1, transform.position);
        }
        else
        {
            preCastLineRend.enabled = false;
        }
    }

    void CheckInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (attached & !justShot)
            {
                DropAttachedGrabbable();
            }
            else if (justShot)
            {
                StopShot();
            }
        }
        else if (Input.GetMouseButton(1) &! onCD)
        {
            if (hit.collider != null && canHit)
            {
                ShootGrabber();
            }
        }

        //if (Input.GetMouseButtonUp(1))
        //{
        //    DropAttachedGrabbable();
        //}
    }

    void StopShot()
    {
        justShot = false;
        lineRend.enabled = false;
    }

    void HoldItem()
    {
        if (attachedGrabbable == null)
        {
            DropAttachedGrabbable();
        }
        else
        {
            trajectoryRend.enabled = true;
            holding = true;
        }
    }

    internal void DropAttachedGrabbable()
    {
        if (attached)
        {
            attached = false;
            trajectoryRend.enabled = false;
            lineRend.enabled = false;
            justShot = false;
            onCD = true;
            if (attachedGrabbable != null)
            {
                attachedGrabbable.transform.SetParent(null);

                if (holding)
                {
                    //Debug.Log("Throwing");
                    justShot = true;
                    attachedGrabbable.Throw(aimer.aimDirection, throwForce);
                    audioSource.PlayOneShot(throwClip);
                }
                else
                {
                    attachedGrabbable.Drop();
                    audioSource.PlayOneShot(detachClip);
                }
            }
            holding = false;
            attachedGrabbable = null;
        }
    }

    void ShootGrabber()
    {
        lineRend.enabled = true;
        grabberTip.position = transform.position;
        justShot = true;
        onCD = true;
        audioSource.PlayOneShot(shootClip);
    }

    void AttachGrabber()
    {
        //Debug.Log("Attached");
        attached = true;
        justShot = false;

        grabberTip.position = hit.point;
        lineRend.SetPosition(0, grabberHandsTransform.position);
        lineRend.SetPosition(1, grabberTip.position);

        if (hit.collider == null)
        {
            DropAttachedGrabbable();
        }
        else
        {
            var pullableObject = hit.collider.GetComponentInParent(typeof(GrabPlatform)) as GrabPlatform;
            if (pullableObject != null) {
                pullableObject.OnActivate();
            } 
            else 
            {
                attachedGrabbable = hit.collider.GetComponent<Grabbable>();
                if (attachedGrabbable != null)
                {
                    attachedGrabbable.transform.SetParent(grabberTip);
                    attachedGrabbable.transform.localPosition = Vector3.zero;
                    attachedGrabbable.PickUp();
                    audioSource.PlayOneShot(attachClip);
                    audioSource.PlayOneShot(pullInClip);
                }
            }
        }
    }

    public Vector3[] Plot(Rigidbody2D body, Vector2 pos, Vector2 vel, int steps)
    {
        List<Vector3> results = new List<Vector3>();
        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations;
        Vector2 gravityAccel = Physics2D.gravity * body.gravityScale * timestep * timestep;
        float drag = 1f - timestep * body.drag;
        Vector2 moveStep = vel * timestep;

        for (int i = 0; i < steps; i++)
        {
            moveStep += gravityAccel;
            moveStep *= drag;
            pos += moveStep;
            if(i % 3 == 0)
                results.Add(pos);
        }
        return results.ToArray();
    }
}
