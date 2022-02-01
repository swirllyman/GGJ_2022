using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
[RequireComponent(typeof(AudioSource))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeedGround = 1;
    [SerializeField] float moveSpeedAir = .35f;
    [SerializeField] float maxVelocity = 1;
    [SerializeField] float wallDistanceCheck = .08f;
    [SerializeField] Vector2 verticalVelocityMax = new Vector2(3, 3);
    [SerializeField] AudioClip runClip;

    [Header("Jumping")]
    [SerializeField] float initialJumpForce = 2.5f;
    [SerializeField] float sustainedJumpForce = .5f;
    [SerializeField] float groundedDistance = .1f;
    [SerializeField] AudioClip landingClip;
    [SerializeField] AudioClip jumpClip;

    [Header("Slope")]
    [SerializeField] float slopeCheckDistance;
    [SerializeField] float maxSlopeAngle;
    [SerializeField] PhysicsMaterial2D noFriction;
    [SerializeField] PhysicsMaterial2D fullFriction;

    [Header("Extras")]
    [SerializeField] Animator myAnim;
    [SerializeField] LayerMask collisionMask;
    [SerializeField] float resetDistance = 50;

    Rigidbody2D myBody;
    CapsuleCollider2D myCollider;
    AudioSource audioSource;

    Vector3 startPosition;
    Vector2 slopeNormalPerp;
    Vector2 movePos;
    Vector2 moveDir;

    Coroutine jumpRoutine;

    private bool isOnSlope;
    private bool canWalkOnSlope;
    private float slopeDownAngle;
    private float slopeSideAngle;
    private float lastSlopeAngle;

    bool justJumped = false;
    bool grounded = false;
    bool wasGrounded = false;
    bool wasMoving = false;

    #region Mono
    private void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();

        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();
        movePos = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        CheckReset();
        CheckJump();
    }

    private void FixedUpdate()
    {
        SlopeCheck();
        UpdateMovement();
        UpdateJump();
        UpdateAnimation();
    }
    #endregion

    void MovingChanged()
    {
        wasMoving = !wasMoving;

        if (grounded)
        {
            if (wasMoving)
            {
                audioSource.clip = runClip;
                audioSource.Play();
            }
            else
            {
                audioSource.Stop();
            }
        }
    }

    void UpdateMovement()
    {

        moveDir = new Vector2(movePos.x * (grounded ? moveSpeedGround : moveSpeedAir), 0);

        if(Mathf.Abs(moveDir.x) > 0)
        {
            if (!wasMoving)
            {
                MovingChanged();
            }
        }
        else
        {
            if (wasMoving)
            {
                MovingChanged();
            }
        }
        //if (grounded && isOnSlope && canWalkOnSlope && !justJumped) 
        //{

        //    myBody.velocity = new Vector2(slopeNormalPerp.x * -moveDir.x, slopeNormalPerp.y * -moveDir.x);
        //}
        //else
        //{
        //    if (moveDir.x > 0)
        //    {
        //        transform.rotation = Quaternion.Euler(Vector3.zero);
        //    }
        //    else if (moveDir.x < 0)
        //    {
        //        transform.rotation = Quaternion.Euler(0, 180, 0);
        //    }
        //    if (CanMove(moveDir))
        //    {
        //        myBody.AddForce(moveDir, ForceMode2D.Impulse);
        //    }
        //}
        if (moveDir.x > 0)
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        else if (moveDir.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        if (CanMove(moveDir))
        {
            myBody.AddForce(moveDir, ForceMode2D.Impulse);
        }

        //Limiting Velocity
        if (Mathf.Abs(myBody.velocity.x) > maxVelocity)
        {
            myBody.velocity = new Vector3(myBody.velocity.x > 0 ? maxVelocity : -maxVelocity, myBody.velocity.y);
        }

        if (myBody.velocity.y < 0 && Mathf.Abs(myBody.velocity.y) > verticalVelocityMax.x)
        {
            myBody.velocity = new Vector3(myBody.velocity.x, -verticalVelocityMax.x);
        }

        if (myBody.velocity.y > 0 && Mathf.Abs(myBody.velocity.y) > verticalVelocityMax.y)
        {
            myBody.velocity = new Vector3(myBody.velocity.x, verticalVelocityMax.y);
        }
    }
    
    void UpdateAnimation()
    {
        if (!justJumped)
        {
            myAnim.SetFloat("GroundSpeed", Mathf.Abs(moveDir.x) > 0 ? 1.0f : 0.0f);
            myAnim.SetBool("Falling", !grounded);
        }
    }

    void CheckReset()
    {
        if (Mathf.Abs(transform.position.x) > resetDistance || Mathf.Abs(transform.position.y) > resetDistance)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
        if (grounded & !justJumped)
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
        //Debug.Log("Jumping");
        myAnim.SetBool("Jumping", true);
        myBody.AddForce(Vector3.up * initialJumpForce, ForceMode2D.Impulse);
        if (jumpRoutine != null) StopCoroutine(jumpRoutine);
        jumpRoutine = StartCoroutine(ResetJump());
        audioSource.PlayOneShot(jumpClip);
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
        yield return new WaitForSeconds(1.0f);
        StopJump();
    }

    void StopJump()
    {
        if (jumpRoutine != null) StopCoroutine(jumpRoutine);
        myAnim.SetBool("Jumping", false);
        justJumped = false;
    }
    #endregion

    #region Slope
    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - (Vector3)(new Vector2(0.0f, myCollider.size.y / 2));

        SlopeCheckHorizontal(checkPos);
        SlopeCheckVertical(checkPos);
    }

    private void SlopeCheckHorizontal(Vector2 checkPos)
    {
        RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, collisionMask);
        RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, collisionMask);

        if (slopeHitFront)
        {
            isOnSlope = true;

            slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);

        }
        else if (slopeHitBack)
        {
            isOnSlope = true;

            slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
        }
        else
        {
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }

    }

    private void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, collisionMask);

        if (hit)
        {

            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != lastSlopeAngle)
            {
                isOnSlope = true;
            }

            lastSlopeAngle = slopeDownAngle;

            Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
            Debug.DrawRay(hit.point, hit.normal, Color.green);

        }

        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }

        if (isOnSlope && canWalkOnSlope && moveDir.x == 0.0f)
        {
            myBody.sharedMaterial = fullFriction;
        }
        else
        {
            myBody.sharedMaterial = noFriction;
        }
    }
    #endregion

    //return true if we are NOT moving into a wall
    bool CanMove(Vector2 moveDir)
    {
        return Physics2D.Raycast(transform.position, moveDir.x > 0 ? Vector3.right : Vector3.left, wallDistanceCheck, collisionMask).collider == null;
    }

    //return true if an object is under us
    void CheckGrounded()
    {
        grounded = Physics2D.CircleCast(transform.position, myCollider.size.y, Vector3.down, groundedDistance, collisionMask).collider != null;
        if(wasGrounded != grounded)
        {
            GroundedChanged();
        }
    }

    void GroundedChanged()
    {
        wasGrounded = grounded;
        if (grounded && justJumped)
        {
            StopJump();
        }

        if (!grounded)
        {
            audioSource.Stop();
            //if (audioSource.Stop()
            //audioSource.clip = fallingClip;
            //audioSource.Play();
        }
        else
        {
            audioSource.PlayOneShot(landingClip);
            if (wasMoving)
            {
                audioSource.clip = runClip;
                audioSource.Play();
            }
            else
            {
                audioSource.Stop();
            }
        }

    }
}