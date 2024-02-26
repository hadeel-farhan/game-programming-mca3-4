using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShootProjectile : MonoBehaviour
{
    public GameObject projectilePrefab;
    public GameObject interactPrefab;
    public float projectileSpeed = 100;
    // public AudioClip spellSFX;

    public Image reticleImage;
    public Color reticleInteractColor;
    public Color interactColor;

    Color originalReticleColor;

    GameObject currentProjectilePrefab;

    // Start is called before the first frame update
    void Start()
    {
        originalReticleColor = reticleImage.color;
        currentProjectilePrefab = projectilePrefab;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1")){
            GameObject projectile = Instantiate(currentProjectilePrefab,
             transform.position + transform.forward, transform.rotation);

            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            rb.AddForce(transform.forward * projectileSpeed, ForceMode.VelocityChange);

            projectile.transform.SetParent(GameObject.FindGameObjectWithTag("ProjectileParent").transform);

            // AudioSource.PlayClipAtPoint(spellSFX, transform.position);
        }
    }

    private void FixedUpdate() {
        ReticleEffect();
    }

    void ReticleEffect() {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity)) {
            if(hit.collider.CompareTag("Enemy")) {
                currentProjectilePrefab= projectilePrefab;
                reticleImage.color = Color.Lerp(reticleImage.color, reticleInteractColor, Time.deltaTime * 2);
                reticleImage.transform.localScale = Vector3.Lerp(reticleImage.transform.localScale, new Vector3(0.7f, 0.7f, 1), Time.deltaTime * 2);
            } 
            else if (hit.collider.CompareTag("Interact")) {
                currentProjectilePrefab= interactPrefab;
                reticleImage.color = Color.Lerp(reticleImage.color, interactColor, Time.deltaTime * 2);
                reticleImage.transform.localScale = Vector3.Lerp(reticleImage.transform.localScale, new Vector3(0.7f, 0.7f, 1), Time.deltaTime * 2);
            }
            else {
                currentProjectilePrefab= projectilePrefab;
                reticleImage.color = Color.Lerp(reticleImage.color, originalReticleColor, Time.deltaTime * 2);
                reticleImage.transform.localScale = Vector3.Lerp(reticleImage.transform.localScale, Vector3.one, Time.deltaTime * 2);    
            }
        }
        else {
            reticleImage.color = Color.Lerp(reticleImage.color, originalReticleColor, Time.deltaTime * 2);
            reticleImage.transform.localScale = Vector3.Lerp(reticleImage.transform.localScale, Vector3.one, Time.deltaTime * 2);  
        }
    }
}
