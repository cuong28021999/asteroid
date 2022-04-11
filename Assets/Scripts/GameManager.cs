using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ParticleSystem orangeExplosion, blueExplosion, collisionEffect, yellowExplosion;

    public int score;
    public string playerName;

    private void Start()
    {
        score = 0;
        playerName = "Character 01";
    }
    public void OrangeDestroy(Asteroid asteroid)
    {
        this.orangeExplosion.transform.position = asteroid.transform.position;
        this.orangeExplosion.Play();

        // increase score
        score += (int)(asteroid.size / asteroid.maxSize * 100);
    }

    public void BlueDestroy(Bullet bullet)
    {
        this.blueExplosion.transform.position = bullet.transform.position;
        this.blueExplosion.Play();
    }

    public void YellowDestroy(Energy energy)
    {
        this.yellowExplosion.transform.position = energy.transform.position;
        this.yellowExplosion.Play();
    }

    public void ImpactEffectStart(Vector3 position)
    {
        this.collisionEffect.transform.position = position;
        this.collisionEffect.Play();
    }

    public void PlayerDied()
    {

    }
}
