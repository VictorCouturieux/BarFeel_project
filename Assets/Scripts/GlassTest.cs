using UnityEngine;
using UnityEngine.UIElements;

public class GlassTest : MonoBehaviour
{
	[SerializeField] private float speed = 800;
	private Rigidbody rb;


	private float initX;
	private float initZ;

	void Start() {
		rb = transform.GetComponents<Rigidbody>()[0];
		initX = transform.position.x;
		initZ = transform.position.z;
	}
	
	void Update() {
		
		if (GameManager.instance.CurrentStep != GameStep.GET) {
			float x = initX;
			if (Input.mousePosition.x > 0 && Input.mousePosition.x < Screen.width) {
				x = Input.mousePosition.x / Screen.width *4 -10.41f;
			}
			float z = initZ;
			if (Input.mousePosition.y > 0 && Input.mousePosition.y < Screen.height) {
				z = Input.mousePosition.y / Screen.height *3 -10.87f;
			}

			transform.position = new Vector3(x, transform.position.y, z);
		}
	    
		if (Input.GetMouseButtonDown(0) && GameManager.instance.CurrentStep == GameStep.SET) {
			rb.AddForce(Vector3.forward*speed);
			GameManager.instance.NextStep();
		}
	}
}