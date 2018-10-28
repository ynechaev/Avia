using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {

    public float recommendedSpeed;
    public bool landing;
    public bool taxiway;
    public bool takeoff;
    public Waypoint nextWaypoint;
    public Vector3 position;

	// Use this for initialization
	void Start () {
        position = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        position = transform.position;
    }
}
