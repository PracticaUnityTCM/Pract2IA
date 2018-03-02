using UnityEngine;

public class ClickSpawn : MonoBehaviour {

    private Camera cam;
    private GameObject pooPrefab;
    private GameObject dustPrefab;
    private GameObject mousePrefab;

	// Use this for initialization
	void Start () {
        cam = Camera.main;
        pooPrefab = Resources.Load<GameObject>("POO");
        dustPrefab = Resources.Load<GameObject>("DUST");
        mousePrefab = Resources.Load<GameObject>("MOUSE");
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 position;

        if (Input.GetMouseButtonDown(0))
        {
            position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            GameObject dust = GameObject.Instantiate(dustPrefab);
            dust.transform.position = position;
        }

        if (Input.GetMouseButtonDown(1))
        {
            position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            GameObject poo = GameObject.Instantiate(pooPrefab);
            poo.transform.position = position;
        }

        if (Input.GetMouseButtonDown(2))
        {
            position = cam.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            GameObject mouse = GameObject.Instantiate(mousePrefab);
            mouse.transform.position = position;
        }

    }
}
