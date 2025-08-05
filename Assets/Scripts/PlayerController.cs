using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TouchControlsKit;

public class PlayerController : MonoBehaviour
{
    // --- Player Movement ---
    public GameController gameController;
    public float speed;
    public float verticalScrollSpeed = 1.0f;
    private float halfWidth;
    private float halfHeight;

    // --- Firing ---
    public GameObject bulletPrefab;
    public float fireRate;
    public Transform[] firePoints;
    public Transform[] extraFirePoints;
    public bool isBulletSpreadEnabled = false;
    public float spreadAngle = 15;
    public bool isExtraGunEnabled = false;
    public float tapFireDelay = 0.1f;
    private bool canTapFire = true;
    private float nextHoldFireTime = 0f;

    // --- Immunity ---
    public bool isImmune = false;
    public float blinkAlpha = 0.5f;
    public float blinkRate = 0.2f;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    // --- Misc/Gameplay State Variables ---
    private float originalFireRate;
    private float originalSpeed;
    private Camera mainCamera;

    // --- Health ---
    public int maxHealth = 1;
    private int currentHealth;

    // --- Shield ---
    public GameObject shieldPrefab;
    public int maxShields = 3;
    public int currentShields;
    public float shieldVerticalOffset = 0.5f;
    private List<GameObject> shieldInstances = new List<GameObject>();

    void Start()
    {
        mainCamera = Camera.main;
        CalculateBoundaries();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        originalFireRate = fireRate;
        originalSpeed = speed;
        currentHealth = maxHealth;
        currentShields = 0;
    }

    /*     void Update()
        {
            KeepPlayerInBounds();

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            Vector2 moveDirection = new Vector2(horizontalInput, verticalInput);
            Vector2 joystickMove = TCKInput.GetAxis("Joystick");
            moveDirection += joystickMove;

            Vector2 movement = moveDirection.normalized * speed * Time.deltaTime;
            transform.Translate(movement);

            transform.Translate(Vector3.up * verticalScrollSpeed * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Space) || TCKInput.GetAction("fireBtn", EActionEvent.Press))
            {
                if (canTapFire)
                {
                    FireBullets();
                    canTapFire = false;
                    StartCoroutine(ResetTapFire());
                }
            }

            if ((Input.GetKey(KeyCode.Space) || TCKInput.GetAction("fireBtn", EActionEvent.Down)) && Time.time > nextHoldFireTime)
            {
                nextHoldFireTime = Time.time + fireRate;
                FireBullets();
            }
        }
     */
    void Update()
    {
        if (gameController != null && gameController.shouldUseMobileControls)
        {
            KeepPlayerInBounds();

            Vector2 joystickMove = TCKInput.GetAxis("Joystick");
            Vector2 movement = joystickMove.normalized * speed * Time.deltaTime;
            transform.Translate(movement);

            if (TCKInput.GetAction("fireBtn", EActionEvent.Press))
            {
                if (canTapFire)
                {
                    FireBullets();
                    canTapFire = false;
                    StartCoroutine(ResetTapFire());
                }
            }

            if (TCKInput.GetAction("fireBtn", EActionEvent.Down) && Time.time > nextHoldFireTime)
            {
                nextHoldFireTime = Time.time + fireRate;
                FireBullets();
            }
        }
        else
        {
            KeepPlayerInBounds();

            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector2 movement = new Vector2(horizontalInput, verticalInput) * speed * Time.deltaTime;
            transform.Translate(movement);

            transform.Translate(Vector3.up * verticalScrollSpeed * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (canTapFire)
                {
                    FireBullets();
                    canTapFire = false;
                    StartCoroutine(ResetTapFire());
                }
            }

            if (Input.GetKey(KeyCode.Space) && Time.time > nextHoldFireTime)
            {
                nextHoldFireTime = Time.time + fireRate;
                FireBullets();
            }

        }
    }

