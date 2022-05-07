using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManage : MonoBehaviour {
	public TextMesh text_fx_name;
	public GameObject[] fx_prefabs;
	int index_fx = 0;

	// Use this for initialization
	void Start () {
		text_fx_name.text = "[" + (index_fx + 1) + "] " + fx_prefabs[ index_fx ].name;
	}
	
	// Update is called once per frame
	void Update () {
		//Change-FX keyboard..	
		if ( Input.GetKeyDown("z") || Input.GetKeyDown("left") ){
			fx_prefabs[ index_fx ].transform.position = new Vector3(-22.0f, fx_prefabs[ index_fx ].transform.position.y, fx_prefabs[ index_fx ].transform.position.z);
			index_fx--;
			if(index_fx <= -1)
				index_fx = fx_prefabs.Length - 1;
			text_fx_name.text = "[" + (index_fx + 1) + "] " + fx_prefabs[ index_fx ].name;
			fx_prefabs[ index_fx ].transform.position = new Vector3(0, fx_prefabs[ index_fx ].transform.position.y, fx_prefabs[ index_fx ].transform.position.z);	
		}

		if ( Input.GetKeyDown("x") || Input.GetKeyDown("right")){
			fx_prefabs[ index_fx ].transform.position = new Vector3(-22.0f, fx_prefabs[ index_fx ].transform.position.y, fx_prefabs[ index_fx ].transform.position.z);
			index_fx++;
			if(index_fx >= fx_prefabs.Length)
				index_fx = 0;
			text_fx_name.text = "[" + (index_fx + 1) + "] " + fx_prefabs[ index_fx ].name;
			fx_prefabs[ index_fx ].transform.position = new Vector3(0, fx_prefabs[ index_fx ].transform.position.y, fx_prefabs[ index_fx ].transform.position.z);
		}
		//Hello theere :)
	}
}
