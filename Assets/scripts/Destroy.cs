using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour
{
    void Update()
    {
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        if (screenPosition.y+(Screen.height*1.1) < 0)
            Destroy(this.gameObject);
    }
}