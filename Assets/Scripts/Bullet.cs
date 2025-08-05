using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bullet : MonoBehaviour
{
    public float speed;
    public bool isPlayerBullet;
    private Camera mainCamera;
    public GameObject hitAnimPrefab;
    public float hitAnimDuration = 0.5f;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (isPlayerBullet)
        {
            transform.Translate(Vector2.up * speed * Time.deltaTime);
            CheckTopBoundary();
        }
        else
        {
            transform.Translate(Vector2.down * speed * Time.deltaTime);
        }
    }

    void CheckTopBoundary()
    {
        Vector3 topOfScreen = mainCamera.ViewportToWorldPoint(Vector3.one);
        if (transform.position.y > topOfScreen.y)
        {
            Destroy(gameObject);
        }
    }

    //void OnCollisionEnter2D(Collision2D other)
    void OnTriggerEnter2D(Collider2D other)
    {
        Detonate();

        if (isPlayerBullet && other.gameObject.CompareTag("Enemy"))
        {
            GameController.instance.EnemyDestroyed(other.gameObject, 10);
        }
        else if (!isPlayerBullet && other.gameObject.CompareTag("Player"))
        {         
            PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
            if (playerController != null && !playerController.isImmune)
            {
                //GameController.instance.PlayerDestroyed(other.gameObject);
                playerController.TakeDamage(1);
            }
        }
        else if (isPlayerBullet && other.gameObject.CompareTag("BossGun"))
        {
            other.gameObject.GetComponent<GunHealth>().TakeDamage(1);

        }
        else if (isPlayerBullet && other.gameObject.CompareTag("Boss"))
        {
            other.gameObject.GetComponent<BossController>().TakeDamage(1);
        }
    }
    void Detonate()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        GameObject hit = Instantiate(hitAnimPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
        Destroy(hit, hitAnimDuration);
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
