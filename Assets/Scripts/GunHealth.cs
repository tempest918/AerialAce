using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunHealth : MonoBehaviour
{
    public int initialHealth = 15;
    private int currentHealth;

    void Start() {
        currentHealth = initialHealth;
    }

    public void TakeDamage(int amount) {
        Debug.Log("hit boss gun");
        currentHealth -= amount;
        Debug.Log("current health: " + currentHealth);
        if (currentHealth <= 0) {
            GameController.instance.BossGunDestroyed(gameObject);
        }
    }

}
