using System.Collections.Generic;
using System.Reflection;
using Unity.Collections;
using UnityEngine;

public abstract class Agent : MonoBehaviour
{
    [SerializeField]
    protected PhysicsObject physObj;

    protected FishManager fishManager;

    [SerializeField]
    private float maxForce = 20f;
    [SerializeField]
    protected float maxSpeed = 10f;
    [SerializeField]
    private float borderPadding = 0f;
    [SerializeField]
    private float separateWeight = 1f;
    [SerializeField]
    private float avoidWeight = 1f;

    protected Vector3 totalForce;

    private float wanderTimer = 0;
    private float wanderAngle;

    //camera information
    private float height;
    private float width;
    Camera camera;

    protected List<Vector3> foundObstacles;

    [SerializeField]
    private Vector3 cohesionPoint;
    private Vector3 alignmentDirection;

    public PhysicsObject PhysObj { get { return physObj; } }    

    public FishManager FishManager { set { fishManager = value; } }

    // Start is called before the first frame update
    void Start()
    {
        //grabbing a reference to the Camera
        camera = Camera.main;

        //calculating the height and width of the screen
        height = 2f * camera.orthographicSize;
        width = height * camera.aspect;

        //calculating the first wanderAngle
        wanderAngle = Random.Range(0, Mathf.PI * 2);

        //instantiating the obstacle list
        foundObstacles = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        //zeroing out the totalForce at the start of every iteration
        totalForce = Vector3.zero;        

        //calculating this Agent's steering force
        CalcSteeringForces();

        //clamping the totalForce of the steering forces
        totalForce = Vector3.ClampMagnitude(totalForce, maxForce);

        //applying the steering force
        physObj.ApplyForce(totalForce);

        //rotating to face the new direction
        transform.rotation = Quaternion.LookRotation(Vector3.forward, physObj.Velocity.normalized);
    }

    /// <summary>
    /// Method for children of Agent to use to calculate their steering forces
    /// </summary>
    protected abstract void CalcSteeringForces();

    /// <summary>
    /// Calculates a Seek steering force to a target position
    /// </summary>
    /// <param name="targetPosition">Position at which the Agent is seeking</param>
    /// <returns>the correct force to travel to the target</returns>
    protected Vector3 Seek(Vector3 targetPosition)
    {
        Vector3 seekingForce;

        //calculating the desired velocity which will be straight to the target
        Vector3 desiredVelocity = targetPosition - transform.position;
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        //calculating the proper force required to smoothly travel to the desired velocity
        seekingForce = desiredVelocity - physObj.Velocity;

        return seekingForce;
    }

    /// <summary>
    /// Overload of Seek Method to use a general GameObject instead of a Vector3
    /// </summary>
    /// <param name="target">Target being seeked</param>
    /// <returns>a Seek force to the target</returns>
    protected Vector3 Seek(Agent target)
    {
        return Seek(target.transform.position);
    }
    
    /// <summary>
    /// Calculates a Flee steering force away from a target position
    /// </summary>
    /// <param name="targetPosition">Position being fleed from</param>
    /// <returns>The correct force to travel away from the target position</returns>
    protected Vector3 Flee(Vector3 targetPosition)
    {
        Vector3 seekingForce;

        //calcualting the opposite desired velocity to that of seek
        Vector3 desiredVelocity = transform.position - targetPosition;
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        //calculating the proper force required to smoothly travel to the desired velocity
        seekingForce = desiredVelocity - physObj.Velocity;

        return seekingForce;
    }

    /// <summary>
    /// Overload of Flee Method to use a general GameObject instead of a Vector3
    /// </summary>
    /// <param name="target">Target being fleed from</param>
    /// <returns>a Flee force away from the target</returns>
    protected Vector3 Flee(Agent target)
    {
        return Flee(target.transform.position);
    }

