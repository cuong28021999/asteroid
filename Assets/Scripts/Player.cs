using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.Experimental.Rendering.Universal;

public class Player : NetworkBehaviour
{
    private Rigidbody2D rb;
    private CinemachineVirtualCamera cinecam;

    public ParticleSystem collisionEffect;
    public ParticleSystem explosionEffect;
    public ParticleSystem spawnEffect;

    [Header("Character")]
    public CharacterDatabase characterDB;
    public SpriteRenderer artworkSprite;

    [SyncVar(hook = nameof(OnPlayerNameChanged))]
    private string strPlayerName = "";
    [SyncVar]
    public int selectedOption = 0;

    [Header("Move variables")]
    public float moveSpeed = 5f;
    public float turnSpeed = 0.5f;
    public float velocity = 0f;
    public float delayRespawn = 5f;

    [SyncVar(hook = nameof(OnPlayerStateChanged))]
    public bool isPlaying = false;
    [SyncVar]
    public bool isInited = false;

    private Vector3 movement;
    private Vector2 turn;
    private float angle;

    public GameObject bulletPrefab;
    //set active/dia object nayf ddeer show player?
    public GameObject spacescarft;

    [Header("Health")]
    public int maxHealth = 500;
    [SyncVar(hook = nameof(OnPlayerHpChanged))]
    public int currentHealth;

    [Header("PlayerUX")]
    [SerializeField]
    private GameObject healBar;
    [SerializeField]
    private Text PlayerNameUI;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        cinecam = FindObjectOfType<CinemachineVirtualCamera>();
    }

    public override void OnStartServer()
    {
        SpawnPlayer();
    }

    public override void OnStartClient()
    {
        healBar.GetComponent<HealthBar>().SetMaxHealth(maxHealth);
        //connect vào các server có client sẵn -> objec inited -> update luôn tên
        if(isInited) {
            PlayerNameUI.text = strPlayerName;
            UpdateCharacter(selectedOption);
            healBar.GetComponent<HealthBar>().SetValue(currentHealth);
        }
        
    }

    public override void OnStartAuthority()
    {
        if (!PlayerPrefs.HasKey("selectedOption"))
        {
            selectedOption = 0;
        }
        else
        {
            Load();
        }
        CmdSetPlayerNameAndOption(PlayerPrefs.GetString("playerName"), selectedOption);
    }

    [Command]
    public void CmdSetPlayerNameAndOption(string name, int option)
    {
        strPlayerName = name;
        selectedOption = option;
        isInited = true;
        ClientRpcSetPlayerInfo(name, option);
    }

    [ClientRpc]
    public void ClientRpcSetPlayerInfo(string name, int option)
    {
        UpdateCharacter(option);
    }

    public void OnPlayerNameChanged(string oldName, string newName)
    {
        PlayerNameUI.text = newName;
    }

    private void Start()
    {
        if (hasAuthority)
        {
            cinecam.m_Follow = rb.transform;
            cinecam.m_LookAt = rb.transform;
        }
    }
    [Server]
    void ServerRespawnPlayer()
    {
        GameNetwork gameNetwork = FindObjectOfType<GameNetwork>();
        Vector3 newSpawnPoint = gameNetwork.GetRandomSpawnPoint();
        this.transform.position = newSpawnPoint;
        
        isPlaying = true;
        currentHealth = maxHealth;
        PlayerRespawnRpc();
    }

    [ClientRpc]
    void PlayerRespawnRpc() {
        spacescarft.SetActive(true);
        healBar.GetComponent<HealthBar>().SetValue(maxHealth);
        spawnEffect.Play();
    }
    void SpawnPlayer()
    {
        isPlaying = true;
        currentHealth = maxHealth;
        spacescarft.SetActive(true);
    }

    // Update is called once per frame
    [ClientCallback]
    public void Update()
    {
        if (!hasAuthority) return;
        if (!isPlaying) return;
        // input move
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        if (isLocalPlayer)
        {
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
    }

    public void FixedUpdate()
    {
        // only let the local player control the racket.
        // don't control other player's rackets
        if (isLocalPlayer)
            transform.position += (movement * moveSpeed * Time.fixedDeltaTime);
            // rb.velocity = movement * moveSpeed * Time.fixedDeltaTime;

    }

    [Command]
    private void Shoot()
    {
        GameObject bullet = Instantiate(this.bulletPrefab, transform.position, transform.rotation);
        bullet.GetComponent<Bullet>().SetOwnerNetId(netId);

        // Color bulletColor = characterDB.GetCharacter(selectedOption).bulletColor;
        // bullet.GetComponent<SpriteRenderer>().color = bulletColor;
        // bullet.GetComponent<Bullet>().Project(this.transform.up, isBoosting, bulletColor);

        NetworkServer.Spawn(bullet);

    }

    [ServerCallback]
    void OnCollisionEnter2D(Collision2D collision)
    {
        // damage with velocity
        if(!isPlaying) return;
        // if(collision.GetComponent<Bullet>().owner == this) return;
        if (collision.gameObject.GetComponent<Bullet>() != null)
        {
            Vector3 point = collision.contacts[0].point;
            EffectTakeDamage(point);
        }
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
    void OnTriggerEnter2D(Collider2D other)
    {
        if(!isPlaying) return;
        if (other.GetComponent<Bullet>() != null)
        {
            if (other.GetComponent<Bullet>().ownerNetId != netId)
            {
                TakeDamage(other.GetComponent<Bullet>().damage);
            }
        }
    }

    [ClientRpc]
    public void EffectTakeDamage(Vector3 point)
    {
        this.collisionEffect.transform.position = point;
        this.collisionEffect.Play();
    }

    [Server]
    public void TakeDamage(int newDamage)
    {
        currentHealth -= newDamage;
        if (currentHealth < 0) currentHealth = 0;


        if (currentHealth <= 0 && isPlaying)
        {
            //destroy playser
            OnPlayerDestroyRpc();
            OnTargerDestroyRpc();

            isPlaying = false;
            Invoke("ServerRespawnPlayer", delayRespawn);
        }
    }

    [ClientRpc]
    void OnPlayerDestroyRpc()
    {
        Debug.Log("BoardCast ---> Player " + strPlayerName + " is DEAD....");
        spacescarft.SetActive(false);
        explosionEffect.Play();
    }

    [TargetRpc]
    void OnTargerDestroyRpc()
    {
        Debug.Log("YOU ARE DEAD!!");
        // spacescarft.SetActive(false);
    }

    void OnPlayerHpChanged(int oldValue, int newValue)
    {
        healBar.GetComponent<HealthBar>().SetHealth(newValue);
    }

    void OnPlayerStateChanged(bool oldValue, bool newValue)
    {

    }

    private void UpdateCharacter(int i)
    {
        Character character = characterDB.GetCharacter(i);
        artworkSprite = Instantiate(character.characterSprite, spacescarft.transform);
        artworkSprite.transform.localScale *= 3f;
        artworkSprite.transform.parent = spacescarft.transform;

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

    private void Save()
    {
        PlayerPrefs.SetInt("score", FindObjectOfType<GameManager>().score);
    }
}
