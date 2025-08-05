using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GunController : MonoBehaviour
{
    public enum FiringMode { SingleShot, Burst, Automatic }

    public GameObject bulletPrefab;
    public Transform[] firePoints;
    public float fireRate = 0.5f;
    public FiringMode firingMode = FiringMode.SingleShot;

    [Header("Burst Settings")]
    public int shotsPerBurst = 3;
    public float burstDelay = 0.1f;

    [Header("Automatic Settings")]
    public float minAutoFireDelay = 0.3f;
    public float maxAutoFireDelay = 0.8f;

    private float nextFireTime = 0.0f;
    private int currentBurstCount = 0;

    void Update()
    {
        if (CanFire())
        {
            FireBullet();
            nextFireTime = Time.time + fireRate;

            if (firingMode == FiringMode.Burst)
            {
                currentBurstCount++;
                if (currentBurstCount >= shotsPerBurst)
                {
                    currentBurstCount = 0;
                }
                else
                {
                    nextFireTime = Time.time + burstDelay;
                }
            }
        }
    }

    bool CanFire()
    {
        if (firingMode == FiringMode.Automatic)
        {
            if (Time.time > nextFireTime)
            {
                float randomDelay = Random.Range(minAutoFireDelay, maxAutoFireDelay);
                nextFireTime = Time.time + randomDelay;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    void FireBullet()
    {
        foreach (Transform fp in firePoints)
        {
            Instantiate(bulletPrefab, fp.position, fp.rotation);
        }
    }
}