    /// <summary>
    /// Returns a force to keep the Agent in a specific bounds
    /// </summary>
    /// <returns>the force necessary to keep the Agent in bounds</returns>
    protected Vector3 KeepInBoundsForce()
    {
        Vector3 position = transform.position;

        //Checking the location of the Agent against the camera bounds
        if (position.y >= height - borderPadding ||
            position.y <= (height - height) + borderPadding ||
            position.x >= width - borderPadding ||
            position.x <= (width - width + borderPadding))
        {
            //this vector is my center point as opposed to (0, 0, 0)
            return Seek(new Vector3(10, 5, 0));
        }

        return Vector3.zero;
    }

    /// <summary>
    /// Calculates a random Wander force to be applied to the Agent 
    /// </summary>
    /// <param name="time">How far ahead the future Position will calculate</param>
    /// <param name="radius">The radius of the Wander Algorithm's circle</param>
    /// <returns>A wander force for the Agent</returns>
    protected Vector3 Wander(float time, float radius)
    {
        //finding the location of the projected wander circle
        Vector3 futurePosition = CalcFuturePosition(time);

        //every 2 seconds, getting a random point on the circle to wander to
        wanderTimer += Time.deltaTime;
        if (wanderTimer >= 2)
        {
            wanderAngle = Random.Range(0, Mathf.PI * 2);
            wanderTimer = 0;
        }

        //calculating the x and y position of the point on the circle
        float x = futurePosition.x + Mathf.Cos(wanderAngle) * radius;
        float y = futurePosition.y + Mathf.Sin(wanderAngle) * radius;

        //seeking the point found
        return Seek(new Vector3(x, y, 0));
    }

    /// <summary>
    /// Calculates the future position of the Agent based on its current
    /// Velocity Vector
    /// </summary>
    /// <param name="time">Amound of frames ahead</param>
    /// <returns>The future position of the Agent</returns>
    protected Vector3 CalcFuturePosition(float time)
    {
        Vector3 futurePosition;

        //getting the future position by getting the current position and
        // adding velecity to it multplied by time
        futurePosition = transform.position + (physObj.Velocity * time);

        return futurePosition;
    }

    /// <summary>
    /// Calculates a force that keeps Agents from going over eachother
    /// </summary>
    /// <returns>the Force that prevents overlapping</returns>
    protected Vector3 Separate()
    {
        //foreach agent found within the 'too close' distance, add to this vector
        Vector3 separateForce = Vector3.zero;

        foreach (Agent agent in fishManager.ActiveFish)
        {
            float distance;

            distance = Vector3.Distance(transform.position, agent.transform.position);

            if (Mathf.Epsilon < distance)
            {
                separateForce += Flee(agent.transform.position) * (separateWeight / distance);
            }

        }

        return separateForce;
    }

    /// <summary>
    /// Agent behavior that avoids obstacles specified by the manager
    /// </summary>
    /// <param name="avoidRange">The range at which the agent will be checking for avoidance</param>
    /// <returns>A force that avoids the obstacles</returns>
    protected Vector3 AvoidObstacle(float avoidRange)
    {
        Vector3 totalAvoidForce = Vector3.zero;

        //clearing the list at the beginning of each method call / checking for null values
        foundObstacles.Clear();

        //Looping through the list and grabbing the position of the
        //  obstacles
        foreach (Obstacle obstacle in fishManager.Obstacles)
        {
            Vector3 agentToObstacle = obstacle.transform.position - transform.position;
            Vector3 futurePos;
            float distance;
            float rightDot;
            float forwardDot;

            //casting AgentToObstacle onto the direction vector
            forwardDot = Vector3.Dot(physObj.Direction, agentToObstacle);

            //calculating the distance of the Agent's view
            futurePos = CalcFuturePosition(avoidRange);
            distance = Vector3.Distance(transform.position, futurePos) + physObj.Radius;

            //checking if the result collids with the forward view of the 
            //  agent avoiding obstacles
            if (forwardDot >= -obstacle.Radius &&
                forwardDot <= distance + obstacle.Radius)
            {
                //Assumes that the agent rotates to look where it is moving
                rightDot = Vector3.Dot(transform.right, agentToObstacle);
                rightDot = Mathf.Abs(rightDot);

                //checking if the obstacle is too far left or right
                if (rightDot <= physObj.Radius + obstacle.Radius)
                {
                    foundObstacles.Add(obstacle.transform.position);

                    //Calculating the steering force
                    if (rightDot >= 0)
                    {
                        //if it is on the right, move left
                        totalAvoidForce += (-transform.right * maxSpeed) *
                            (avoidWeight / distance);
                    }
                    else
                    {
                        //if it is on the left, move right
                        totalAvoidForce += (transform.right * maxSpeed) *
                            (avoidWeight / distance);
                    }
                }
            }
        }


        //return the force to avoid the obstacles
        return totalAvoidForce;
    }

