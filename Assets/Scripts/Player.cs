using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    // move variables
    public float moveSpeed = 5f;
    public float normalSpeed = 5f;
    public float boostSpeed = 10f;
    public bool isBoosting = false;
    public float turnSpeed = 4.5f;
    public float velocity = 0f;

    private Vector2 movement;
    private Vector2 turn;
    private float angle;

    public Bullet bulletPrefab;

    // health
    public int maxHealth = 500;
    public int currentHealth;

    // mana for boosting
    public float maxMana = 100f;
    public float currentMana;
    public float manaScale = 1f;
    public bool isStartCharging;
    public bool isChargingEnergy;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentMana = 0;
        StartCharging();
        StopChargeEnergy();

        FindObjectOfType<HealthBar>().SetMaxHealth(maxHealth);
        FindObjectOfType<BoostingBar>().SetMaxMana(maxMana);
        FindObjectOfType<BoostingBar>().SetMana(0);
    }

    // Update is called once per frame
    public void Update()
    {
        // input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // immediate charge after done delay
        if (Input.GetKeyUp(KeyCode.LeftShift) && currentMana == 0 && FindObjectOfType<CinemachineVirtualCamera>().m_Lens.OrthographicSize == 5f)
        {
            StartCharging();
        }

        // check boosting
        if (Input.GetKey(KeyCode.LeftShift) && isStartCharging)
        {
            isBoosting = true;
            moveSpeed = boostSpeed;

            if (!isChargingEnergy)
            {
                // using mana
                if (currentMana <= 0)
                {
                    currentMana = 0;
                    isBoosting = false;
                    isStartCharging = false;
                    WaitFor(3f, nameof(StartCharging));
                } else
                {
                    currentMana -= manaScale * 0.7f;
                }
            }
        } else
        {
            isBoosting = false;
            moveSpeed = normalSpeed;

            if (!isChargingEnergy)
            {
                // charging mana
                if (currentMana >= maxMana)
                {
                    currentMana = maxMana;
                }
                else if (isStartCharging)
                {
                    currentMana += manaScale;
                }
            }
        }
        FindObjectOfType<BoostingBar>().SetMana(currentMana);

        // shoot
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        velocity = rb.velocity.magnitude;
    }

    private void FixedUpdate()
    {
        // movement position
        rb.AddForce(movement * moveSpeed);

        // rotate
        turn = Camera.main.ScreenToWorldPoint(Input.mousePosition) - rb.transform.position;
        turn.Normalize();
        angle = Mathf.Atan2(turn.y, turn.x) * Mathf.Rad2Deg - 90;
        rb.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), turnSpeed * Time.fixedDeltaTime);
    }

    private void Shoot()
    {
        Bullet bullet = Instantiate(this.bulletPrefab, this.transform.position, this.transform.rotation);
        bullet.Project(this.transform.up, isBoosting);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // damage with velocity
        int newDamge = (int)Mathf.Round(velocity) + 2;
        TakeDamage(newDamge);
        Vector3 point = collision.contacts[0].point;

        FindObjectOfType<GameManager>().ImpactEffectStart(point);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (FindObjectOfType<GameManager>().collisionEffect.isStopped)
        {
            // damage with velocity
            int newDamge = (int)Mathf.Round(velocity) + 2;
            TakeDamage(newDamge);
            Vector3 point = collision.contacts[0].point;
            FindObjectOfType<GameManager>().ImpactEffectStart(point);
       
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Energy")
        {
             isChargingEnergy = true;
             WaitFor(0.4f, nameof(StopChargeEnergy));
        }
    }

    public void TakeDamage(int newDamage)
    {
        // not control health <= 0 yet
        if (currentHealth - newDamage <= maxHealth)
        {
            currentHealth -= newDamage;
        } else
        {
            currentHealth = maxHealth;
        }
        FindObjectOfType<HealthBar>().SetHealth(currentHealth);
    }

    void WaitFor(float second, string functionName)
    {
        Invoke(functionName, second);
    }

    void StopChargeEnergy()
    {
        isChargingEnergy = false;
    }

    void StartCharging()
    {
        isStartCharging = true;
    } 
}
