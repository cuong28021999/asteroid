using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 750f;
    public float maxLifetime = 2f;
    public int damage = 20;
    public Color color;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Project(Vector2 direction, bool isBoosting, Color c)
    {
        color = c;

        if (isBoosting)
        {
            rb.AddForce(direction * this.speed * 2f);

        } else
        {
            rb.AddForce(direction * this.speed);
        }

        Destroy(this.gameObject, this.maxLifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnExplosion();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnExplosion();
    }

    private void OnExplosion()
    {
        FindObjectOfType<GameManager>().BlueDestroy(this, color);

        Destroy(this.gameObject);
    }
}
