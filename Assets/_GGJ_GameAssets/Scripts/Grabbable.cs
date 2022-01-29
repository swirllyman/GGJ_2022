using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    [SerializeField] SpriteRenderer myRend;
    [SerializeField] Color flashColor;
    [SerializeField] float flashTime = 1.0f;
    [SerializeField] LeanTweenType tweenType;

    Rigidbody2D myBody;
    Collider2D myCollider;

    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        myRend = GetComponent<SpriteRenderer>();
        LeanTween.color(myRend.gameObject, flashColor, flashTime).setLoopPingPong().setEase(tweenType);
    }

    public void PickUp()
    {
        myCollider.enabled = false;
        myBody.bodyType = RigidbodyType2D.Kinematic;
        myBody.velocity = Vector2.zero;
        myBody.angularVelocity = 0.0f;
    }

    public void Drop()
    {
        myCollider.enabled = true;
        myBody.bodyType = RigidbodyType2D.Dynamic;
    }

    public void Throw(Vector3 direction, float force)
    {
        Drop();
        myBody.AddForce(direction * force, ForceMode2D.Impulse);
        myBody.angularVelocity = -55 * force;
    }
}
