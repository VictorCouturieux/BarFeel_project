using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glass : MonoBehaviour
{
	private bool isShoot = false;

	[SerializeField] private float speed = 200;
	private Rigidbody rb;
	
    void Start() {
	    rb = transform.GetComponents<Rigidbody>()[0];
    }
	
    void Update() {
	    if (Input.GetMouseButtonDown(0)) {
		    isShoot = true;
		    rb.AddForce(Vector3.forward*speed);
	    }
	    
	    if (!isShoot) {
		    float x = 0.5f;
		    if (Input.mousePosition.x < Screen.width) {
			    x = Input.mousePosition.x / Screen.width;
		    }
		    float z = 0.5f;
		    if (Input.mousePosition.y < Screen.height) {
			    z = Input.mousePosition.y / Screen.height;
		    }

		    transform.position = new Vector3(x, transform.position.y, z);
	    }
	    
    }
}
