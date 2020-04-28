using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnDust : MonoBehaviour {
    public GameObject dustPrefab;
    public float timer=0f;
    public float inerval = 5; 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer > inerval)
        {
           GameObject go= Instantiate(dustPrefab, RandomLocationGenerator.RandomWalkableLocation(), Quaternion.identity);
            go.GetComponent<SpriteRenderer>().color = Random.ColorHSV();
            timer = 0;
        }
	}
}
