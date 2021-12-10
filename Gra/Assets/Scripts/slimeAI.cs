using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slimeAI : MonoBehaviour
{
    public Enemy slime;
    public WorldManager worldManager;
    public Room room;

    float slimeGravity = 1f;
    float slimeJumpStr = 1.5f;

    Rigidbody2D rigidbody;
    BoxCollider2D collider;

    Vector2 colliderSize;
    Vector2 colliderOffset;

    bool onGround;
    bool hittingLeft;
    bool hittingRight;
    bool hittingTop;

    bool jumped;

    public float collisionTolerance = 0.05f;
    public float collisionOffSet = 0.05f;

    float lastjump;
    public void InitializeSlime(Enemy _slime,WorldManager _worldManager,Room _room)
    {
        slime = _slime;
        worldManager = _worldManager;
        room = _room;
    }
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = this.gameObject.AddComponent<Rigidbody2D>();
        rigidbody.isKinematic = true;

        collider = this.GetComponent<BoxCollider2D>();

        collider.size = new Vector2(1f, 0.765f);
        collider.offset = new Vector2(0f,-0.12f);
        colliderSize = collider.size;
        colliderOffset = collider.offset;
    }

    // Update is called once per frame
    void Update()
    {
        if (slime.invincibilityTimer>=0)
        {
            slime.invincibilityTimer = slime.invincibilityTimer - Time.deltaTime;
        }

        if (slime.health<=0)
        {
            worldManager.PlayerIncreaseScore(slime.scoreValue);
            this.gameObject.SetActive(false);
        }
        DetectCollisions();
        Vector2 playerPosition = worldManager.Player.transform.position;
        if (playerPosition.x > this.transform.position.x && onGround && !jumped && lastjump<=0f)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x + slimeJumpStr, rigidbody.velocity.y + slimeJumpStr);
            jumped = true;
            lastjump = 3f;
        }
        else if (playerPosition.x < this.transform.position.x && onGround && !jumped && lastjump <= 0f)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x - slimeJumpStr, rigidbody.velocity.y + slimeJumpStr);
            jumped = true;
            lastjump = 3f;
        }
        if (!hittingLeft && !hittingRight) rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y);
        if (hittingLeft)
        {
            if (rigidbody.velocity.x < 0) rigidbody.velocity = new Vector2(-rigidbody.velocity.x, rigidbody.velocity.y);
        }
        else if (hittingRight)
        {
            if (rigidbody.velocity.x > 0) rigidbody.velocity = new Vector2(-rigidbody.velocity.x, rigidbody.velocity.y);
        }
        if (onGround && !jumped)
        {

            rigidbody.velocity = new Vector2(0, 0);
        }
        else if (jumped)
        {
            if (!hittingTop)
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y - ((1 + slimeGravity) * Time.deltaTime));
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
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y - (slimeGravity * Time.deltaTime));
        }
        if(lastjump>0)lastjump = lastjump - Time.deltaTime;
    }
    public void TakeDamage(Vector2 damageSource,float damage)
    {
        if (slime.invincibilityTimer <= 0){
            slime.health = slime.health - (damage / (slime.armor+1));
            Vector2 damageVector = new Vector2(this.transform.position.x, this.transform.position.y) - damageSource;
            if (damageVector.x >= 0)
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x + (1f * damage), rigidbody.velocity.y + (1f * damage));
                jumped = true;
            }
            else
            {
                rigidbody.velocity = new Vector2(-rigidbody.velocity.x - (1f * damage), rigidbody.velocity.y + (1f * damage));
                jumped = true;
            }
            slime.invincibilityTimer = slime.invincibilityTime;
        }

    }
    void DamagePlayerContact(bool fromLeft)
    {
        worldManager.PlayerTakeDamage(slime.contactDamage,fromLeft);
    }
    void DetectCollisions()
    {
        int defaultLayerMask = ~(1 << 8);
        int layerMask = 0;
        if (rigidbody.velocity.y < 0) layerMask = layerMask + ~(1 << 7);

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
            colliderSize.y / 2f + collisionTolerance, defaultLayerMask + layerMask);
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
        Debug.DrawLine(LeftColliderPosition + new Vector2(collisionOffSet, 0), LeftColliderPosition + new Vector2(collisionOffSet, 0) + new Vector2(0f, colliderSize.y / 2f + collisionTolerance), Color.green);
        Debug.DrawLine(RightColliderPosition + new Vector2(-collisionOffSet, 0), RightColliderPosition + new Vector2(-collisionOffSet, 0) + new Vector2(0f, colliderSize.y / 2f + collisionTolerance), Color.green);
        RaycastHit2D TopRayHitLeft = Physics2D.Raycast(LeftColliderPosition + new Vector2(collisionOffSet, 0),
            new Vector2(0f, 1f), colliderSize.y / 2f + collisionTolerance, defaultLayerMask + layerMask);
        RaycastHit2D TopRayHitRight = Physics2D.Raycast(RightColliderPosition + new Vector2(-collisionOffSet, 0),
            new Vector2(0f, 1f), colliderSize.y / 2f + collisionTolerance, defaultLayerMask + layerMask);
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
        if ((groundRayHitLeft.collider != null && groundRayHitLeft.collider.CompareTag("Player"))
            || (groundRayHitRight.collider != null && groundRayHitRight.collider.CompareTag("Player"))
            || (leftCollisionUpper.collider != null && leftCollisionUpper.collider.CompareTag("Player"))
            || (leftCollisionLower.collider != null && leftCollisionLower.collider.CompareTag("Player"))
            ) DamagePlayerContact(false);
        else if ((rightCollisionUpper.collider != null && rightCollisionUpper.collider.CompareTag("Player"))
            || (rightCollisionLower.collider != null && rightCollisionLower.collider.CompareTag("Player"))
            ) DamagePlayerContact(true);
    }
}
