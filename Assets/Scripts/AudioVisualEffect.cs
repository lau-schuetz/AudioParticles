using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.VFX;

namespace UnityEngine.VFX.Utility
{
public class AudioVisualEffect : MonoBehaviour
{
    public VisualEffect VFX;
    static readonly ExposedProperty colorAttribute = "Color";
    public Camera cam;

    // controllable scales waveform, spectrum, radius of particle system
    private float SCALE_WF = 1500.0f;
    //private float SCALE_SP = 1800.0f;       // for microphone input
    private float SCALE_SP = 100.0f;         // for Chunity input
    //private float SCALE_SP_RAD = 1000.0f;   // for microphone input
    private float SCALE_SP_RAD = 2.0f;     // for Chunity input

    // rgb values and Color for spectrum to color mapping
    private float r;
    private float g;
    private float b;
    Color targetColor;
    Color oldColor;
    Color curColor = new Color (0.0f, 0.0f, 0.0f);
    float timeLeft;

    float baseFreq = 0.5f;
    float maxSpectrum = 0f;
    float maxFreq = 0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // -------------- change color with index of spectrum max ---------------- //
        
        // local reference to the audio input spectrum
        float[] sp = ChunityAudioInput.the_spectrum;

        // get max value of spectrum array
        float max_sp = Mathf.Max(sp);
        // get index of max value
        float index = System.Array.IndexOf(sp, max_sp);

        if (timeLeft <= Time.deltaTime)
        {
            // transition complete
            // assign the target color
            VFX.SetVector4(colorAttribute, targetColor);
            oldColor = targetColor;
        
            // map index of highest frequency to a vale between 0 and 1
            // start a new transition
            float scaled_max_sp = ((index - 3) / 8.0f);

            // map float value to color spectrum red to violet
            // if scaled value less than 0 or greater 1, color is black -> clamp
            if (scaled_max_sp < 0.0f)
            {
                getColor(0.0f, 1.0f);
            }
            else if (scaled_max_sp > 1.0f)
            {
                getColor(1.0f, 1.0f);
            }
            else
            {
                getColor(scaled_max_sp, 1.0f);
            }

            // set color of VXF depending on spectrum max, freq max
            targetColor = new Color(r, g, b);
            timeLeft = 0.5f;
        }
        else
        {
            // transition in progress
            // calculate interpolated color
            Color temp = Color.Lerp(oldColor, targetColor, Time.deltaTime / timeLeft);
            VFX.SetVector4(colorAttribute, temp);

            // update the timer
            timeLeft -= Time.deltaTime;
        }
        

        //------------------------ pause mode -------------------------//

        // change between flow and canvas  mode
        if(Input.GetKeyDown(KeyCode.C))
        {
            pauseMode();
        }

    }

    public void pauseMode()
    {
        // change to static canvas mode
        if (cam.GetComponent<HDAdditionalCameraData>().clearColorMode == HDAdditionalCameraData.ClearColorMode.Color)
        {
            cam.GetComponent<HDAdditionalCameraData>().clearColorMode = HDAdditionalCameraData.ClearColorMode.None;
        }
        // switch back to flow mode
        else if(cam.GetComponent<HDAdditionalCameraData>().clearColorMode == HDAdditionalCameraData.ClearColorMode.None)
        {
            cam.GetComponent<HDAdditionalCameraData>().clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;
            cam.GetComponent<HDAdditionalCameraData>().backgroundColorHDR = Color.black;
        }
    }

    // ----------------- color mapping helper function ---------------------- //

    // map spectrum max value to color spectrum from red to violet
    void getColor(float cur_value, float max_value)
    {
        float inc = 6.0f / max_value;
        float x = cur_value * inc;
        r = 0.0f; g = 0.0f; b = 0.0f;
        if ((0 <= x && x <= 1) || (5 <= x && x <= 6)) r = 1.0f;
        else if (4 <= x && x <= 5) r = x - 4;
        else if (1 <= x && x <= 2) r = 1.0f - (x - 1);
        if (1 <= x && x <= 3) g = 1.0f;
        else if (0 <= x && x <= 1) g = x - 0;
        else if (3 <= x && x <= 4) g = 1.0f - (x - 3);
        if (3 <= x && x <= 5) b = 1.0f;
        else if (2 <= x && x <= 3) b = x - 2;
        else if (5 <= x && x <= 6) b = 1.0f - (x - 5);
    }
}
}
