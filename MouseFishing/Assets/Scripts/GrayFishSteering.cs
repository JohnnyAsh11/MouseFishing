using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public enum FishLocation
{
    Point1,
    Point2,
    Point3,
    Point4
}


public class GrayFishSteering : Agent
{
    private Vector3 point1;
    [SerializeField]
    private Vector3 point2;
    [SerializeField]
    private Vector3 point3;
    [SerializeField]
    private Vector3 point4;
    [SerializeField]
    private float switchDistance = 2f;
    [SerializeField]
    private float seekWeight = 10f;
    [SerializeField]
    private float boundsWeight = 10f;

    [SerializeField]
    private SpriteRenderer renderer;

    private FishLocation fishLocation;

    // Start is called before the first frame update
    //  Overridding the start method written in the Agent class
    void Start()
    {
        //setting the starting point of the basic pathfinding algorithm
        point1 = transform.position;
        fishLocation = FishLocation.Point1;

        //instantiating the obstacle list
        foundObstacles = new List<Vector3>();
    }

    protected override void CalcSteeringForces()
    {
        //Finite state machine to control the fish's pathfinding
        switch (fishLocation)
        {
            case FishLocation.Point1:

                //seek the next point
                totalForce += Seek(point2) * seekWeight;

                //when the fish reaches that point
                if (Vector3.Distance(transform.position, point2) < switchDistance)
                {
                    //move the location to that point
                    fishLocation = FishLocation.Point2;
                }

                break;
            case FishLocation.Point2:
                
                //seek the next point
                totalForce += Seek(point3) * seekWeight;

                //when the fish reaches that point
                if (Vector3.Distance(transform.position, point3) < switchDistance)
                {
                    //move the location to that point
                    fishLocation = FishLocation.Point3;
                }

                break;
            case FishLocation.Point3:

                //seek the next point
                totalForce += Seek(point4) * seekWeight;

                //when the fish reaches that point
                if (Vector3.Distance(transform.position, point4) < switchDistance)
                {
                    //move the location to that point
                    fishLocation = FishLocation.Point4;
                }

                break;
            case FishLocation.Point4:

                //seek the next point
                totalForce += Seek(point1) * seekWeight;

                //when the fish reaches that point
                if (Vector3.Distance(transform.position, point1) < switchDistance)
                {
                    //move the location to that point
                    fishLocation = FishLocation.Point1;
                }

                break;
        }

        //makes it so blue fish are never upside down
        if (physObj.Velocity.x < 0)
        {
            renderer.flipX = false;
        }
        else
        {
            renderer.flipX = true;
        }

        //adding the separate force
        totalForce += Separate();

        //adding obstacle avoidance
        totalForce += AvoidObstacle(2);

        //keeping the fish in bounds
        totalForce += KeepInBoundsForce() * boundsWeight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (fishLocation == FishLocation.Point1)
        {
            Gizmos.DrawLine(transform.position, point2);
            Gizmos.DrawWireSphere(point2, switchDistance);
        }
        else if (fishLocation == FishLocation.Point2)
        {
            Gizmos.DrawLine(transform.position, point3); 
            Gizmos.DrawWireSphere(point3, switchDistance);
        }
        else if (fishLocation == FishLocation.Point3)
        {
            Gizmos.DrawLine(transform.position, point4);
            Gizmos.DrawWireSphere(point4, switchDistance);
        }
        else
        {
            Gizmos.DrawLine(transform.position, point1);
            Gizmos.DrawWireSphere(point1, switchDistance);
        }


        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, physObj.Radius);
    }
}
