using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class FishManager : MonoBehaviour
{
    [SerializeField]
    private List<Agent> fish;
    [SerializeField]
    private int[] numbersOfFish = { 10, 10, 10};
    [SerializeField]
    private GameObject fishContainer;
    [SerializeField]
    private GameObject obstacleContainer;
    [SerializeField]
    private Obstacle jellyfishObstacle;

    private List<Agent> activeFish;
    private List<Agent> flockFish;
    private List<Agent> wanderFish;

    private List<Obstacle> obstacles;

    private float gameTimer;

    private Agent hunterFish;

    //camera information
    private float height;
    private float width;
    Camera camera;

    public List<Agent> ActiveFish {  get { return activeFish; } }
    public List<Agent> FlockFish {  get { return flockFish; } }
    public List<Agent> WanderFish {  get { return wanderFish; } }
    public List<Obstacle> Obstacles {  get { return obstacles; } }
    public float GameTimer {  get { return gameTimer; } }

    public Agent Hunter { get { return hunterFish; } }


    // Start is called before the first frame update
    void Start()
    {
        //initializing the lists
        activeFish = new List<Agent>();
        flockFish = new List<Agent>();
        wanderFish = new List<Agent>();
        obstacles = new List<Obstacle>();

        //grabbing a reference to the Camera
        camera = Camera.main;

        //calculating the height and width of the screen
        height = 2f * camera.orthographicSize;
        width = height * camera.aspect;

        //give the player 1 minutes to collect as many
        // fish as possible
        gameTimer = 60;

        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        //if the amount of fish are less than what was started with
        //  create up to the difference
        if (wanderFish.Count < numbersOfFish[0])
        {
            //instantiating a new object
            Agent newFish = Instantiate(this.fish[0]);

            //setting the container of the fish clones
            newFish.transform.parent = fishContainer.transform;

            //giving the new fish a reference to the manager
            newFish.FishManager = this;

            //setting a random spawn position for the new Fish
            newFish.transform.position = new Vector3(
                (Random.value * 20),
                newFish.transform.position.y,
                0);

            //Adding the fish to the active list
            activeFish.Add(newFish);

            //adding the fish to the wander fish list
            wanderFish.Add(newFish);
        }
        
        if (flockFish.Count < numbersOfFish[1])
        {
            //instantiating a new object
            Agent newFish = Instantiate(this.fish[1]);

            //setting the container of the fish clones
            newFish.transform.parent = fishContainer.transform;

            //giving the new fish a reference to the manager
            newFish.FishManager = this;

            //setting a random spawn position for the new Fish
            newFish.transform.position = new Vector3(
                (Random.value * 20),
                newFish.transform.position.y,
                0);

            //Adding the fish to the active list
            activeFish.Add(newFish);

            //adding the clown fish to the clown fish flocking list
            flockFish.Add(newFish);
        }

        //Checking the what fish the hunter collided with
        for (int i = activeFish.Count - 1; i > 0; i--)
        {
            //creating a reference
            Agent agent = activeFish[i];

            //moving to the next iteration if it is the hunter fish
            if (agent != hunterFish)
            {
                //calculating the distance between the agents
                float distance = Vector3.Distance(
                    agent.transform.position,
                    hunterFish.transform.position);

                //if there was a collision
                if (distance < (agent.PhysObj.Radius + hunterFish.PhysObj.Radius))
                {
                    //remove the agent from the list
                    activeFish.Remove(agent);

                    //removing the fish in other referenced lists as well
                    if (flockFish.Contains(agent))
                    {
                        //removing the agent from the list
                        flockFish.Remove(agent);
                    }
                    else if (wanderFish.Contains(agent))
                    {
                        //removing the agent from the list
                        wanderFish.Remove(agent);
                    }

                    //destroy the GameObject
                    Destroy(agent.gameObject);
                }
            }
        }


        //decreasing the amount of time that the player has left
        gameTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Spawn fish method for the beginning of games
    /// </summary>
    private void Spawn()
    {
        //Dynamic spawning of active fish in the game
        //spawning the blue fish
        for (int i = 0; i < numbersOfFish[0]; i++)
        {
            //instantiating a new object
            Agent newFish = Instantiate(this.fish[0]);

            //setting the container of the fish clones
            newFish.transform.parent = fishContainer.transform;

            //giving the new fish a reference to the manager
            newFish.FishManager = this;

            //Adding the fish to the active list
            activeFish.Add(newFish);

            //adding the fish to the wander fish list
            wanderFish.Add(newFish);
        }

        //spawning the clownfish
        for (int i = 0; i < numbersOfFish[1]; i++)
        {
            //instantiating a new object
            Agent newFish = Instantiate(this.fish[1]);
        
            //setting the container of the fish clones
            newFish.transform.parent = fishContainer.transform;
        
            //giving the new fish a reference to the manager
            newFish.FishManager = this;
        
            //Adding the fish to the active list
            activeFish.Add(newFish);
            
            //adding the clown fish to the clown fish flocking list
            flockFish.Add(newFish);
        }

        //instantiating obstacles
        //   only ever want 4
        for (int i = 0; i < 4; i++)
        {
            //creating a new obstacle
            Obstacle jellyfish = Instantiate(jellyfishObstacle);

            //randomizing the obstacle positions:
            //   I provide some padding between where they could spawn
            jellyfish.transform.position = new Vector3(
                Random.Range(2, width - 2),
                Random.Range(2, height - 2),
                0);

            //giving it a container
            jellyfish.transform.parent = obstacleContainer.transform;

            //adding it to the list of Jellyfish/obstacles
            obstacles.Add(jellyfish);
        }

        //instantiating the hunter / pathfinding fish
        hunterFish = Instantiate(this.fish[2]);

        //adding it into the fish container
        hunterFish.transform.parent = fishContainer.transform;

        //giving the new fish a reference to the manager
        hunterFish.FishManager = this;

        //Adding the hunter to the list of active fish
        activeFish.Add(hunterFish);
    }
}
