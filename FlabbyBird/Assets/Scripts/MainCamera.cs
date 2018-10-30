using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour {

    public float yOffset = 2;
    public float zOffset = -12;

    // Update is called once per frame
    void Update ()
    {
        transform.LookAt(transform.parent);
        transform.right = Vector3.right;
        transform.position = new Vector3(0, transform.parent.position.y + yOffset, transform.parent.position.z + zOffset);
	}
}
