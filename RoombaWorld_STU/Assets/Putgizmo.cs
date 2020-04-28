using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Putgizmo : MonoBehaviour {

    public Mesh plane;
     void Start()
    {
        
    }
    void OnDrawGizmos()
    {
       // Gizmos.DrawMesh(plane,transform.position,Quaternion.identity,new Vector3(5f,5f,0f));
        Gizmos.DrawIcon(transform.position,"cross_3.png",false);
    }
}
