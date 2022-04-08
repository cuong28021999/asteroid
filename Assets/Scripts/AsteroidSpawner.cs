using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public Asteroid asteroidPrefab;
    public float trajectoryVariance = 15f;
    public float spawnRate = 2f;
    public float spawnDistance = 40f;
    public int spawnAmount = 1;
    private void Start()
    {
        InvokeRepeating(nameof(Spawn), this.spawnRate, this.spawnRate);
    }

    private void Spawn()
    {
        for(int i = 0; i < spawnAmount; i++)
        {
            // random a point direction in a circle
            Vector3 spawnDirection = Random.insideUnitCircle.normalized * this.spawnDistance;
            Vector3 spawnPoint = this.transform.position + spawnDirection; // offset point

            // random rotation
            float variance = Random.Range(-this.trajectoryVariance, this.trajectoryVariance);
            Quaternion rotation = Quaternion.AngleAxis(variance, Vector3.forward); // Vector3.forward = Vector3(0,0,1)

            // init asteroid
            Asteroid asteroid = Instantiate(this.asteroidPrefab, spawnPoint, rotation);
            float size = Random.Range(asteroid.minSize, asteroid.maxSize);
            asteroid.size = size;
            asteroid.HP = (int)Mathf.Round(size/asteroid.maxSize * 100f);
            asteroid.SetTrajectory(rotation * -spawnDirection);
        }
    }
}
