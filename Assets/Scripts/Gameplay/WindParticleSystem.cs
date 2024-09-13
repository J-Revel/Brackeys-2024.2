using UnityEngine;

public class WindParticleSystem : MonoBehaviour
{
    private ParticleSystem particle_system;

    public int[] spawn_rates = new int[]{0, 10, 20, 50};
    public float[] speeds = new float[]{0, 3, 7, 10};

    private void Start()
    {
        particle_system = GetComponent<ParticleSystem>();

    }
    
    public void UpdateDisplay(int wind_intensity)
    {
        if(particle_system == null)
            particle_system = GetComponent<ParticleSystem>();
        ParticleSystem.EmissionModule wind_emission = particle_system.emission;
        wind_emission.rateOverTime = spawn_rates[wind_intensity];
        ParticleSystem.MainModule main_module = particle_system.main;
        main_module.startSpeed = speeds[wind_intensity];
    }
}