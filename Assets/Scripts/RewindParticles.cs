using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class RewindParticles : MonoBehaviour
{
    ParticleSystem particleSystem;
 
    float simulationTime;
 
    public float startTime = 2.0f;
    public float simulationSpeedScale = 1.0f;
 
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    //}
 
    //void OnEnable()
    //{   
        simulationTime = 0.0f; 
        particleSystem.Simulate(startTime, true, false, true); 
    } 

    void Update() 
    { 
        particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); 

        //bool useAutoRandomSeed = particleSystem.useAutoRandomSeed;
        //particleSystem.useAutoRandomSeed = false;
        particleSystem.Play(false);
    
        float deltaTime = particleSystem.main.useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
        simulationTime -= (deltaTime * particleSystem.main.simulationSpeed) * simulationSpeedScale;
    
        float currentSimulationTime = startTime + simulationTime;
        particleSystem.Simulate(currentSimulationTime, false, false, true);
        Debug.Log("current simulation time: " + currentSimulationTime);
    
        //particleSystem.useAutoRandomSeed = useAutoRandomSeed;
    
        if (currentSimulationTime < 0.0f)
        {
            particleSystem.Play(false);
            particleSystem.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        }
    }
}