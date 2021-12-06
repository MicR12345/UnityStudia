using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
//Mial byc skrypt do ruchu ale ostatecznie stal sie skryptem do wszystkiego
//nie zmieniam juz nazwy
public class PlayerMovement : MonoBehaviour
{
    public float jumpStr = 3f;
    public float gravity = 1f;
    public float playerSpeed = 1f;
    public float knockback = 3f;


    public float collisionTolerance = 0.05f;
    public float collisionOffSet = 0.05f;

    bool onGround;
    bool hittingLeft;
    bool hittingRight;
    bool hittingTop;
    bool knockedBack;

    bool jumped;
    //animation bools
    public bool isRunning;
    public bool isStanding;
    public bool justJumped;
    public bool midAir;
    public bool attacked;
    public bool interacted;


    public bool lockMovement = false;

    ActionsInput inputActions;
    Rigidbody2D rigidbody;
    BoxCollider2D boxCollider;
    SpriteRenderer spriteRenderer;
    SpriteRenderer weaponRenderer;
    Animator animator;

    Vector2 colliderSize;
    Vector2 colliderOffset;

    public GameObject WeaponDamageCheckOffsetGO;
    public Vector2 WeaponCheckOffset;
    public float WeaponRadius;

    public GameObject worldHandler;
    WorldManager world;

    public GameObject weapon;
    private void Awake()
    {
        inputActions = new ActionsInput();
        inputActions.Enable();

        inputActions.Player.Attack.performed += On_Attack;
        inputActions.Player.Jump.performed += On_Jump;
        inputActions.Player.MainMenu.performed += MainMenu_performed;
    }

