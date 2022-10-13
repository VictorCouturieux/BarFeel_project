using UnityEngine;

public class GlassTest : MonoBehaviour
{
	[SerializeField] private float speed = 200;
	private Rigidbody rb;

	void Start() {
		rb = transform.GetComponents<Rigidbody>()[0];
	}
	
	void Update() {
		
		if (GameManager.instance.CurrentStep != GameManager.GameStep.GET) {
			float x = 0.5f;
			if (Input.mousePosition.x > 0 && Input.mousePosition.x < Screen.width) {
				x = Input.mousePosition.x / Screen.width;
			}
			float z = 0.5f;
			if (Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height) {
				z = Input.mousePosition.y / Screen.height;
			}

			transform.position = new Vector3(x, transform.position.y, z);
		}
	    
		if (Input.GetMouseButtonDown(0) && GameManager.instance.CurrentStep == GameManager.GameStep.SET) {
			rb.AddForce(Vector3.forward*speed);
			GameManager.instance.NextStep();
		}
	}
}