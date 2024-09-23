using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class RotateImage : MonoBehaviour
{
    private bool rotating;              // Check if rotation is in progress
    private float targetAngle = 0f;     // The angle that needs to be reached by rotating

    
    // By showing a partial rotation each frame, the rotating becomes an animation. 
    void Update()
    {
        if (rotating)
        {
            // Calculate the new rotation step
            float stepSize = 200f * Time.deltaTime; // 200f is the speed of the rotation in degrees per second
            float newRotation = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, stepSize); // Calculating the partial rotation wrt the targetAngle

            // Apply the (partial) rotation
            transform.eulerAngles = new Vector3(0f, 0f, newRotation);

            // Check if image has rotated the desired 180 degrees
            if (Mathf.Abs(newRotation - targetAngle) < 0.01f) // 0.01f because of rounding errors
            {
                rotating = false; // Stop rotating
            }
        }
    }

    // This method is called when clicked on the 'Next' button
    public void Rotate()
    {
        // By adding this if-statement, nothing happens when the button is pressed during a rotation. 
        if (!rotating)
        {
            // Calculate the new target rotation by adding 180 degrees to the current rotation
            targetAngle = transform.eulerAngles.z + 180f;
            rotating = true;
        }
    }
}
