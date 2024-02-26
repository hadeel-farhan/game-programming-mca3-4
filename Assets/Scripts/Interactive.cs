using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("ProjectileInteract")){
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            Destroy(gameObject, 0.5f);

        } 
        else if (other.CompareTag("Player")) {
            gameObject.GetComponent<Rigidbody>().useGravity = true;
        }
    }
}
