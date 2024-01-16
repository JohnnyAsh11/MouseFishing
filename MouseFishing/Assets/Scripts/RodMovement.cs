using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem;

public class RodMovement : MonoBehaviour
{
    private Vector3 mousePos;

    [SerializeField]
    private float radius;

    public Vector3 MousePosition { get { return mousePos; } }
    public float Radius { get { return radius; } }


    // Start is called before the first frame update
    void Start()
    {
        mousePos = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //getting the mouse's position
        mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mousePos.z = 0;

        //altering the position values to fit the sprite being used
        //mousePos.y += 5.2f;

        //setting the rod's position to be the where the mouse is
        transform.position = mousePos;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //
    //    Gizmos.DrawWireSphere(mousePos, radius);
    //}
}
