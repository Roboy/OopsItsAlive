using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    //Evil global Hack
    public static Player player;
    //Numbers
    [Header("--- Player Values: ---------")]
    //public int lives = 5;
    //public float damagePerShot = 10;
    //public float healingPerShot = 10;
    //public float projectileCooldown = 0.1f;
    //public float friction = 1f;
    public float acceleration = 5;
    //public float maximumVelocity = 100;
    //public float minimumVelocity = 1f;
    //public float projectileSpeed = 10;
    public float moveSpeed = 100;
    //Gameplay public bools
    //public bool invulnerable = false;
    public bool canSpike = false;
    bool usingspikes = false;

    //public bool canShoot = false;
    public bool canHeal = false;

    [Header("--- Energie: ---------")]
    public static int atp = 10;
    public int startATP = 10;
    public static int maxATP = 50;

    [Header("--- Externe Ressourcen: ---------")]
    //System public bools
    public bool inputEnabled = true;

    public float soundCooldown = 0.1f;
    private float soundTimer = 0f;
    public AudioClip plopSound;
    public AudioClip spikeSound;
    public SpriteRenderer stacheln;
    public LineRenderer healBeam;

    //Public Prefabs
    //public Projectile damageProjectilePrefab;

    //Private variables, timers and such
    //private float rocketsReadyTimer = 0;
    private float nextProjectileReadyTimer = 0;
    private float lastUpdate = 0.5f;
    private bool healingActive = false;
    private float invulnerabilityTimer = 0;
    private Vector2 velocity = new Vector3(0, 0);
    private float movementCooldown = 0;

    public KeyCode ATPCheatKey = KeyCode.P;

    private float auraMult = 1.0f;
    Rigidbody2D myRigidbody;
    public KeyCode spikeKey = KeyCode.LeftShift;

    private AudioSource source;

    // Use this for initialization
    void Start()
    {
        atp = startATP;
        myRigidbody = GetComponent<Rigidbody2D>();
        player = this;
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(ATPCheatKey))
        {
            AddATP(9999);
        }

        GetComponent<Transform>().Translate(new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Time.deltaTime * acceleration * auraMult, Space.World);

        LookAtMouseCursor();
        ShootingHandler();

        lastUpdate -= Time.deltaTime;
        if (lastUpdate < 0 && healingActive){
            AddATP(-1);
            lastUpdate = 0.5f;
        }
        if (atp == 0 && healingActive)
        {
            DisableHealingBeam();
        }
    }


    private void LookAtMouseCursor()
    {
        transform.LookAt(ClampDepth(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        transform.Rotate(new Vector3(0, 90, 90));
    }

    private Vector3 ClampDepth(Vector3 input)
    {
        return new Vector3(input.x, input.y, 0);
    }

    /*public void ShootProjectile(Projectile.ProjectileType projectileType)
    {
        //Offset = how far away from the player's position the projectile spawns
        float offset = 2;
        //Getting spawnlocation
        Vector3 thisPosition = this.GetComponent<Rigidbody2D>().transform.position;
        Quaternion thisRotation = this.GetComponent<Rigidbody2D>().transform.rotation;
        Debug.Log(thisRotation);
        Vector3 mousePositionInWorld = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
        Vector3 aimingVector = new Vector3(mousePositionInWorld.x - thisPosition.x, mousePositionInWorld.y - thisPosition.y, 0);
        aimingVector.Normalize();

        Vector3 spawnPosition = thisPosition + (offset * aimingVector);
        //Creating Projectile
        Projectile projectileSpawned;
        projectileSpawned = Instantiate(damageProjectilePrefab);
        projectileSpawned.GetComponent<Rigidbody2D>().transform.position = spawnPosition;
        projectileSpawned.GetComponent<Rigidbody2D>().transform.rotation = thisRotation;
        projectileSpawned.velocity = new Vector2(projectileSpeed * aimingVector.x, projectileSpeed * aimingVector.y);
    }*/

    private void MovementHandler()
    {
        float horizontalMovementValue = Input.GetAxis("Horizontal");
        velocity = velocity + (acceleration * horizontalMovementValue * new Vector2(1, 0));
        float verticalMovementValue = Input.GetAxis("Vertical");

        velocity = (velocity + (acceleration * verticalMovementValue * new Vector2(0, 1))) * auraMult;
    }

    private void ShootingHandler()
    {
		/*if (Input.GetButton("Fire1") && canShoot)
        {
            if (nextProjectileReadyTimer <= 0)
            {
                ShootProjectile(Projectile.ProjectileType.friendlyDamage);
                nextProjectileReadyTimer = projectileCooldown;
            }
            else
            {
                nextProjectileReadyTimer -= Time.deltaTime;
            }
        }*/
		if (Input.GetButtonDown("Fire2") && canHeal)
        {
            healBeam.gameObject.SetActive(true);
            healingActive = true;
        }
		else if (Input.GetButtonUp("Fire2") && canHeal)
        {
            DisableHealingBeam();
        }
		else if (Input.GetKeyDown(spikeKey) && canSpike)
        {
            if (atp >= 5)
            {
                if (!usingspikes)
                {
                    AddATP(-5);
                    StartCoroutine(UseSpikes());
                }
            }
        }
    }

    public void DisableHealingBeam()
    {
        healBeam.gameObject.SetActive(false);
        healingActive = false;
    }

    public IEnumerator UseSpikes()
    {
        usingspikes = true;
        CircleCollider2D sColl = GetComponent<CircleCollider2D>();
        float backupRadius = sColl.radius;

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.25f);
            source.PlayOneShot(spikeSound, 0.1f);
            stacheln.gameObject.SetActive(true);
            sColl.radius = backupRadius * 1.1f;
            yield return new WaitForSeconds(0.25f);
            stacheln.gameObject.SetActive(false);
            sColl.radius = backupRadius;
        }
        usingspikes = false;
    }

    public bool isUsingSpikes()
    {
        return usingspikes;
    }


    void OnTriggerEnter2D(Collider2D hit)
    {

        handleAuraCollision(hit);
        CollectPickup(hit);
    }

    void handleAuraCollision(Collider2D hit)
    {
        Aura aura = hit.GetComponent<Aura>();
        if (aura == null) return;
        if (aura.auraEffect == Aura.KINDSOFAURA.slow)
            auraMult = 0.5f;
        if (aura.auraEffect == Aura.KINDSOFAURA.fast)
            auraMult = 2.0f;
    }


    void OnTriggerExit2D(Collider2D hit)
    {
        Aura aura = hit.GetComponent<Aura>();
        if (aura) auraMult = 1.0f;
    }

    private void CollectPickup(Collider2D hit)
    {
        // ATP is used as Enegery = lives
        if (hit.tag == "Pickup_ATP")
        {
            if (atp < maxATP) AddATP(1);
            Destroy(hit.gameObject);
        }
    }

    private void AddATP(int v)
    {
        atp = Mathf.Clamp(atp + v, 0, maxATP);
    }

    public void CellDivisionSound(Vector2 cellLocation)
    {
        soundTimer -= Time.deltaTime;
        if(soundTimer <= 0)
        {
            float volumeScale = 1 / (Vector3.Distance(cellLocation, gameObject.transform.position));
            if (volumeScale > 1)
                volumeScale = 1;
            source.PlayOneShot( plopSound, volumeScale);
            soundTimer = soundCooldown;
        }
    }
}
