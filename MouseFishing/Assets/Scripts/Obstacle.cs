using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    private float radius;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    private float timer;

    public float Radius { get { return radius; } }
    public float Timer 
    { 
        get { return timer; } 
        set { timer = value; } 
    }

    void Update()
    {
        //if the timer is up then the player collided with the jellyfish
        if (timer > 0)
        {
            //subtract from the cooldown timer
            timer -= Time.deltaTime;
        }
        else
        {
            //when the timer is done, return the color to normal
            spriteRenderer.color = Color.white;
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
