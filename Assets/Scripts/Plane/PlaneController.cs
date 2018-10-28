﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneController : InputControler
{

    public bool controlledByPlayer;

    public float speed;
    public float turnSpeed = 90.0f;
    public float boost = 0.8f;
    public float airBrakes = 0.5f;
    public float patrolRadius = 20.0f;

    public Vector2 position;
    public Vector2 velocity;

    public ParticleSystem jetTrace;

    public float maxSpeed = 3.0f;
    public float minSpeed = 0.2f;

    public float followDistance = 10.0f;

    private float _turnBoost = 1.0f;
    private float minSoundPitch = 1.0f;
    private float maxSoundPitch = 1.4f;
    private float followThreshhold = 0.2f;
    private float fuelBurnRate = 1.0f;
    private float fuel = 1.0f;

    private bool landed;
    private Waypoint currentWaypoint;

    private float _turnControl;

    public PlaneController currentTarget;

    public override float cruiserSpeed
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public override float maxLinearSpeed {
        get
        {
            return maxSpeed;
        }
    }

    public override float minLinearSpeed {
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

    public override float turnBoost
    {
        get
        {
            return _turnBoost;
        }

        protected set
        {
            _turnBoost = value;
        }
    }

    public void SetTarget(PlaneController target)
    {
        currentTarget = target;
    }

    // Use this for initialization
    void Start()
    {
        position = transform.position;
    }

    private void FixedUpdate()
    {
        var rigidbody2D = GetComponent<Rigidbody2D>();
        //rigidbody2D.angularVelocity = -turnControl * turnSpeed;
        //rigidbody2D.velocity = transform.right * speed;

        //var direction = transform.worldToLocalMatrix.MultiplyVector(transform.right);
        //rigidbody2D.AddForce(transform.TransformDirection(Vector2.right));

        //rigidbody2D.velocity = Vector2.ClampMagnitude(rigidbody2D.velocity, maxLinearSpeed);
        rigidbody2D.velocity = transform.right * speed;


    }

    void Update()
    {
        if (controlledByPlayer)
        {
            SpeedControl();
            SteeringControl();
        } else
        {
            if (currentTarget)
            {
                FollowTarget(currentTarget);
            }
            else
            {
                PatrolCarrier();
            }
        }

        UpdateSprite();
        UpdateSound();

        var rigidbody2D = GetComponent<Rigidbody2D>();

        position = transform.position;
        Debug.DrawRay(position, rigidbody2D.velocity, Color.magenta);
        fuel -= fuelBurnRate * Time.deltaTime;
        var emission = jetTrace.emission;
        emission.enabled = !landed;
    }

    void PatrolCarrier()
    {
        if (fuel <= 0)
        {
            Land();
        }
    }

    void Land()
    {
        if (currentWaypoint)
        {
            MoveToWaypoint(currentWaypoint);
        } else
        {
            MoveToWaypoint(GameManager.instance.landingPoint);
        }
    }

    void MoveToWaypoint(Waypoint waypoint)
    {
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();
        float waypointSpeed = waypoint.recommendedSpeed == 0 ? maxLinearSpeed : waypoint.recommendedSpeed;
        Vector2 direction = (waypoint.position - transform.position).normalized;
        Debug.DrawRay(position, direction, Color.blue);
        Vector3 x = Vector3.Cross(direction, transform.right);
        float rotateAmount = Vector3.Cross(direction, transform.right).z;
        rigidbody.angularVelocity = -turnSpeed * rotateAmount;
        SetSpeed(waypointSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var waypoint = collision.gameObject.GetComponent<Waypoint>();
        if (waypoint)
        {
            if (currentWaypoint)
            {
                if (currentWaypoint.landing)
                {
                    landed = true;
                    AudioSource audio = collision.gameObject.GetComponent<AudioSource>();
                    if (audio)
                    {
                        audio.Play();
                    }
                    transform.parent = GameManager.instance.carrier.gameObject.transform;
                }
                if (currentWaypoint.takeoff)
                {
                    landed = false;
                    transform.parent = null;
                }
            }
            currentWaypoint = waypoint.nextWaypoint;
        }
    }

    void FollowTarget(PlaneController target)
    {
        float dist = Vector3.Distance(target.position, transform.position);
        Vector2 direction = (target.position + target.velocity) - position;
        Debug.DrawRay(position, direction, Color.blue);
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.right).z;
        turnControl = rotateAmount * turnSpeed * Time.deltaTime;

        if (dist > followDistance)
        { // S = V0*t+(a*t^2)/2
            var desiredSpeed = Mathf.Sqrt(2 * airBrakes * dist);
            SetSpeed(desiredSpeed);
        } else
        {
            if (dist < followDistance * (1 - followThreshhold))
            {
                SetSpeed(minLinearSpeed);
            } else
            {
                SetSpeed(target.speed);
            }
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
            speed -= airBrakes * Time.deltaTime;
            if (speed < minLinearSpeed)
            {
                speed = minLinearSpeed;
            }
        }
    }

    void UpdateSprite()
    {
        if (landed)
        {
            setSprite(GameManager.instance.straightSprite);
            return;
        }
        if (turnControl > 0.33f && turnControl < 0.66f) // slightly right
        {
            setSprite(GameManager.instance.rightSprite0);
        }
        else if (turnControl >= 0.66f) // right
        {
            setSprite(GameManager.instance.rightSprite1);
        }
        else if (turnControl < -0.33f && turnControl > -0.66f) // slightly left
        {
            setSprite(GameManager.instance.leftSprite0);
        }
        else if (turnControl <= -0.66f) // left
        {
            setSprite(GameManager.instance.leftSprite1);
        }
        else // straight
        {
            setSprite(GameManager.instance.straightSprite);
        }
    }

    void UpdateSound()
    {
        float thrust = speed / (maxLinearSpeed - minLinearSpeed);
        float pitchRange = maxSoundPitch - minSoundPitch;
        var pitch = pitchRange * thrust;
        var audio = GetComponent<AudioSource>();
        audio.pitch = minSoundPitch + pitch;
    }

    private void setSprite(Sprite sprite)
    {
        var renderer = GetComponent<SpriteRenderer>();
        if (renderer.sprite != sprite)
        {
            renderer.sprite = sprite;
        }
    }
}