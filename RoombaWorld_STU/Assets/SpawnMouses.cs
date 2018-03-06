using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMouses : MonoBehaviour {
    public GameObject[] spawnPoints;
    public float timer=0;
    public float interval=20f;
    public GameObject mousePrefab;
	// Use this for initialization
	void Start () {
        spawnPoints = GameObject.FindGameObjectsWithTag("EXTERNAL_SPAWNER");
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer > interval)
        {
            Instantiate(mousePrefab, getRandomSpawnerLocation().transform.position, Quaternion.identity);
            timer = 0;
        }
	}
    private GameObject getRandomSpawnerLocation()
    {
        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }
}
