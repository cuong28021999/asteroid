using UnityEngine;
using Cinemachine;
using Mirror;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{
    private Rigidbody2D rb;
    private CinemachineVirtualCamera cinecam;

    [Header("Character")]
    public CharacterDatabase characterDB;
    public SpriteRenderer artworkSprite;

    public Transform firePoint;
    public int selectedOption = 0;

    [Header("Move variables")]
    public float moveSpeed = 5f;
    public float normalSpeed = 5f;
    public float boostSpeed = 10f;
    public bool isBoosting = false;
    public float turnSpeed = 4.5f;
    public float velocity = 0f;

    private Vector2 movement;
    private Vector2 turn;
    private float angle;

    public GameObject bulletPrefab;

    [Header("Health")]
    public int maxHealth = 500;
    public int currentHealth;

    [Header("Boosting")]
    public float maxMana = 100f;
    public float currentMana;
    public float manaScale = 1f;
    public bool isStartCharging;
    public bool isChargingEnergy;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cinecam = FindObjectOfType<CinemachineVirtualCamera>();
        if (!PlayerPrefs.HasKey("selectedOption"))
        {
            selectedOption = 0;
        }
        else
        {
            Load();
        }

    }

    public override void OnStartClient()
    {   
        RespawnPlayer();
        UpdateCharacter(selectedOption);
    }

    public override void OnStartLocalPlayer()
    {
        Debug.Log("OnStartLocalPlayer");

        // // Set isLocalPlayer for this Player in UI for background shading
        // playerUI.SetLocalPlayer();

        // Activate the main panel
        // CanvasUI.instance.mainPanel.gameObject.SetActive(true);
    }

    private void Start()
    {
        if(hasAuthority) {
            cinecam.m_Follow = rb.transform;
            cinecam.m_LookAt = rb.transform;
            
        }
    }

    void RespawnPlayer() {
        currentHealth = maxHealth;
        currentMana = 0;
        StartCharging();
        StopChargeEnergy();

        FindObjectOfType<HealthBar>().SetMaxHealth(maxHealth);
        FindObjectOfType<BoostingBar>().SetMaxMana(maxMana);
        FindObjectOfType<BoostingBar>().SetMana(0);
    }

    // Update is called once per frame
    [Client]
    public void Update()
    {
        if (!hasAuthority) return;
        // input move
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if(isLocalPlayer) {
            // rotate
            turn = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            turn.Normalize();
            angle = Mathf.Atan2(turn.y, turn.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), turnSpeed * Time.fixedDeltaTime);

            // shoot
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                Shoot();
            }
        }

        // immediate charge after done delay
        if (Input.GetKeyUp(KeyCode.LeftShift) && currentMana == 0 && FindObjectOfType<CinemachineVirtualCamera>().m_Lens.OrthographicSize == 5f)
        {
            StartCharging();
        }

        // // check boosting
        // if (Input.GetKey(KeyCode.LeftShift) && isStartCharging)
        // {
        //     isBoosting = true;
        //     moveSpeed = boostSpeed;

        //     if (!isChargingEnergy)
        //     {
        //         // using mana
        //         if (currentMana <= 0)
        //         {
        //             currentMana = 0;
        //             isBoosting = false;
        //             isStartCharging = false;
        //             WaitFor(3f, nameof(StartCharging));
        //         }
        //         else
        //         {
        //             currentMana -= manaScale * 0.7f;
        //         }
        //     }
        // }
        // else
        // {
        //     isBoosting = false;
        //     moveSpeed = normalSpeed;

        //     if (!isChargingEnergy)
        //     {
        //         // charging mana
        //         if (currentMana >= maxMana)
        //         {
        //             currentMana = maxMana;
        //         }
        //         else if (isStartCharging)
        //         {
        //             currentMana += manaScale;
        //         }
        //     }
        // }
        // FindObjectOfType<BoostingBar>().SetMana(currentMana);

        // velocity = rb.velocity.magnitude;
    }

    public void FixedUpdate()
    {
        // only let the local player control the racket.
        // don't control other player's rackets
        if (isLocalPlayer)
            rb.velocity = movement * moveSpeed * Time.fixedDeltaTime;
        
    }

    [Command]
    private void Shoot()
    {
        GameObject bullet = Instantiate(this.bulletPrefab, transform.position, transform.rotation);
        // Color bulletColor = characterDB.GetCharacter(selectedOption).bulletColor;
        // bullet.GetComponent<SpriteRenderer>().color = bulletColor;
        // bullet.GetComponent<Bullet>().Project(this.transform.up, isBoosting, bulletColor);
        NetworkServer.Spawn(bullet);

    }

    [ServerCallback]
    void OnCollisionEnter2D(Collision2D collision)
    {   
        Debug.Log("Player Collision 2D ->" + collision.gameObject.name);
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

    [ServerCallback]
    void OnTriggerEnter2D(Collider2D collision)
    {   
        Debug.Log("OnTriggerEnter2D______PLAYER???");
        // if (collision.gameObject.tag == "Energy")
        // {
        //     isChargingEnergy = true;
        //     WaitFor(0.4f, nameof(StopChargeEnergy));
        // }
        
        if (collision.GetComponent<Bullet>() != null)
        {   
            TakeDamage(collision.GetComponent<Bullet>().damage);
            // currentHealth -= collision.GetComponent<Bullet>().damage;
            if (currentHealth <= 0)
                NetworkServer.Destroy(gameObject);
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
            NetworkServer.Destroy(gameObject);
            // // game over

            // // save score
            // Save();

            // // change to game over scene
            // int currentSceneId = SceneManager.GetActiveScene().buildIndex;
            // SceneManager.LoadScene(currentSceneId + 1);
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
        // Debug.Log()
        Character character = characterDB.GetCharacter(i);
        artworkSprite = Instantiate(character.characterSprite, transform);
        artworkSprite.transform.localScale *= 3f;
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
