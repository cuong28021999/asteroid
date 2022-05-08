using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
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

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), maxLifetime);
    }

    // set velocity for server and client. this way we don't have to sync the
    // position, because both the server and the client simulate it.
    void Start()
    {   
        rb.AddForce(transform.up * speed);
    }

    // destroy for everyone on the server
    [Server]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    // ServerCallback because we don't want a warning
    // if OnTriggerEnter is called on the client
    [ServerCallback]
    void OnTriggerEnter2D(Collider2D co) => DestroySelf();
}
