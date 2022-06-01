using UnityEngine;
using Mirror;

public class Bullet : NetworkBehaviour
{
    public float speed = 750f;
    public float maxLifetime = 2f;
    public int damage = 20;

    [SyncVar]
    public uint ownerNetId;

    public Color color;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetOwnerNetId(uint ownerNetId)
    {
        this.ownerNetId = ownerNetId;
    }

    public override void OnStartServer()
    {
        
        Invoke(nameof(DestroySelf), maxLifetime);
    }

    // set velocity for server and client. this way we don't have to sync the
    // position, because both the server and the client simulate it.
    void Start()
    {
        if(NetworkServer.active) {
            rb.AddForce(transform.up * speed);
        } else if(NetworkClient.active) {
            NetworkIdentity networkIdentity = NetworkClient.connection.identity;
            Player player = networkIdentity.GetComponent<Player>();
            // transform.position = player.transform;
            if(ownerNetId == player.netId) {
                transform.position = player.transform.position;
            }
            rb.AddForce(transform.up * speed);
        }
    }

    // destroy for everyone on the server
    [ServerCallback]
    void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    // ServerCallback because we don't want a warning
    // if OnTriggerEnter is called on the client
    [ServerCallback]
    void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            if (player.netId != ownerNetId && player.isPlaying)
            {
                DestroySelf();
            }
        } else {
            DestroySelf();
        }
    }
}
