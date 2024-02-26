using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int startingHealth = 100;
    public AudioClip deadSFX;
    public Slider healthSlider;

    public static int currentHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = startingHealth;
        healthSlider.value = currentHealth;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("current health: " + currentHealth);

    }

    public void TakeDamage(int damageAmount) {
        if (!LevelManager.isGameOver) {
            if (currentHealth > 0) {
                currentHealth -= damageAmount;
                healthSlider.value = currentHealth;

            }
            if (currentHealth <= 0) {
                PlayerDies();
            }
        }

        Debug.Log("current health: " + currentHealth);
    }

    void PlayerDies() {
        Debug.Log("Player is dead...");
        AudioSource.PlayClipAtPoint(deadSFX, transform.position);
        transform.Rotate(-90, 0, 0, Space.Self);
    }

    public void TakeHealth(int healthAmount) {
        if (currentHealth < 100) {
            currentHealth += healthAmount;
            healthSlider.value = Mathf.Clamp(currentHealth, 0, 100);

        }
        if (currentHealth <= 0) {
            PlayerDies();
        }

        Debug.Log("current health: " + currentHealth);
    }


}
