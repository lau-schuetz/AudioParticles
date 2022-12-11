using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;


[RequireComponent(typeof(ParticleSystem))]
public class AudioParticles : MonoBehaviour
{
    ParticleSystem ps;
    ParticleSystem.ColorOverLifetimeModule colorModule;
    ParticleSystem.ShapeModule shapeModule;

    Gradient ourGradientMin;
    Gradient ourGradientMax;

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

    // --------- AUDIO -------------
    // float sync
    // private ChuckFloatSyncer ck_tempo;
    public Toggle toggle;

    // Start is called before the first frame update
    void Start()
    {
        // Get the system and the emission module.
        ps = GetComponent<ParticleSystem>();
        colorModule = ps.colorOverLifetime;
        shapeModule = ps.shape;

        var main = ps.main;

        rend = brushTip.GetComponent<Renderer>();

        //main.startDelay = 1.0f;
        //main.startLifetime = 8.0f;
        //sizeModule = ps.colorOverLifetime;
        /*
        // A simple 2 color gradient with a fixed alpha of 1.0f.
        float alpha1 = 1.0f;
        ourGradientMin = new Gradient();
        ourGradientMin.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha1, 0.0f), new GradientAlphaKey(alpha1, 1.0f) }
        );

        // A simple 2 color gradient with a fixed alpha of 0.0f.
        float alpha2 = 0.0f;
        ourGradientMax = new Gradient();
        ourGradientMax.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha2, 0.0f), new GradientAlphaKey(alpha2, 1.0f) }
        );

        // Apply the gradients.
        colorModule.color = new ParticleSystem.MinMaxGradient(ourGradientMin, ourGradientMax);

        // In 5 seconds we will modify the gradient.
        Invoke("ModifyGradient", 5.0f);
        */
    }

    // initialize audio
    void InitAudio()
    {
        // run the sequencer
        //GetComponent<ChuckSubInstance>().RunFile("Trial_tempo.ck", true);

        // add the float sync
        //ck_tempo = gameObject.AddComponent<ChuckFloatSyncer>();
        //ck_tempo.SyncFloat(GetComponent<ChuckSubInstance>(), "ck_tempo");
    }



