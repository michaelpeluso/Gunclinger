using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSlopeMovement : MonoBehaviour
{
    bool IsGrounded;
    Vector2 velocity;
    //float gravityModifier = 0.6f;
    Vector2 groundNormal;
    ContactFilter2D contactFilter;
    Rigidbody2D rb;

    private void Awake() {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = false;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        velocity += Physics2D.gravity * Time.deltaTime;
        velocity.x = 0f;
        IsGrounded = false;
        var deltaPosition = velocity * Time.deltaTime;
        var moveAlongGround = new Vector2(groundNormal.y, -groundNormal.x);
        var move = moveAlongGround * deltaPosition.x;
        PerformMovement(move, false);
        move = Vector2.up * deltaPosition.y;
        PerformMovement(move, true);

    }

    void PerformMovement(Vector2 move, bool yMovement)
    {
        var distance = move.magnitude;

        if (distance > 0.001f)
        {
            //check if we hit anything in current direction of travel
            RaycastHit2D[] hitBuffer = new RaycastHit2D[16];

            var count = rb.Cast(move, contactFilter, hitBuffer, distance + 0.01f);
            for (var i = 0; i < count; i++)
            {
                var currentNormal = hitBuffer[i].normal;

                //is this surface flat enough to land on?
                //if (currentNormal.y > minGroundNormalY)
                //{
                    IsGrounded = true;
                    // if moving up, change the groundNormal to new surface normal.
                    if (yMovement)
                    {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                //}
                if (IsGrounded)
                {
                    //how much of our velocity aligns with surface normal?
                    var projection = Vector2.Dot(velocity, currentNormal);
                    if (projection < 0)
                    {
                        //slower velocity if moving against the normal (up a hill).
                        velocity = velocity - projection * currentNormal;
                    }
                }
                else
                {
                    //We are airborne, but hit something, so cancel vertical up and horizontal velocity.
                    velocity.x *= 0;
                    velocity.y = Mathf.Min(velocity.y, 0);
                }
                //remove shellDistance from actual move distance.
                var modifiedDistance = hitBuffer[i].distance - 0.01f;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }
        }
        rb.position = rb.position + move.normalized * distance;
    }
}
