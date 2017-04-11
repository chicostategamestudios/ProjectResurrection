using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMeshCheckpoints : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(GetParentName());
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    IEnumerator GetParentName()
    {
        yield return new WaitForSeconds(0.5f);
        GetComponent<TextMesh>().text = transform.parent.name;
    }
}
