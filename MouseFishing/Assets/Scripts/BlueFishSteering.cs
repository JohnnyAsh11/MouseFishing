using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// Enumeration to track the state that the fish is in
/// </summary>
public enum FishState
{
    Exploring,
    Chasing,
    Escape,
    Group
}

public class BlueFishSteering : Agent
{
    [SerializeField]
    private FishState fState;

    [SerializeField]
    private float wanderTime = 1f;
    [SerializeField]
    private float wanderRadius = 1f;
    [SerializeField]
    private float boundsWeight = 1f;
    [SerializeField]
    private float wanderWeight = 1f;
    [SerializeField]
    private float seekWeight = 1f;

    [SerializeField]
    private float timer;
    [SerializeField]
    private float avoidTime;

    [SerializeField]
    private Agent targetAgent;
    [SerializeField]
    private SpriteRenderer renderer;

    protected override void CalcSteeringForces()
    {
        //switch statement to track the type of fish forces are being calculated for
        switch (fState)
        {
            //in this state the fish is wandering around and not really doing anything too crazy
            case FishState.Exploring:

                //Calculating and adding the Wander force to the total force
                totalForce += Wander(wanderTime, wanderRadius) * wanderWeight;


                //counts to one second
                timer += Time.deltaTime;
                if (timer >= 3)
                {
                    //getting a random value
                    float rng = Random.value;
                    timer = 0;

                    //every 3 seconds there is a 10 percent chance that
                    // the blue fish will start chasing another fish
                    if (rng >= .9)
                    {
                        //getting the random index of the target fish
                        int fish = Random.Range(0, fishManager.ActiveFish.Count);

                        //getting the target
                        targetAgent = fishManager.ActiveFish[fish];

                        //checking to make sure BlueFish do not seek themselves
                        while (targetAgent == this)
                        {
                            fish = Random.Range(0, fishManager.ActiveFish.Count);
                            targetAgent = fishManager.ActiveFish[fish];
                        }

                        //setting the timer for the chase state
                        timer = 5;

                        //changing states
                        fState = FishState.Chasing;
                        renderer.color = Color.magenta;
                    }
                }

                break;
            //randomly, the bluefish will chase an random target for 5 seconds
            case FishState.Chasing:

                //checking for null values
                if (targetAgent != null)
                {
                    //seeking out our target
                    totalForce += Seek(targetAgent) * seekWeight;
                }

                //timer handling
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    fState = FishState.Exploring;
                    renderer.color = Color.white;
                }
                break;
        }

        //makes it so blue fish are never upside down
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

        //Always calculate Separate
        totalForce += Separate();

        //Obstacle avoidance
        totalForce += AvoidObstacle(avoidTime);

        //keeping everything at 0 on the z axis
        totalForce.z = 0;
    }

    private void OnDrawGizmos()
    {
        //
        //  Draw safe space box
        //
        Vector3 futurePos = CalcFuturePosition(avoidTime);

        float dist = Vector3.Distance(transform.position, futurePos) + physObj.Radius;

        Vector3 boxSize = new Vector3(physObj.Radius * 2f,
            dist,
            physObj.Radius * 2f);

        Vector3 boxCenter = Vector3.zero;
        boxCenter.y += dist / 2f;

        Gizmos.color = Color.green;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(boxCenter, boxSize);
        Gizmos.matrix = Matrix4x4.identity;


        //
        //  Draw lines to found obstacles
        //
        Gizmos.color = Color.red;
        foreach (Vector3 pos in foundObstacles)
        {
            Gizmos.DrawLine(transform.position, pos);
        }
    }
}
