using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;


[RequireComponent(typeof(ParticleSystem))]
public class AudioParticles : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.ColorOverLifetimeModule colorModule;
    ParticleSystem.ShapeModule shapeModule;

    public Scene tutorialScene;
    Scene currentScene;
    
    public VisualEffect VFX;
    public Camera cam;
    public GameObject brushTip;
    private Renderer rend;

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
    Color curColor = new Color (0.0f, 0.0f, 0.0f);
    float timeLeft;

    float baseFreq = 0.5f;
    float maxSpectrum = 0f;
    float maxFreq = 0f;

    bool erase = false;

    // Start is called before the first frame update
    void Start()
    {
        // get the particle system and the main, color and shape module
        ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        colorModule = ps.colorOverLifetime;
        shapeModule = ps.shape;

        // get sphere at cursor and current scene
        rend = brushTip.GetComponent<Renderer>();
        currentScene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    void Update()
    {
        // --------- change particle emission with waveform / volume --------------- //
        
        // local reference to the time domain waveform
        float[] wf = ChunityAudioInput.the_waveform;
        float max_wf = Mathf.Max(wf);
        float scaled_max_wf = SCALE_WF * max_wf;

        // adjust particle lifetime with waveform maximum, volume maximum
        var main = ps.main;
        // never let the particles stay for longer than 5 seconds
        if (scaled_max_wf < 5.0f)
        {
            main.startLifetime = scaled_max_wf;
        }
        else
        {
            main.startLifetime = 5.0f;
        }

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
            if (erase == false)
            {
                main.startColor = targetColor;
            }

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

            // set color of particle system depending on spectrum max, freq max
            targetColor = new Color(r, g, b);
            timeLeft = 0.5f;
        }
        else
        {
            // transition in progress
            // calculate interpolated color
            if (erase == false)
            {
                main.startColor = Color.Lerp(main.startColor.color, targetColor, Time.deltaTime / timeLeft);
            }
            // update the timer
            timeLeft -= Time.deltaTime;
        }
        
        // --------------- change audio beat / tempo with input from Unity -------------- //

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {   
            GetComponent<ChuckSubInstance>().BroadcastEvent("changeTempoUp");
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {   
            GetComponent<ChuckSubInstance>().BroadcastEvent("changeTempoDown");
        }

        // ---------------- change audio gain with input from Unity ------------------- //
       
        if (Input.GetKeyDown(KeyCode.W))
        {   
            GetComponent<ChuckSubInstance>().BroadcastEvent("changeGainUp");
            shapeModule.radius += 0.15f;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {   
            GetComponent<ChuckSubInstance>().BroadcastEvent("changeGainDown");
            shapeModule.radius -= 0.15f;
        }

        // --------- change audio frequency with input from Unity --------------- //
        
        // scale base frequency betqeen 45 and 71, start at 55
        if (Input.GetKeyDown(KeyCode.D))
        {   
            GetComponent<ChuckSubInstance>().BroadcastEvent("changeFreqUp");
        }
        if (Input.GetKeyDown(KeyCode.A))
        {   
            GetComponent<ChuckSubInstance>().BroadcastEvent("changeFreqDown");
        }

        // make background change color instead of brush when in pause mode
        if(Input.GetKeyDown(KeyCode.Y))
        {
            if (erase == false && cam.GetComponent<HDAdditionalCameraData>().clearColorMode == HDAdditionalCameraData.ClearColorMode.None)
            {
                var main_2 = ps.main;
                main_2.startColor = Color.black;
                erase = true;
            }
            else if (erase == true)
            {
                var main_2 = ps.main;
                main_2.startColor = targetColor;
                erase = false;
            }
        }

        //------------------------ pause mode -------------------------//

        // change between flow and canvas  mode
        if(Input.GetKeyDown(KeyCode.C))
        {
            // change to static canvas mode
            if (cam.GetComponent<HDAdditionalCameraData>().clearColorMode == HDAdditionalCameraData.ClearColorMode.Color)
            {
                // if in tutorial scene, disable particle system
                if(currentScene.name == "TutorialScene")
                {
                    VFX.Reinit();
                    VFX.Stop();
                }
                cam.GetComponent<HDAdditionalCameraData>().clearColorMode = HDAdditionalCameraData.ClearColorMode.None;
                // change audioc to slow mode
                GetComponent<ChuckSubInstance>().BroadcastEvent("dreamOn");
            }
            // switch back to flow mode
            else if(cam.GetComponent<HDAdditionalCameraData>().clearColorMode == HDAdditionalCameraData.ClearColorMode.None)
            {
                // re-enable visual effect of particles
                if(currentScene.name == "TutorialScene")
                {
                    VFX.Play();
                }
                cam.GetComponent<HDAdditionalCameraData>().clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;
                cam.GetComponent<HDAdditionalCameraData>().backgroundColorHDR = Color.black;
                erase = false;
                // change music back to normal mode
                GetComponent<ChuckSubInstance>().BroadcastEvent("dreamOff");
            }
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
