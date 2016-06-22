using UnityEngine;
using System.Collections;

public class RightCar : MonoBehaviour {
    private GameObject startingPoint;
    private GameObject endingPoint;

    public float speed = 100.0f;
    public float randomizer;

    // Use this for initialization
    void Start () {
        startingPoint = GameObject.Find("ponto_direito");
        endingPoint = GameObject.Find("ponto_eskerdo");	}
	
	// Update is called once per frame
	void Update () {
        float speedRandom = speed + randomizer;

        this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - speedRandom * Time.deltaTime);
        if (this.transform.position.z < endingPoint.transform.position.z)
        {
            this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, startingPoint.transform.position.z);
            randomizer = Random.Range(1, speed);
        }

    }
}
