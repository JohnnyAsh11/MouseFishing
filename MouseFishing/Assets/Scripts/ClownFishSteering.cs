using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClownFishSteering : Agent
{
    [SerializeField]
    private SpriteRenderer renderer;
    [SerializeField]
    private float boundsWeight = 1f;
    [SerializeField]
    private float cohesionWight = 1f;
    [SerializeField]
    private float alignWeight = 1f;
    [SerializeField]
    private float fleeWeight = 1f;

    [SerializeField]
    private float timer;
    [SerializeField]
    private float avoidTime;

    [SerializeField]
    private FishState fState;

    private int inRangeWanders;

    protected override void CalcSteeringForces()
    {
        inRangeWanders = 0;

        switch (fState)
        {
            case FishState.Escape:
                Vector3 closestFish = Vector3.zero;
                float closestDistance = float.MaxValue;

                //looking through the wanderers to check ranges and who to flee from
                foreach (Agent wanderer in fishManager.WanderFish)
                {
                    //finding the distance between the fish and the wandering fish
                    float distance = Vector3.Distance(
                        transform.position,
                        wanderer.transform.position);

                    //if the wanderer is in range
                    if (distance < 5f)
                    {
                        //increment the wanderer counter
                        inRangeWanders++;
                    }

                    //if the calculated distance is less then the currently saved distance
                    if (distance < closestDistance)
                    {
                        //replace the saved position with this agent's position
                        closestFish = wanderer.transform.position;

                        //also replace the saved distance
                        closestDistance = distance;
                    }
                }

                //if there are less than 5 wanderers nearby then switch back to the group state
                // and change back to a normal color tint
                if (inRangeWanders < 5)
                {
                    fState = FishState.Group;
                    renderer.color = Color.white;
                    break;
                }


                //applying the separate force
                totalForce += Separate();

                //fleeing from the closest fish
                totalForce += Flee(closestFish) * fleeWeight;

                break;
            case FishState.Group:

                //Since the clown fish is in the group state, 
                //  add the flock force (which includes separate)
                //  to the totalForce
                totalForce += Flock(cohesionWight, alignWeight);

                //checking fish distances for the state change
                foreach (Agent wanderer in fishManager.WanderFish)
                {
                    //finding the distance between the fish and the wandering fish
                    float distance = Vector3.Distance(
                        transform.position,
                        wanderer.transform.position);

                    if (distance < 5f)
                    {
                        inRangeWanders++;

                        //if the counter hits five then break from the loop,
                        // change state and tint the fish
                        if (inRangeWanders >= 5)
                        {
                            fState = FishState.Escape;
                            renderer.color = Color.magenta;
                            break;
                        }
                    }
                }

                break;
        }

        //makes it so clown fish are never upside down
        if (physObj.Velocity.x < 0)
        {
            renderer.flipX = true;
        }
        else
        {
            renderer.flipX = false;
        }

        //Always calculate the Bounds force
        totalForce += KeepInBoundsForce() * boundsWeight;

        //Obstacle avoidance
        totalForce += AvoidObstacle(avoidTime);

        //keeping everything at 0 on the z axis
        totalForce.z = 0;
    }

    //obstacle avoidance gizmo
    //private void OnDrawGizmos()
    //{
    //    //
    //    //  Draw safe space box
    //    //
    //    Vector3 futurePos = CalcFuturePosition(4);
    //
    //    float dist = Vector3.Distance(transform.position, futurePos) + physObj.Radius;
    //
    //    Vector3 boxSize = new Vector3(physObj.Radius * 2f,
    //        dist,
    //        physObj.Radius * 2f);
    //
    //    Vector3 boxCenter = Vector3.zero;
    //    boxCenter.y += dist / 2f;
    //
    //    Gizmos.color = Color.green;
    //
    //    Gizmos.matrix = transform.localToWorldMatrix;
    //    Gizmos.DrawWireCube(boxCenter, boxSize);
    //    Gizmos.matrix = Matrix4x4.identity;
    //
    //
    //    //
    //    //  Draw lines to found obstacles
    //    //
    //    Gizmos.color = Color.red;
    //    foreach (Vector3 pos in foundObstacles)
    //    {
    //        Gizmos.DrawLine(transform.position, pos);
    //    }
    //}
}