    void KeepPlayerInBounds()
    {
        Vector3 playerPos = transform.position;
        Vector3 viewPortPos = mainCamera.WorldToViewportPoint(playerPos);

        viewPortPos.x = Mathf.Clamp01(viewPortPos.x);
        viewPortPos.y = Mathf.Clamp01(viewPortPos.y);

        transform.position = mainCamera.ViewportToWorldPoint(viewPortPos);
    }

    void CalculateBoundaries()
    {
        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(Vector3.zero);
        Vector3 topRight = mainCamera.ViewportToWorldPoint(Vector3.one);

        halfWidth = Mathf.Abs(topRight.x - bottomLeft.x) / 2f;
        halfHeight = Mathf.Abs(topRight.y - bottomLeft.y) / 2f;
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (!isImmune)
            {
                TakeDamage(1);
            }
        }
    }
    public void ResetPlayer()
    {
        fireRate = originalFireRate;
        speed = originalSpeed;

        isBulletSpreadEnabled = false;
        isExtraGunEnabled = false;

        foreach (Transform firePoint in extraFirePoints)
        {
            firePoint.gameObject.SetActive(false);
        }
    }
    public void GrantImmunity(float duration)
    {
        isImmune = true;
        StartCoroutine(EndImmunity(duration));
        StartCoroutine(BlinkEffect());
    }

    IEnumerator EndImmunity(float duration)
    {
        yield return new WaitForSeconds(duration);
        isImmune = false;
        spriteRenderer.enabled = true;
    }

    IEnumerator BlinkEffect()
    {
        while (isImmune)
        {
            Color blinkColor = originalColor;
            blinkColor.a = blinkAlpha;
            spriteRenderer.color = blinkColor;
            yield return new WaitForSeconds(blinkRate);

            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(blinkRate);
        }
    }
    public void IncreaseFireRate(float increase)
    {
        fireRate -= increase;
        fireRate = Mathf.Max(fireRate, 0.2f);
    }

    public void IncreaseSpeed(float increase)
    {
        speed += increase;
        speed = Mathf.Min(speed, 10.0f);
    }

    public void IncreaseSpread(float angle)
    {
        isBulletSpreadEnabled = true;
        spreadAngle = angle;
    }

    public void EnableExtraGun()
    {
        isExtraGunEnabled = true;

        foreach (Transform firePoint in extraFirePoints)
        {
            firePoint.gameObject.SetActive(true);
        }
    }

    IEnumerator ResetTapFire()
    {
        yield return new WaitForSeconds(tapFireDelay);
        canTapFire = true;
    }

    void FireBullets()
    {
        foreach (Transform firePoint in firePoints)
        {
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }

        if (isBulletSpreadEnabled)
        {
            foreach (Transform firePoint in firePoints)
            {
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, spreadAngle));
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * Quaternion.Euler(0, 0, -spreadAngle));
            }
        }

        if (isExtraGunEnabled)
        {
            foreach (Transform firePoint in extraFirePoints)
            {
                Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentShields > 0)
        {
            Debug.Log("Player lost a shield");
            currentShields--;
            GrantImmunity(0.3f);
            DestroyShield();
        }
        else
        {
            currentHealth -= damage;
            GrantImmunity(0.3f);
            if (currentHealth == 0)
            {
                Debug.Log("Player took " + damage + " damage");
                GameController.instance.PlayerDestroyed(gameObject);
            }
        }
    }
    public void ResetHealth()
    {
        currentHealth = maxHealth;
    }

    public void GainShield()
    {
        if (currentShields < maxShields)
        {
            currentShields++;
            CreateShield();
        }
    }

    void CreateShield()
    {
        Vector3 offsetPosition = transform.position + Vector3.up * shieldVerticalOffset * shieldInstances.Count;
        GameObject newShield = Instantiate(shieldPrefab, offsetPosition, Quaternion.identity, transform); // Parent to player
        shieldInstances.Add(newShield);
    }

    void DestroyShield()
    {
        if (shieldInstances.Count > 0)
        {
            Destroy(shieldInstances.Last());
            shieldInstances.RemoveAt(shieldInstances.Count - 1);
        }
    }
}