    private void MainMenu_performed(InputAction.CallbackContext obj)
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void On_Jump(InputAction.CallbackContext obj)
    {
        if (onGround && !lockMovement)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y + jumpStr);
            jumped = true;
        }
    }

    private void On_Attack(InputAction.CallbackContext obj)
    {
        
        if (world.playerData.attackCooldownTimer <= 0)
        {
            animator.SetFloat("attackCooldown", 1/world.playerData.attackCooldown);
            weapon.SetActive(true);
            world.playerData.attackCooldownTimer = world.playerData.attackCooldown;
            if (spriteRenderer.flipX)
            {
                animator.Play("AttackLeft");
            }
            else
            {
                animator.Play("AttackRight");
            }
            
            lockMovement = true;
            Debug.Log("Attacked");
            Collider2D[] collider2Ds = Physics2D.OverlapCircleAll(new Vector2(this.transform.position.x,this.transform.position.y) + WeaponCheckOffset, WeaponRadius);
            Debug.Log(collider2Ds.Length);
            Debug.DrawLine(new Vector2(this.transform.position.x, this.transform.position.y) + WeaponCheckOffset, new Vector2(this.transform.position.x, this.transform.position.y), Color.white);
            foreach (Collider2D item in collider2Ds)
            {
                if (item.CompareTag("Enemy"))
                {
                    if (item.GetComponent<slimeAI>()!=null)
                    {
                        slimeAI ai = item.GetComponent<slimeAI>();
                        ai.TakeDamage(new Vector2(this.transform.position.x, this.transform.position.y) + WeaponCheckOffset, world.playerData.attackDamage);
                    }
                }
            }
        }
    }

    public void TakeDamage(bool fromLeft)
    {
        knockedBack = true;
        if (fromLeft)
        {
            if (onGround)
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x + knockback, rigidbody.velocity.y + knockback);
            }
            else
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x + knockback, rigidbody.velocity.y);
            }
        }
        else
        {
            if (onGround)
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x - knockback, rigidbody.velocity.y + knockback);
            }
            else
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x - knockback, rigidbody.velocity.y);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = this.GetComponent<Rigidbody2D>();
        world = worldHandler.GetComponent<WorldManager>();
        boxCollider = this.GetComponent<BoxCollider2D>();
        spriteRenderer = this.GetComponent<SpriteRenderer>();
        animator = this.GetComponent<Animator>();
        weaponRenderer = weapon.GetComponent<SpriteRenderer>();

        WeaponCheckOffset = WeaponDamageCheckOffsetGO.transform.localPosition;

        colliderSize = boxCollider.size;
        colliderOffset = boxCollider.offset;
    }

    // Update is called once per frame
    void Update()
    {
        if (world.playerData.attackCooldownTimer <= 0)
        {
            if (lockMovement)
            {
                animator.Play("Standing");
                lockMovement = false;
            }
            weapon.SetActive(false);
        }

        Vector2 moveVector = inputActions.Player.Move.ReadValue<Vector2>();
        float moveX = moveVector.x;
        float moveY = moveVector.y;
        if (lockMovement)
        {
            moveX = 0;
            moveY = 0;
        }
        

        if (moveX < 0)
        {
            if (WeaponCheckOffset.x > 0)
            {
                WeaponCheckOffset = new Vector2(-WeaponCheckOffset.x, WeaponCheckOffset.y);
            }
            if (weapon.transform.localScale.x > 0)
            {
                weapon.transform.localScale = new Vector2(-weapon.transform.localScale.x, weapon.transform.localScale.y);
            }
            weaponRenderer.flipX = true;
            spriteRenderer.flipX = true;
            isRunning = true;
            isStanding = false;
        }
        if (moveX > 0)
        {
            if (WeaponCheckOffset.x<0)
            {
                WeaponCheckOffset = new Vector2(-WeaponCheckOffset.x, WeaponCheckOffset.y);
            }
            if (weapon.transform.localScale.x < 0)
            {
                weapon.transform.localScale = new Vector2(-weapon.transform.localScale.x, weapon.transform.localScale.y);
            }
            weaponRenderer.flipX = false;
            spriteRenderer.flipX = false;
            isRunning = true;
            isStanding = false;
        }
        if (moveX==0)
        {
            isRunning = false;
            isStanding = true;
        }
        DetectCollisions();
        if (!hittingLeft && !hittingRight && !knockedBack) rigidbody.velocity = new Vector2(moveX * playerSpeed, rigidbody.velocity.y);
        else if (!hittingLeft && !hittingRight && knockedBack) rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y);
        else if (hittingLeft)
        {
            if (moveX < 0) rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
            else
            {
                if (!knockedBack) rigidbody.velocity = new Vector2(moveX * playerSpeed, rigidbody.velocity.y);
                else
                {
                    if (knockedBack && rigidbody.velocity.x < 0) rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y);
                    else rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
                }
            }
        }
        else if (hittingRight)
        {
            if (moveX > 0) rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
            else
            {
                if (!knockedBack) rigidbody.velocity = new Vector2(moveX * playerSpeed, rigidbody.velocity.y);
                else
                {
                    if (knockedBack && rigidbody.velocity.x > 0) rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y);
                    else rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
                }
            }
        }
        else
        {
            rigidbody.velocity = new Vector2(0, rigidbody.velocity.y);
        }
        if (onGround && !jumped && !knockedBack)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
            transform.position = new Vector2(transform.position.x, Mathf.FloorToInt(transform.position.y) + 0.35f);
        }
        else if (jumped)
        {
            if (!hittingTop)
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y - ((moveY + gravity) * Time.deltaTime));
                if (rigidbody.velocity.y <= 0)
                {
                    jumped = false;
                }
            }
            else
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, 0);
                jumped = false;
            }
        }
        else
        {
            if (rigidbody.velocity.y <= 0) knockedBack = false;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y - (gravity * Time.deltaTime));
        }
        animator.SetFloat("SpeedX", Mathf.Abs(rigidbody.velocity.x));
        animator.SetFloat("SpeedY", Mathf.Abs(rigidbody.velocity.y)+0.01f);
        animator.SetBool("onGround", onGround);
    }

    void DetectCollisions()
    {
        Vector2 moveVector = inputActions.Player.Move.ReadValue<Vector2>();

        float moveX = moveVector.x;
        float moveY = moveVector.y;

        int defaultLayerMask = ~(1 << 3) + ~(1 << 9);
        int layerMask = 0;
        if (moveY < 0) layerMask = layerMask + ~(1 << 7);

        //Collision Detection
        Vector2 positionAsV2 = new Vector2(this.transform.position.x, this.transform.position.y);
        Vector2 LeftColliderPosition = new Vector2(positionAsV2.x - colliderSize.x / 2f, positionAsV2.y) + colliderOffset;
        Vector2 RightColliderPosition = new Vector2(positionAsV2.x + colliderSize.x / 2f, positionAsV2.y) + colliderOffset;
        Vector2 UpperColliderPosition = new Vector2(positionAsV2.x + colliderOffset.x, positionAsV2.y + colliderSize.y / 2f + colliderOffset.y - collisionOffSet * 3);
        Vector2 LowerColliderPosition = new Vector2(positionAsV2.x + colliderOffset.x, positionAsV2.y - colliderSize.y / 2f + colliderOffset.y + collisionOffSet * 3);

        //Ground collision

        Debug.DrawRay(LeftColliderPosition, new Vector2(0f, -1f), Color.green);
        Debug.DrawRay(RightColliderPosition, new Vector2(0f, -1f), Color.green);
        RaycastHit2D groundRayHitLeft = Physics2D.Raycast(LeftColliderPosition + new Vector2(collisionOffSet, 0), new Vector2(0f, -1f),
            colliderSize.y / 2f + collisionTolerance,defaultLayerMask +layerMask);
        RaycastHit2D groundRayHitRight = Physics2D.Raycast(RightColliderPosition + new Vector2(-collisionOffSet, 0), new Vector2(0f, -1f),
            colliderSize.y / 2f + collisionTolerance, defaultLayerMask + layerMask);
        if (groundRayHitLeft.collider != null || groundRayHitRight.collider != null)
        {
            onGround = true;
        }
        else onGround = false;

        //LeftCollision
        
        Debug.DrawRay(UpperColliderPosition, new Vector2(-1f, 0f), Color.red);
        Debug.DrawRay(LowerColliderPosition, new Vector2(-1f, 0f), Color.red);
        RaycastHit2D leftCollisionUpper = Physics2D.Raycast(UpperColliderPosition, new Vector2(-1f, 0f),
            colliderSize.x / 2f + collisionTolerance, defaultLayerMask + layerMask);
        RaycastHit2D leftCollisionLower = Physics2D.Raycast(LowerColliderPosition, new Vector2(-1f, 0f),
            colliderSize.x / 2f + collisionTolerance, defaultLayerMask + layerMask);
        if (leftCollisionUpper.collider != null || leftCollisionLower.collider != null) hittingLeft = true;
        else hittingLeft = false;

        //RightCollision

        Debug.DrawRay(UpperColliderPosition, new Vector2(1f, 0f), Color.red);
        Debug.DrawRay(LowerColliderPosition, new Vector2(1f, 0f), Color.red);
        RaycastHit2D rightCollisionUpper = Physics2D.Raycast(UpperColliderPosition, new Vector2(1f, 0f),
            colliderSize.x / 2f + collisionTolerance, defaultLayerMask + layerMask);
        RaycastHit2D rightCollisionLower = Physics2D.Raycast(LowerColliderPosition, new Vector2(1f, 0f),
            colliderSize.x / 2f + collisionTolerance, defaultLayerMask + layerMask);
        if (rightCollisionUpper.collider != null || rightCollisionLower.collider != null) hittingRight = true;
        else hittingRight = false;

        //UpCollision
        Debug.DrawLine(LeftColliderPosition + new Vector2(collisionOffSet, 0),LeftColliderPosition + new Vector2(collisionOffSet, 0) + new Vector2(0f, colliderSize.y / 2f + collisionTolerance), Color.green);
        Debug.DrawLine(RightColliderPosition + new Vector2(-collisionOffSet, 0), RightColliderPosition + new Vector2(-collisionOffSet, 0) + new Vector2(0f, colliderSize.y / 2f + collisionTolerance), Color.green);
        RaycastHit2D TopRayHitLeft = Physics2D.Raycast(LeftColliderPosition + new Vector2(collisionOffSet,0),
            new Vector2(0f, 1f), colliderSize.y / 2f + collisionTolerance, defaultLayerMask + layerMask);
        RaycastHit2D TopRayHitRight = Physics2D.Raycast(RightColliderPosition + new Vector2(-collisionOffSet, 0),
            new Vector2(0f, 1f), colliderSize.y / 2f + collisionTolerance, defaultLayerMask +layerMask);
        if (TopRayHitLeft.collider != null || TopRayHitRight.collider != null)
        {
            if (jumped 
                && TopRayHitLeft.collider != null
                && TopRayHitLeft.collider.CompareTag("platform")
                && TopRayHitRight.collider != null
                && TopRayHitRight.collider.CompareTag("platform"))
                hittingTop = false;
            else hittingTop = true;
        }
        else hittingTop = false;
    }
}