    // Update is called once per frame
    void Update()
    {
        // --------- change particle emission with waveform / volume ---------------
        // local reference to the time domain waveform
        float[] wf = ChunityAudioInput.the_waveform;;
        float max_wf = Mathf.Max(wf);
        float scaled_max_wf = SCALE_WF * max_wf;

        // adjust particle lifetime with waveform maximum, volume maximum
        var main = ps.main;
        // never let the particles stay for longer than 5 seconds
        if (scaled_max_wf < 5.0f)
        {
            main.startLifetime = scaled_max_wf;
            Debug.Log("waveform max: " +  max_wf);
            Debug.Log("scaled max: " +  scaled_max_wf);
        }
        else
        {
            main.startLifetime = 5.0f;
        }

        /*       
        // --------- change particle spread with waveform ---------------
        float scaled_max_wf_spread = Mathf.Pow(max_wf * 11f, 10f);

        shapeModule.radius = scaled_max_wf_spread;
        Debug.Log("scaled max: " +  scaled_max_wf_spread);
        */

        // --------- change color with spectrum max ---------------
        /*
        // local reference to the audio input spectrum
        float[] sp = ChunityAudioInput.the_spectrum;
        // reference to particle system main
        var main = ps.main;
        // get max value of spectrum array
        float max_sp = Mathf.Max(sp);
        Debug.Log("spectrum max: " +  max_sp);

        if (timeLeft <= Time.deltaTime)
        {
            // transition complete
            // assign the target color
            if (erase == false)
            {
                main.startColor = targetColor;
            }
        
            // start a new transition
            float scaled_max_sp = SCALE_SP * max_sp;
            Debug.Log("scaled spectrum max: " +  scaled_max_sp);
            // map float value to color spectrum red to violet
            getColor(scaled_max_sp, 1.0f);
            Debug.Log("r: " + r);
            // set color of particle system depending on spectrum max, freq max
            targetColor = new Color(r, g, b);
            timeLeft = 0.5f;
        }
        else
        {
            // transition in progress
            // calculate interpolated color
            //colorModule.color = new ParticleSystem.MinMaxGradient(curColor, targetColor);
            if (erase == false)
            {
                main.startColor = Color.Lerp(main.startColor.color, targetColor, Time.deltaTime / timeLeft);
            }
            // update the timer
            timeLeft -= Time.deltaTime;
        }
        */

        // --------- change color with index of spectrum max ---------------
        
        // local reference to the audio input spectrum
        float[] sp = ChunityAudioInput.the_spectrum;

        // reference to particle system main
        //var main = ps.main;
        // get max value of spectrum array
        float max_sp = Mathf.Max(sp);
        // get index of max value
        float index = System.Array.IndexOf(sp, max_sp);
        Debug.Log("index: " +  index);

        if (timeLeft <= Time.deltaTime)
        {
            // transition complete
            // assign the target color
            if (erase == false)
            {
                main.startColor = targetColor;
            }

            // map index of highest frequency to a vale between 0 and 1
            // index between 0 and 30 sound pleasant
            // start a new transition
            float scaled_max_sp = ((index - 3) / 8.0f);
            Debug.Log("scaled spectrum max: " +  scaled_max_sp);
            // map float value to color spectrum red to violet
            getColor(scaled_max_sp, 1.0f);
            Debug.Log("r: " + r);
            // set color of particle system depending on spectrum max, freq max
            targetColor = new Color(r, g, b);
            timeLeft = 0.5f;
        }
        else
        {
            // transition in progress
            // calculate interpolated color
            //colorModule.color = new ParticleSystem.MinMaxGradient(curColor, targetColor);
            if (erase == false)
            {
                main.startColor = Color.Lerp(main.startColor.color, targetColor, Time.deltaTime / timeLeft);
            }
            // update the timer
            timeLeft -= Time.deltaTime;
        }
        

        /*
        // local refernce to the spectrum
        float[] sp = ChunityAudioInput.the_spectrum;
        // get max value of spectrum array
        float max_sp = Mathf.Max(sp);
        Debug.Log("spectrum max: " +  max_sp);
        float scaled_max_sp = SCALE_SP * max_sp;
        Debug.Log("scaled spectrum max: " +  scaled_max_sp);
        getColor(scaled_max_sp, 1.0f);

        // set color of particle system depending on spectrum max, freq max
        var main = ps.main;
        main.startColor = new Color(r, g, b);
        // spectrum_history[row, col].GetComponent<Renderer>().material.SetColor("_BaseColor", colors[col]);
        */

        // --------- change radius with spectrum ---------------
        // float new_radius = SCALE_SP_RAD * max_sp; <----- works well! but little change
        //float new_radius = Mathf.Pow((SCALE_SP_RAD * Mathf.Sqrt(max_sp * 20f)), 2f);
        //shapeModule.radius = new_radius;
        //Debug.Log("new radius: " + new_radius);

        // --------- change audio tempo with input from Unity ---------------
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {   
            GetComponent<ChuckSubInstance>().BroadcastEvent("changeTempoUp");
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {   
            GetComponent<ChuckSubInstance>().BroadcastEvent("changeTempoDown");
        }

        // --------- change audio volume / gain with input from Unity ---------------
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

        // --------- change audio frequency with input from Unity ---------------
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

        // change between flow and canvas  mode
        if(Input.GetKeyDown(KeyCode.C))
        {
            // change to static canvas mode
            if (cam.GetComponent<HDAdditionalCameraData>().clearColorMode == HDAdditionalCameraData.ClearColorMode.Color)
            {
                cam.GetComponent<HDAdditionalCameraData>().clearColorMode = HDAdditionalCameraData.ClearColorMode.None;
                // change the music to slow mode
                GetComponent<ChuckSubInstance>().BroadcastEvent("dreamOn");

            }
            // switch back to flow mode
            else if (cam.GetComponent<HDAdditionalCameraData>().clearColorMode == HDAdditionalCameraData.ClearColorMode.None)
            {
                cam.GetComponent<HDAdditionalCameraData>().clearColorMode = HDAdditionalCameraData.ClearColorMode.Color;
                cam.GetComponent<HDAdditionalCameraData>().backgroundColorHDR = Color.black;
                erase = false;
                // change music back to normal mode
                GetComponent<ChuckSubInstance>().BroadcastEvent("dreamOff");
            }
        }

    }

     void ModifyGradient()
    {
        // Reduce the alpha
        float alpha = 0.5f;
        ourGradientMin.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.green, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0.0f), new GradientAlphaKey(alpha, 1.0f) }
        );

        // Apply the changed gradients.
        colorModule.color = new ParticleSystem.MinMaxGradient(ourGradientMin, ourGradientMax);
    }

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
