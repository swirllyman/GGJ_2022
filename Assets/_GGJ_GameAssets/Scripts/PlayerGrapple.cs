using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrapple : MonoBehaviour
{
    [SerializeField] DistanceJoint2D myGrappleJoint;
    [SerializeField] float attachDistance = .2f;
    [SerializeField] LayerMask collisionMask;
    [SerializeField] LineRenderer lineRend;

    Rigidbody2D myBody;
    CapsuleCollider2D myCollider;
    Vector2 currentHitPos;
    bool attached = false;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CapsuleCollider2D>();
        myGrappleJoint.transform.parent = null;
        myGrappleJoint.enabled = false;
        lineRend.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();

        if (attached)
        {
            lineRend.SetPosition(0, transform.position);
            lineRend.SetPosition(1, currentHitPos);
        }
    }

    void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckHit();
        }

        if (Input.GetMouseButtonUp(0))
        {
            RemoveGrapple();
        }
    }

    void RemoveGrapple()
    {
        if (attached)
        {
            myGrappleJoint.enabled = false;
            lineRend.enabled = false;
            myBody.velocity *= 2;
        }
    }

    void CheckHit()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, myCollider.size.x, Vector3.up, attachDistance, collisionMask);
        if (hit.collider != null)
        {
            AttachGrapple(hit.point);
        }
    }

    void AttachGrapple(Vector3 hitPos)
    {
        attached = true;
        lineRend.enabled = true;
        myGrappleJoint.enabled = true;

        currentHitPos = hitPos;

        lineRend.SetPosition(0, transform.position);
        lineRend.SetPosition(1, hitPos);
        myGrappleJoint.enabled = true;
        myGrappleJoint.transform.position = hitPos;
        float hitDistance = Vector3.Distance(transform.position, hitPos);
        print("Hit Grapple @distance: " + hitDistance);
        myGrappleJoint.distance = hitDistance;
    }
}
