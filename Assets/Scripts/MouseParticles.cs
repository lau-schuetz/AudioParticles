using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseParticles : MonoBehaviour
{
    private Camera canvasCamera;

    // Start is called before the first frame update
    void Start()
    {
        canvasCamera = GameObject.Find("Canvas Camera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // makes the particle system follow the position of the cursor
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // 10 meters in front of the camera:
        transform.position = ray.GetPoint(10);

        // makes the particle system follow the position of the cursor
        //var ray_2 = canvasCamera.ScreenPointToRay(Input.mousePosition);
        // 10 meters in front of the camera:
        //transform.position = ray_2.GetPoint(10);
    }
}
