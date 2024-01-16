using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum FishType
{
    BlueFish,
    ClownFish
}

public class PhysicsObject : MonoBehaviour
{
    [SerializeField]
    private float maxSpeed = 10f;

    [SerializeField]
    private float mass = 1f;

    [SerializeField]
    private float radius = 1f;

    [SerializeField]
    private FishType fishType;

    private Vector3 acceleration;
    private Vector3 velocity;
    private Vector3 position;

    private Vector3 direction;

    public Vector3 Velocity { get { return velocity; } } 

    public FishType FishType { get { return fishType; } }
    public float Radius { get { return radius; } }

    public Vector3 Direction { get { return direction; } }

    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        acceleration = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //calculating velocity
        velocity += acceleration * Time.deltaTime;

        //clamping the velocity to prevent crazy speeds
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        //calculating the new position and setting it
        position += velocity * Time.deltaTime;
        transform.position = position;

        //getting the direction with the normalized velocity
        direction = velocity.normalized;

        // zeroing out acceleration for the next PhysObj Update call
        acceleration = Vector3.zero;
    }

    /// <summary>
    /// Calculates the force based off of Newton's Second Law
    /// </summary>
    /// <param name="force">Vector3 force being applied to the PhysObj</param>
    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }
}
