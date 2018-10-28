using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class InputControler : MonoBehaviour {

    abstract public float cruiserSpeed { get; }
    abstract public float maxLinearSpeed { get; }
    abstract public float minLinearSpeed { get; }
    abstract public float turnControl { get; set; }
    abstract public float turnBoost { get; protected set; }
    virtual public bool fireToggle { get; protected set; }

    public void SpeedControl()
    {
        if (Input.GetKey(KeyCode.W))
        {
            SetSpeed(maxLinearSpeed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            SetSpeed(minLinearSpeed);
        }
        //If the player is not pressing forward or backward.
        if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            var cruiserSpeed = (maxLinearSpeed - minLinearSpeed) / 2;
            SetSpeed(cruiserSpeed);
        }
    }

    public void FireControl()
    {
        //If the player presses spacebar
        if (Input.GetKey(KeyCode.Space))
        {
            fireToggle = true;
        } else
        {
            fireToggle = false;
        }
    }

    public void SteeringControl()
    {
        //If the player presses D (right)
        if (Input.GetKey(KeyCode.D))
        {
            turnControl += turnBoost * Time.deltaTime;
            if (turnControl > 1)
            {
                turnControl = 1;
            }
        }
        //If the player presses A (left)
        if (Input.GetKey(KeyCode.A))
        {
            turnControl -= turnBoost * Time.deltaTime;
            if (turnControl < -1)
            {
                turnControl = -1;
            }
        }

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            if (turnControl > 0)
            {
                turnControl -= turnBoost * Time.deltaTime;
                if (turnControl < 0)
                {
                    turnControl = 0;
                }
            }
            else
            {
                turnControl += turnBoost * Time.deltaTime;
                if (turnControl > 0)
                {
                    turnControl = 0;
                }
            }
        }
    }

    abstract public void SetSpeed(float newSpeed);
}
