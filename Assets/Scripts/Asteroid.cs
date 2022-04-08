using UnityEngine;

public class Asteroid : MonoBehaviour
{
    public Sprite[] sprites;
    public float size = 1f;
    public float minSize = 0.1f;
    public float maxSize = 0.5f;
    public float speed = 1f;
    public float maxLifetime = 30f;
    public int HP = 100;
    public Energy energyPrefab;

    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private BoxCollider2D bc;


    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        // pick random sprite
        sr.sprite = sprites[Random.Range(0, sprites.Length)];

        // random angle
        this.transform.eulerAngles = new Vector3(0f, 0f, Random.value * 360f);

        // random size and mass
        this.transform.localScale = Vector3.one * this.size; // Vector3.one = Vector3(1,1,1)
        rb.mass = this.size * 3f; // instead of random speed
        bc.isTrigger = true;
    }

    public void SetTrajectory(Vector2 direction)
    {
        rb.AddForce(direction * this.speed);

        Destroy(this.gameObject, this.maxLifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // collision with bullet
        if (collision.gameObject.tag == "Bullet")
        {
            // hp <= 0
            if (HP <= 0)
            {
                // check asteroid split ability
                if (this.size * 0.5f > this.minSize)
                {
                    CreateSplit();
                    CreateSplit();
                }

                FindObjectOfType<Player>().TakeDamage(-20); // +20 hp for player
                FindObjectOfType<GameManager>().OrangeDestroy(this);

                CreateEnergy();

                Destroy(this.gameObject);
            }

            // hp > 0
            // decrease hp
            HP -= FindObjectOfType<Bullet>().damage;
        }
    }

    private void CreateSplit()
    {
        Vector2 position = this.transform.position;
        position += Random.insideUnitCircle * 0.5f;

        Asteroid half = Instantiate(this, position, this.transform.rotation);
        float newSize = this.size * 0.5f;
        half.HP = (int)Mathf.Round(100f * newSize/maxSize);
        half.size = newSize;
        half.SetTrajectory(Random.insideUnitCircle.normalized * this.speed * 8f);
    }

    private void CreateEnergy()
    {
        float percent = Random.Range(0f, 100f);
        if (percent > 70f)
        {
            Instantiate(energyPrefab, this.transform.position, Quaternion.identity);
        }
    }
}
