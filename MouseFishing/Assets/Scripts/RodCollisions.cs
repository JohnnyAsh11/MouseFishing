using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.InputSystem;

public class RodCollisions : MonoBehaviour
{
    [SerializeField]
    private FishManager fishManager;
    [SerializeField]
    private RodMovement rod;

    private int score;

    public int Score { get { return score; } }

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //handling collisions
        for (int i = fishManager.ActiveFish.Count - 1; i >= 0; i--)
        {
            //saving a reference to this iteration's agent
            Agent agent = fishManager.ActiveFish[i];

            //making sure the player cant catch the hunter fish
            if (agent == fishManager.Hunter)
            {
                continue;
            }

            //checking for a collision
            if (CircleCollision(agent))
            {
                //remove the agent from the list
                fishManager.ActiveFish.Remove(agent);

                //removing the fish in other referenced lists as well
                if (fishManager.FlockFish.Contains(agent))
                {
                    //removing the agent from the list
                    fishManager.FlockFish.Remove(agent);

                    //because it was a flock fish, increment the score by 5
                    score += 5;
                }
                else if (fishManager.WanderFish.Contains(agent))
                {
                    //removing the agent from the list
                    fishManager.WanderFish.Remove(agent);

                    //because it was a wander fish, increment the score by 10
                    score += 10;
                }

                //destroy the GameObject
                Destroy(agent.gameObject);
            }
        }

        //if the player collides with any jellyfish/obstacles then remove 10 points
        foreach (Obstacle jellyfish in fishManager.Obstacles)
        {
            //checking if a collision occured and that the timer cooldown is up
            if (CircleCollision(jellyfish) && jellyfish.Timer <= 0)
            {
                //getting the obstacle's spriteRenderer
                SpriteRenderer jellyfishRenderer = jellyfish.GetComponent<SpriteRenderer>();

                //tint the jellyfish red
                jellyfishRenderer.color = Color.red;

                //giving the tint timer a second cooldown
                jellyfish.Timer = 1f;

                //giving the player a score deduction
                score -= 20;
            }
        }
    }

    /// <summary>
    /// Finds whether or not a collision occured between a fish and the player's rod
    /// </summary>
    /// <param name="fish">the fish being check against the rod</param>
    /// <returns>whether or not a collision has occured</returns>
    private bool CircleCollision(Agent fish)
    {
        //calculating the distance between the two objects
        float distance = Vector3.Distance(fish.transform.position, transform.position);

        //determining whether or not a collision has occured
        return distance < (fish.PhysObj.Radius + rod.Radius);
    }

    /// <summary>
    /// overload of the circle collisions to check against jellyfish
    /// </summary>
    /// <param name="jellyfish">current obstacle being checked for a collision</param>
    /// <returns>whether or not a collision has occurred</returns>
    private bool CircleCollision(Obstacle jellyfish)
    {
        //calculating the distance between the two objects
        float distance = Vector3.Distance(jellyfish.transform.position, transform.position);

        //determining whether or not a collision has occured
        return distance < (jellyfish.Radius + rod.Radius);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, rod.Radius);
    }
}
