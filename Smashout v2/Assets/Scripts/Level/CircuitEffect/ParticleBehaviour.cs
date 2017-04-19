using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleBehaviour : MonoBehaviour {
    ParticleSystem system;
    ParticleSystem.Particle[] particles;
    public float volatility;
    public float updateIncrement;
    public float[] angleChanges = { -90.0f, -45.0f, 45.0f, 90.0f };
    private float checkpoint;

    void Awake()
    {
        checkpoint = 0;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        InitializeIfNeeded();
        int numParticlesAlive = system.GetParticles(particles);
        for (int i = 0; i < numParticlesAlive; ++i)
        {
            float snapAngle = -Mathf.Atan2(particles[i].velocity.x, particles[i].velocity.y) * Mathf.Rad2Deg + 90.0f;
            snapAngle = Mathf.Round(snapAngle / 45.0f) * 45.0f;

            particles[i].velocity = Quaternion.AngleAxis(snapAngle, Vector3.forward) * Vector3.up;
            Debug.Log(particles[i].velocity.ToString());
        }

        checkpoint += Time.deltaTime;
        if (checkpoint >= updateIncrement)
        {
            while (checkpoint >= updateIncrement) checkpoint -= updateIncrement;

            for (int i = 0; i < numParticlesAlive; ++i)
            {
                if (Random.value <= volatility)
                {
                    particles[i].velocity = Quaternion.AngleAxis(angleChanges[Random.Range(0, 4)], Vector3.forward) * particles[i].velocity;
                }
            }
        }
        system.SetParticles(particles, numParticlesAlive);
	}

    void InitializeIfNeeded()
    {
        if (system == null)
            system = GetComponent<ParticleSystem>();

        if (particles == null || particles.Length < system.main.maxParticles)
            particles = new ParticleSystem.Particle[system.main.maxParticles];
    }
}
