using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingDust : MonoBehaviour
{
    [SerializeField] private GameObject dustPrefab;
    public GameObject dust;
    //public CameraShakeCaveEvent cameraShake;
    public CameraShake camera;

    void Awake() {
        //collider = GetComponent<Collider2D>();
    }
    
    void Update() {
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Player")) {
            Debug.Log("all works");
            //camera.ShakeCamera(3f, 2f);
            camera.ShakeCamera(5f, 3f);
            dust = Instantiate(dustPrefab, transform.position, Quaternion.identity);
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        Destroy(dust, 1.5f);
    }
    
}
