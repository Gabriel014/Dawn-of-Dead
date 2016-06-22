using UnityEngine;
using System.Collections;

public class Destroyer : MonoBehaviour {

	public GameObject player;
	public float destroyDistance = 160;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if ((player.transform.position.x + destroyDistance) < this.transform.position.x) {
			Destroy (this);
		}

	}
		
}
