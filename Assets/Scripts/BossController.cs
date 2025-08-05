using UnityEngine;

public class BossController : MonoBehaviour
{
    public Camera mainCamera;
    public float verticalSpeed = 2f;
    public float verticalOffset = 1f;
    public GameObject BossRocketPrefab;
    public Transform firePoint;
    public float rocketFireRate = 3f;
    public int initialHealth = 50;
    private int currentHealth;

    public GameObject gunType1Prefab;
    public GameObject gunType2Prefab;
    public GameObject gunType3Prefab;
    public Transform[] backGunSpawns;
    public Transform[] middleGunSpawns;
    public Transform[] frontGunSpawns;
    private float nextFireTime = 0.0f;
    public bool bossVulnerable = false;
    public PolygonCollider2D bossBodyCollider;

    private int maxOffsetReductions = 2;
    private int currentOffsetReductions = 0;
    public float initialVerticalOffset = 15f;
    public float finalVerticalOffset = 5f;
    public float offsetChangeDuration = 10f;

    private float offsetStartTime;
    private bool isOffsetChanging = false;

    void Start()
    {
        currentHealth = initialHealth;
        SpawnGuns();
        mainCamera = Camera.main;
        bossBodyCollider.enabled = false;
        offsetStartTime = Time.time;
        isOffsetChanging = true;
    }

    void SpawnGuns()
    {
        SpawnGunType(gunType1Prefab, frontGunSpawns);
        SpawnGunType(gunType2Prefab, middleGunSpawns);
        SpawnGunType(gunType3Prefab, backGunSpawns);
    }

    void SpawnGunType(GameObject gunPrefab, Transform[] spawnPoints)
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            GameObject gun = Instantiate(gunPrefab, spawnPoint.position, spawnPoint.rotation);
            gun.transform.SetParent(spawnPoint);

            RotatingGun gunController = gun.GetComponent<RotatingGun>();
            if (gun.transform.position.x > 0)
            {
                gunController.invertRotation = true;
            }
        }
    }

    void Update()
    {
        if (isOffsetChanging)
        {
            UpdateVerticalOffset();
        }

        Vector3 targetPosition = new Vector3(
            transform.position.x,
            mainCamera.transform.position.y + verticalOffset,
            transform.position.z);

        transform.position = Vector3.Lerp(transform.position, targetPosition, verticalSpeed * Time.deltaTime);

        if (Time.time > nextFireTime)
        {
            Instantiate(BossRocketPrefab, firePoint.position, firePoint.rotation);
            nextFireTime = Time.time + rocketFireRate;
        }

        if (!bossVulnerable && GameObject.FindGameObjectsWithTag("BossGun").Length == 0)
        {
            bossVulnerable = true;
            if (bossBodyCollider != null)
            {
                bossBodyCollider.enabled = true;
            }
        }

        CheckForOffsetReduction();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            nextFireTime = 1000000f;
            GameController.instance.BossDestroyed(gameObject);
        }
    }
    void CheckForOffsetReduction()
    {
        if (currentOffsetReductions < maxOffsetReductions)
        {
            if (!AreGunsAlive(frontGunSpawns) && currentOffsetReductions == 0)
            {
                verticalOffset -= 1;
                currentOffsetReductions++;
            }
            else if (!AreGunsAlive(middleGunSpawns) && currentOffsetReductions == 1)
            {
                verticalOffset -= 1;
                currentOffsetReductions++;
            }
        }
    }

    bool AreGunsAlive(Transform[] spawnPoints)
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint.childCount > 0)
            {
                //Debug.Log("Gun found");
                return true;
            }
        }
        return false;
    }

    void UpdateVerticalOffset()
    {
        float elapsedTime = Time.time - offsetStartTime;
        float lerpFactor = Mathf.Clamp01(elapsedTime / offsetChangeDuration);

        verticalOffset = Mathf.Lerp(initialVerticalOffset, finalVerticalOffset, lerpFactor);

        if (elapsedTime >= offsetChangeDuration)
        {
            isOffsetChanging = false;
        }
    }

}
