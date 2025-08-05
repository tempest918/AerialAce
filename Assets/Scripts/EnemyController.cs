using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // --- Movement ---
    public float speed; 

    // --- Firing ---
    public GameObject enemyBulletPrefab; 
    public Transform firePoint;
    public float fireRate = 2f; 
    private float nextFireTime = 0.0f;
    private float initialShootDelay = 0f; 

    // --- Internal State --- 
    private Camera mainCamera;    

    void Start()
    {
        mainCamera = Camera.main;
        initialShootDelay = Random.Range(0f, 1f);
        nextFireTime = Time.time + initialShootDelay;
    }

    void Update()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;

        if (Time.time > nextFireTime)
        {
            Instantiate(enemyBulletPrefab, firePoint.position, firePoint.rotation);
            nextFireTime = Time.time + fireRate;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameController.instance.EnemyDestroyed(gameObject,0);
        }
    }

    void OnBecameInvisible()
    {
        Vector3 bottomOfScreen = mainCamera.ViewportToWorldPoint(Vector3.zero);

        if (transform.position.y < bottomOfScreen.y + 1f)
        {
            Destroy(gameObject);
        }
    }
}
