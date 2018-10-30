using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugSpawner : MonoBehaviour {

    public GameObject bug;
    public Transform bird;

    public float MaxNumBugs = 20;
    public float maxSpawnPerFrame = 5;
    private float numSpawnedBugs = 0;
    public float despawnRange = -30;
    private float xSpawnPos = 20;

    void Start ()
    {
        StartCoroutine(DoMaintainBugs());
	}


    IEnumerator DoMaintainBugs()
    {
        while (true)
        {
            float spawns = 0;
            while ((numSpawnedBugs < MaxNumBugs) && (spawns < maxSpawnPerFrame))
            {
                spawns++;
                SpawnBug();
            }
            yield return null;
        }
    }

    private void SpawnBug()
    {
        float yOffset = Random.Range(-50, 40);
        if (bird.position.y + yOffset < 5) { return; }

        float xOffset = Random.Range(0, 50);
        GameObject newObj = Instantiate(bug, transform);
        Bug newBug = newObj.GetComponent<Bug>();

        newObj.transform.position = new Vector3(xSpawnPos + xOffset, bird.position.y + yOffset, 0);
        newBug.parent = this;
        numSpawnedBugs++;
    }

    public void ChildDespawn()
    {
        numSpawnedBugs--;
    }
}
