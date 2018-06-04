using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly : MonoBehaviour {

    Rigidbody rb;
    private float flapTime = .2f;
    private float flapPower = 250;
    private bool isFlapping = false;
    private float glidePercentage = .5f;
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (Input.GetKeyDown("mouse 0") && !isFlapping)
        {
            float downVelocity = Mathf.Clamp(Vector3.Dot(Vector3.down, rb.velocity), -25, 25);
            Debug.Log("Speed: " + downVelocity);
            Debug.Log("FlapPower: " + (downVelocity * 15 + flapPower));

            StartCoroutine(Flap(downVelocity));
        }
        else if (Input.GetKey("mouse 0"))
        {
            float scalar = (float)(Time.deltaTime * 9.81f * 59.85 * rb.mass * glidePercentage);
            rb.AddForce(Vector3.up * scalar);
            Debug.Log("adding force: " + scalar);
        }
	}

    IEnumerator Flap(float velocity)
    {
        isFlapping = true;
        float elapsedTime = 0;
        while(elapsedTime<flapTime)
        {
            elapsedTime += Time.deltaTime;
            rb.AddForce(Vector3.up * ((flapPower+ velocity * 15) / flapTime) * Time.deltaTime);

            yield return null;
        }

        rb.AddForce(Vector3.up * ((flapPower + velocity * 15) / flapTime) * (elapsedTime-flapTime));
        isFlapping = false;
        yield return null;

    }
}
