using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour {

    public float lifeTimeMax = 2;
    public bool firing;
    private float lifeTime;


	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        if (firing)
        {
            TrailRenderer trail = GetComponent<TrailRenderer>();
            trail.enabled = true;
            lifeTime += Time.deltaTime;
            if (lifeTime > lifeTimeMax)
            {
                Destroy(gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.MovePosition(transform.position + transform.right * Time.deltaTime * 20);

    }

    public void Fire(ProjectileScript sender, Vector2 initialVelocity)
    {
        ProjectileScript clone = (ProjectileScript)Instantiate(sender, transform.position, transform.rotation);
        clone.firing = true;
    }
}
