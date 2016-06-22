using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {

    private int x = 5;
    private int y = 0;
    private int z = 0;

	// Update is called once per frame
	void Update () {
        transform.Rotate(x, y, z);
	}
}
