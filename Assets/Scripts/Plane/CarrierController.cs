using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrierController : InputControler
{
    public bool playerControlEnabled;
    public Vector2 position;
    public Vector2 velocity;
    public float turnSpeed = 3.0f;

    public float speed;
    public float boost = 0.2f;
    public float maxSpeed = 0.5f;
    public float minSpeed = 0;

    private float _turnBoost = 0.2f;
    private float _turnControl;

    public override float cruiserSpeed
    {
        get
        {
            return 0;
        }
    }

    public override float maxLinearSpeed {
        get
        {
            return maxSpeed;
        }
    }   

    public override float minLinearSpeed
    {
        get
        {
            return minSpeed;
        }
    }

    public override float turnControl {
        get
        {
            return _turnControl;
        }

        set
        {
            _turnControl = value;
        }
    }
    public override float turnBoost {
        get
        {
            return _turnBoost;
        }

        protected set
        {
            _turnBoost = value;
        }
    }

    public override void SetSpeed(float newSpeed)
    {
        if (speed < newSpeed)
        {
            speed += boost * Time.deltaTime;
            if (speed > maxLinearSpeed)
            {
                speed = maxLinearSpeed;
            }
        }
        else
        {
            speed -= boost * Time.deltaTime;
            if (speed < minLinearSpeed)
            {
                speed = minLinearSpeed;
            }
        }
    }

    // Use this for initialization
    void Start () {
		
	}

    private void FixedUpdate()
    {
        var rigidbody2D = GetComponent<Rigidbody2D>();
        velocity = rigidbody2D.velocity;

        if (velocity.magnitude > maxLinearSpeed)
        {
            velocity = velocity.normalized;
            velocity *= maxLinearSpeed;
        }

        rigidbody2D.angularVelocity = -turnControl * turnSpeed;
        rigidbody2D.velocity = transform.right * speed;
    }

    // Update is called once per frame
    void Update () {
		if (playerControlEnabled)
        {
            SpeedControl();
            SteeringControl();
        }
	}
}
