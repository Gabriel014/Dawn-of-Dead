using UnityEngine;
using System.Collections;

public class GhostGenerator : MonoBehaviour {

	public GameObject player;
	public GameObject GhostPrefab;
	public float ghostRate;
	public float zPositioner;
	public float xPositioner;
	public float zRandomizer;
	public float playerMovement = 80;
	public Vector3 positioner;

	void Start () {

		InvokeRepeating ("SpawnGhost", ghostRate, ghostRate);

	}

	void Update () {
	
	}

	void SpawnGhost()
	{
		
		zRandomizer = Random.Range (-2, 2);
		zPositioner = (player.transform.position.z + (zRandomizer * playerMovement));
		xPositioner = (player.transform.position.x - 400f);
		positioner = new Vector3 (xPositioner, 37.5f, zPositioner);
		Instantiate (GhostPrefab, positioner, new Quaternion(1, 0, 1, 0));

	}
}
