using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { FireRate, BulletSpread, ExtraLife, ExtraScore, Immunity, Shield, SpeedBoost, Bomb, ExtraGun }
    public PowerUpType powerUpType;
    public float fireRateIncrease = 0.1f;
    public float bulletSpreadAngle = 15;
    public float flyingSpeedIncrease = 3.0f;
    public float ImmunityDuration = 10.0f;
    public enum PowerupType { Standard, Bomb }
    public PowerupType powerupType = PowerupType.Standard;

    public AudioClip standardPowerupSound;
    public AudioClip bombPowerupSound;

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayPowerupSound();
            ApplyPowerUp(other.gameObject);
            Destroy(gameObject);
        }
    }

    void ApplyPowerUp(GameObject playerObject)
    {
        PlayerController playerController = playerObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            switch (powerUpType)
            {
                case PowerUpType.BulletSpread:
                    playerController.IncreaseSpread(bulletSpreadAngle);

                    if (playerController.isExtraGunEnabled && playerController.isBulletSpreadEnabled)
                    {
                        playerController.IncreaseFireRate(fireRateIncrease);
                    }
                    break;
                case PowerUpType.ExtraGun:
                    playerController.EnableExtraGun();

                    if (playerController.isBulletSpreadEnabled && playerController.isExtraGunEnabled)
                    {
                        playerController.IncreaseFireRate(fireRateIncrease);
                    }
                    break;
                case PowerUpType.ExtraLife:
                    GameController.instance.UpdateLives(1);
                    break;
                case PowerUpType.ExtraScore:
                    GameController.instance.AddScore(100);
                    break;
                case PowerUpType.Immunity:
                    playerController.GrantImmunity(ImmunityDuration);
                    break;
                case PowerUpType.SpeedBoost:
                    playerController.IncreaseSpeed(flyingSpeedIncrease);
                    break;
                case PowerUpType.Bomb:
                    GameController.instance.DestroyAllEnemies();
                    break;
                case PowerUpType.Shield:
                    playerController.GainShield();
                    break;
            }
        }
    }

    void OnBecameInvisible()
    {
        if (GameController.instance.guaranteeNextPowerup)
        {
            GameController.instance.guaranteeNextPowerup = false;
        }

        Destroy(gameObject);
    }

    void PlayPowerupSound()
    {
        if (powerupType == PowerupType.Standard)
        {
            AudioSource.PlayClipAtPoint(standardPowerupSound, transform.position);
        }
        else if (powerupType == PowerupType.Bomb)
        {
            AudioSource.PlayClipAtPoint(bombPowerupSound, transform.position);
        }
    }
}
