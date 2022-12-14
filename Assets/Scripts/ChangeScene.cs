using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class ChangeScene : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            SceneManager.LoadScene("SecretScene"); 
        }
    }
    public void AudioPaint() {  
        SceneManager.LoadScene("AudioParticles");  
    }
    public void secretScene() {  
        SceneManager.LoadScene("SecretScene");
    }
}
