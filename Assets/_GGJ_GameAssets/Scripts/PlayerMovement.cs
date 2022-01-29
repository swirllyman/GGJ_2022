using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]float moveSpeed = 1;
    [SerializeField] float maxVelocity = 1;
    [SerializeField] float wallDistanceCheck = .08f;

    [Header("Jumping")]
    [SerializeField] float initialJumpForce = 2.5f;
    [SerializeField] float sustainedJumpForce = .5f;
    [SerializeField] float groundedDistance = .1f;

    [Header("Extras")]
    [SerializeField] LayerMask collisionMask;
    [SerializeField] float resetDistance = 50;

    Vector3 startPosition;

    Rigidbody2D myBody;
    CapsuleCollider2D myCollider;
    Vector2 movePos;

    bool justJumped = false;

    #region Mono
    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CapsuleCollider2D>();

        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        movePos = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        CheckReset();
        CheckJump();
    }

    private void FixedUpdate()
    {
        UpdateMovement();
        UpdateJump();
    }
    #endregion

    void UpdateMovement()
    {
        Vector2 moveDir = new Vector2(movePos.x * moveSpeed, 0);
        if(moveDir.x > 0)
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        else if(moveDir.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (CanMove(moveDir))
        {
            myBody.AddForce(moveDir, ForceMode2D.Impulse);
        }

        if (Mathf.Abs(myBody.velocity.x) > maxVelocity)
        {
            myBody.velocity = new Vector3(myBody.velocity.x > 0 ? maxVelocity : -maxVelocity, myBody.velocity.y);
        }
    }
    
    void CheckReset()
    {
        if (Mathf.Abs(transform.position.x) > resetDistance || Mathf.Abs(transform.position.y) > resetDistance)
        {
            Reset();
        }
    }

    private void Reset()
    {
        myBody.velocity = Vector3.zero;
        transform.position = startPosition;
    }

    #region Jumping
    void CheckJump()
    {
        if (Grounded() & !justJumped)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }
    }

    void Jump()
    {
        justJumped = true;
        myBody.AddForce(Vector3.up * initialJumpForce, ForceMode2D.Impulse);
        StartCoroutine(ResetJump());
    }


    void UpdateJump()
    {
        if (justJumped)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                myBody.AddForce(Vector2.up * sustainedJumpForce);
            }
        }
    }

    IEnumerator ResetJump()
    {
        yield return new WaitForSeconds(.5f);
        justJumped = false;
    }
    #endregion

    //return true if we are NOT moving into a wall
    bool CanMove(Vector2 moveDir)
    {
        return Physics2D.Raycast(transform.position, moveDir.x > 0 ? Vector3.right : Vector3.left, wallDistanceCheck, collisionMask).collider == null;
    }

    //return true if an object is under us
    bool Grounded()
    {
        return Physics2D.CircleCast(transform.position, myCollider.size.x, Vector3.down, groundedDistance, collisionMask).collider != null;
    }
}