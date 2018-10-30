using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bug : MonoBehaviour {
    
    public BugSpawner parent;
    float flutteringTime = .1f;
    float minFlutterTime = .2f;
    float maxFlutterTime = 2f;
    float minFlutterForce = 6f;
    float maxFlutterForce = 20f;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        StartCoroutine(DoFlutter());
    }

    // Update is called once per frame
    void Update ()
    {
        transform.Translate(GameManager.instance.speed * Time.deltaTime * -1, 0, 0);
        CheckBounds();
	}

    void CheckBounds()
    {
        if (transform.position.x < parent.despawnRange)
        {
            parent.ChildDespawn();
            Destroy(this.gameObject);
        }
    }

    IEnumerator DoFlutter()
    {
        while (true)
        {
            float force = Random.Range(minFlutterForce, maxFlutterForce);
            Vector3 angle = Random.insideUnitCircle;
            StartCoroutine(DoFluttering(force, angle));
            yield return new WaitForSeconds(Random.Range(minFlutterTime, maxFlutterTime));
        }
    }

    IEnumerator DoFluttering(float force, Vector3 angle)
    {
        float time = 0;
        while (time<flutteringTime)
        {
            time += Time.deltaTime;
            rb.AddForce(angle * (force / flutteringTime) * Time.deltaTime);
            yield return null;
        }
        yield return null;
    }
}
