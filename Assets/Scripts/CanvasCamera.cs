using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;


public class CanvasCamera : MonoBehaviour
{
    public Camera cam;

    void Start()
    {

    }

    void Update()
    {
        // change to paint mode, all paint constant
        if(Input.GetKeyDown(KeyCode.C))
        {
            if (cam.GetComponent<HDAdditionalCameraData>().clearColorMode == HDAdditionalCameraData.ClearColorMode.Color)
            {
                cam.GetComponent<HDAdditionalCameraData>().clearColorMode = HDAdditionalCameraData.ClearColorMode.None;
            }
            else if (cam.GetComponent<HDAdditionalCameraData>().clearColorMode == HDAdditionalCameraData.ClearColorMode.None)
            {
                cam.GetComponent<HDAdditionalCameraData>().clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;
                cam.GetComponent<HDAdditionalCameraData>().backgroundColorHDR = Color.black;
            }
        }
    }
}