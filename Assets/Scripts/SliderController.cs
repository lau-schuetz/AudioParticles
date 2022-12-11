using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    // --------- AUDIO -------------
    // float sync
    private ChuckFloatSyncer ck_tempo;
    public Slider slider;

    // initialize audio
    void InitAudio()
    {
        // run the sequencer
        GetComponent<ChuckSubInstance>().RunFile("PaintSound.ck", true);

        // add the float sync
        ck_tempo = gameObject.AddComponent<ChuckFloatSyncer>();
        ck_tempo.SyncFloat(GetComponent<ChuckSubInstance>(), "ck_tempo");
    }

    public void OnSliderChanged()
    {
        ck_tempo.SetNewValue(slider.value);
    }
}
