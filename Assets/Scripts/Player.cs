using UnityEngine;
using Cinemachine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    // character
    public CharacterDatabase characterDB;
    public SpriteRenderer artworkSprite;
    public int selectedOption = 0;

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
        if (!PlayerPrefs.HasKey("selectedOption"))
        {
            selectedOption = 0;
        }
        else
        {
            Load();
        }

        UpdateCharacter(selectedOption);

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
        // input move
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
                }
                else
                {
                    currentMana -= manaScale * 0.7f;
                }
            }
        }
        else
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
        Color bulletColor = characterDB.GetCharacter(selectedOption).bulletColor;
        bullet.gameObject.GetComponent<SpriteRenderer>().color = bulletColor;
        bullet.Project(this.transform.up, isBoosting, bulletColor);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // damage with velocity
        int newDamage = (int)Mathf.Round(velocity) + 2;
        TakeDamage(newDamage);
        Vector3 point = collision.contacts[0].point;

        FindObjectOfType<GameManager>().ImpactEffectStart(point);
    }

    /*    private void OnCollisionStay2D(Collision2D collision)
        {
            if (FindObjectOfType<GameManager>().collisionEffect.isStopped)
            {
                // damage with velocity
                int newDamage = (int)Mathf.Round(velocity) + 2;
                TakeDamage(newDamage);
                Vector3 point = collision.contacts[0].point;
                FindObjectOfType<GameManager>().ImpactEffectStart(point);

            }
        }*/

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
        if (currentHealth - newDamage <= maxHealth)
        {
            currentHealth -= newDamage;
        }
        else
        {
            currentHealth = maxHealth;
        }

        if (currentHealth <= 0)
        {
            // game over

            // save score
            Save();

            // change to game over scene
            int currentSceneId = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneId + 1);
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

    private void UpdateCharacter(int i)
    {
        Character character = characterDB.GetCharacter(i);
        artworkSprite = Instantiate(character.characterSprite, transform);
        artworkSprite.transform.localScale *= 0.3f;
        artworkSprite.transform.parent = gameObject.transform;

        // scale light
        Light2D[] lights = artworkSprite.gameObject.GetComponentsInChildren<Light2D>();
        lights[0].pointLightInnerRadius *= 0.3f;
        lights[0].pointLightOuterRadius *= 0.3f;
        lights[1].pointLightOuterRadius *= 0.3f;
        lights[1].pointLightInnerRadius *= 0.3f;
        if (lights.Length == 3)
        {
            lights[2].pointLightOuterRadius *= 0.3f;
            lights[2].pointLightInnerRadius *= 0.3f;
        }
    }

    private void Load()
    {
        selectedOption = PlayerPrefs.GetInt("selectedOption");
    }

    private void Save() {
        PlayerPrefs.SetInt("score", FindObjectOfType<GameManager>().score);
    }
}
