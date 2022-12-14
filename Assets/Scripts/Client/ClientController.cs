using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class ClientController : MonoBehaviour
{
	[SerializeField] private ClientAnim[] clientList;
	private ClientAnim currentClient;
	private int currentIndexClient = -1;
	private IEnumerator currentCoroutine;
	
	[SerializeField] private Material material;
	[SerializeField] private Animator _testClientAnimator;
	[SerializeField] private CharacterSoundGameEvent _characterSound;

	private GameObject bubble;
	
	[SerializeField] private VoidGameEvent glassReceived;
	
	private float timer = 0;
	[SerializeField] private float timeDrink = 10;
	private bool startTimer;

	private void Start() {
		bubble = transform.GetChild(0).gameObject;
		bubble.SetActive(false);
		// _characterSound.Call(type, CharacterSoundType.SIGH, transform.position);
		ClientCome();
	}

	private void Update() {
		//temporary stop glass
		if (startTimer) {
			timer += Time.deltaTime;
			if (timer >= timeDrink) {
				StopCoroutine(currentCoroutine);
				material.SetTexture("_MainTex", currentClient.ClientBase);
				ClientLeave();
				GameManager.instance.NextStep();
				startTimer = false;
				timer = 0;
			}
		}
	}

	public void ClientCome() {
		ChangeClient();
		material.SetTexture("_MainTex", currentClient.ClientBase);
		Debug.Log("ENTER");
		_characterSound.Call(currentClient.type, CharacterSoundType.ENTER, transform.position);
		_testClientAnimator.SetTrigger("ClientCome");
	}

	public void ClientAckForGlass() {
		GameManager.instance.NextStep();
		if (currentCoroutine != null) {
			StopCoroutine(currentCoroutine);
		}
		currentCoroutine = LoopAskForGlass();
		StartCoroutine(currentCoroutine);
	}

	private IEnumerator LoopAskForGlass() {
		while (true) {
			material.SetTexture("_MainTex", currentClient.ClientSign);
			Debug.Log("SIGH");
			_characterSound.Call(currentClient.type, CharacterSoundType.SIGH, transform.position);
			bubble.SetActive(true);
			yield return new WaitForSeconds(Random.Range(3f, 5f));
			material.SetTexture("_MainTex", currentClient.ClientBase);
			bubble.SetActive(false);
			yield return new WaitForSeconds(Random.Range(5f, 15f));
		}
	}
	
	public void ClientGrabAndDrink() {
		bubble.SetActive(false);
		if (currentCoroutine != null) {
			StopCoroutine(currentCoroutine);
		}
		currentCoroutine = LoopGrabAndDrink();
		Debug.Log("SATISFIED");
		_characterSound.Call(currentClient.type, CharacterSoundType.SATISFIED, transform.position);
		StartCoroutine(currentCoroutine);
		startTimer = true;
	}
	
	private IEnumerator LoopGrabAndDrink() {
		while (true) {
			material.SetTexture("_MainTex", currentClient.ClientGrab);
			yield return new WaitForSeconds(Random.Range(1f, 6f));
			material.SetTexture("_MainTex", currentClient.ClientDrink);
			Debug.Log("DRINKING");
			_characterSound.Call(currentClient.type, CharacterSoundType.DRINKING, transform.position);
			yield return new WaitForSeconds(Random.Range(1f, 3f));
		}
	}
	
	public void ClientLeave() {
		_testClientAnimator.SetTrigger("ClientLeave");
	}

	public void ClientExit() {
		Debug.Log("EXIT");
		_characterSound.Call(currentClient.type, CharacterSoundType.EXIT, transform.position);
	}
	
	private void OnTriggerEnter(Collider collision) {
		glassReceived.Call();
		ClientGrabAndDrink();
	}

	private void ChangeClient() {
		int r = currentIndexClient;
		while (currentIndexClient == r) {
			r = Random.Range(0, 3);
		}
		currentIndexClient = r;
		int t;
		if (r == 2) {
			t = Random.Range(0, 2);
		}
		else {
			t = Random.Range(0, 3);
		}

		int i = r * 3 + t;
		currentClient = clientList[i];
		
	}

}