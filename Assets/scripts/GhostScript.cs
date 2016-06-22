using UnityEngine;
using System.Collections;

public class GhostScript : MonoBehaviour {

    public GameObject player;
	public int velocity = 120;

	void Start () {

	}
		
	void Update () {

		this.transform.position = new Vector3 (this.transform.position.x + velocity * Time.deltaTime, this.transform.position.y, this.transform.position.z);
	
	}
		
}
