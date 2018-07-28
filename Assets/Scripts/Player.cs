using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public float speed;
    public float jetFuel;
    public float jetSpeed;
    public float roll = 0.4f;
    public Rigidbody rb;
    public ParticleSystem hitPoint;
    public LayerMask lm;
    public float weaponRange = 10;
    public Camera cam;
    public SpriteRenderer jet;

	void Start () {
        rb.centerOfMass = new Vector3(0, -0.7f, 0);
	}

    void Update ()
    {

        if (Input.GetMouseButton(0))
        {
            
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            Vector3 dir  = pos - transform.position;
            dir *= 1000000;
            dir.Normalize();
            RaycastHit rh;
            //Physics.Linecast(transform.position, dir * 20, out rh, lm);
            Physics.Raycast(transform.position, dir, out rh,weaponRange , lm);

            if (rh.transform == null) return;
            pos = rh.point;
            
            pos.x += Random.Range(-2, 3)/40f;
            pos.y += Random.Range(-2, 3)/40f;
            
            hitPoint.transform.position = pos + (dir / 20);
            hitPoint.Emit(1);
            
            Data.ter.Dig(hitPoint.transform.position,1);
            
/*
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            Vector3 dir = pos - transform.position;
            dir *= 1000000;
            dir.Normalize();
            RaycastHit rh;
            //Physics.Linecast(transform.position, dir * 20, out rh, lm);
            Physics.Raycast(transform.position, dir, out rh, weaponRange, lm);

            if (rh.transform == null) return;
            Terrain2D t = rh.transform.parent.gameObject.GetComponent<Terrain2D>();
            if (t == null) return;
            pos = rh.point;
            hitPoint.transform.position = pos + (dir / 30);

            pos.x += Random.Range(-2, 3) / 40f;
            pos.y += Random.Range(-2, 3) / 40f;
            t.Dig(hitPoint.transform.position);
			*/
        }


    }

	void FixedUpdate () {

        jet.enabled = false;
        cam.transform.rotation = Quaternion.identity;
        jetFuel += Time.deltaTime;
        jetFuel = Mathf.Clamp(jetFuel, 0, 4);
        float j = 0;
        if (Input.GetButton("Jump"))
        {
            jet.enabled = true;
            jet.transform.localScale = Vector3.one * 1.3f * jetFuel / 4;
            //jetpack.Emit(1);
            j = jetFuel / 4.5f;
            jetFuel -= Time.deltaTime*2;
        }
        //Input.GetAxis("Vertical") * speed
        rb.AddRelativeForce(new Vector3(0, j * jetSpeed, 0),ForceMode.VelocityChange);
        rb.AddRelativeTorque(new Vector3(0, 0, Input.GetAxis("Horizontal") * roll),ForceMode.VelocityChange);
	}
}
