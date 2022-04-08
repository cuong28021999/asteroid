using UnityEngine;

public class GameManager : MonoBehaviour
{
    public ParticleSystem orangeExplosion, blueExplosion, collisionEffect, yellowExplosion;

    public int score = 0;
    public void OrangeDestroy(Asteroid asteroid)
    {
        this.orangeExplosion.transform.position = asteroid.transform.position;
        this.orangeExplosion.Play();
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