    /// <summary>
    /// calculates a seek force to the center of the flock
    /// </summary>
    /// <returns>The Vector3 force to the center of the flock</returns>
    private Vector3 Cohesion()
    {
        float maxDistance = 5f;
        int inRangeAgents = 0;
        Vector3 cohesionForce = Vector3.zero;

        //summing all of the positions of the fish in the flock
        foreach (Agent agent in fishManager.FlockFish)
        {
            //calculate the distance between this agents position to the other
            // agent positions
            float distance = Vector3.Distance(
                transform.position,
                agent.transform.position);

            //if the distance is less than the max distance
            if (distance < maxDistance)
            {
                //then sum the positions
                cohesionForce += agent.transform.position;

                //increment the number of agents in range
                inRangeAgents++;
            }
        }

        //if there are agents in range
        if (inRangeAgents > 0)
        {
            //divide the sum of all positions by the number of flocking fish
            // to get the average position
            cohesionForce /= inRangeAgents;

            //setting to cohesion point for gizmos
            cohesionPoint = cohesionForce;

            //seek that average position
            return Seek(cohesionForce);
        }
        else
        {
            return Vector3.zero;
        }
    }

    /// <summary>
    /// Determines the average direction of in range agents and 
    /// calculates a force to them to move together
    /// </summary>
    /// <returns>a force that moves together with other aligned agents</returns>
    private Vector3 Alignment()
    {
        float maxDistance = 5f;
        int inRangeAgents = 0;
        Vector3 alignDirection = Vector3.zero;

        //summing all of the normalized velocities
        foreach (Agent agent in fishManager.FlockFish)
        {
            //calculating the distance between this agent and the other agents
            float distance = Vector3.Distance(
                transform.position,
                agent.transform.position);

            //if the agents are in range of each other
            if (distance < maxDistance)
            {
                //sum the velocity of this agent with the summation of the other velocities
                alignDirection += agent.PhysObj.Velocity.normalized;

                //increment the in range agents counter
                inRangeAgents++;
            }
        }

        //if there were agents found in range
        if (inRangeAgents > 0)
        {
            //divide the sum of all positions by number of flocking fish
            // in range to get the average velocity
            alignDirection /= inRangeAgents;

            //normalizing that velocity to get the direction and scaling it by the
            // maxSpeed
            alignDirection *= maxSpeed;

            //gizmos
            alignmentDirection = alignDirection;

            //Calculating the steering force
            return alignDirection - this.PhysObj.Velocity;
        }
        else
        {
            return Vector3.zero;
        }
    }

    /// <summary>
    /// Sums the three parts of the Flock algorithm and returns that flocking force
    /// </summary>
    /// <param name="cohesionWeight">the weight applied to the cohesion portion of the algorithm</param>
    /// <param name="alignWeight">The weight applied to the alignment portion of the algorithm</param>
    /// <returns>The resulting flocking force</returns>
    protected Vector3 Flock(float cohesionWeight, float alignWeight)
    {
        Vector3 flockForce = Vector3.zero;

        //summing all calculated flocking forces and their weights
        flockForce += Cohesion() * cohesionWeight;
        flockForce += Alignment() * alignWeight;
        flockForce += Separate();

        //returning the flock force
        return flockForce;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Gizmos.DrawLine(transform.position - physObj.Velocity.normalized, transform.position);
        //Gizmos.DrawWireSphere(transform.position, 1);

        Gizmos.DrawLine(transform.position, cohesionPoint);


        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(cohesionPoint, alignmentDirection);
    }

}
