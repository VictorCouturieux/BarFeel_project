using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager instance = null;
    [SerializeField] private GameStepGameEvent newGameStepEvent;

    private GameStep currentStep = GameStep.WAIT_CIENT;
    public GameStep CurrentStep {
        get { return currentStep; }
    }

    private Collider fillCollider;
    private Collider cleanCollider;
    
    
    private ClientController clientController;

    private void Awake() {
        if (instance == null) {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start() {
        fillCollider = GameObject.FindWithTag("Fill").GetComponent<Collider>();
        cleanCollider = GameObject.FindWithTag("Clean").GetComponent<Collider>();
        
        clientController = GameObject.FindWithTag("Client").GetComponent<ClientController>();
    }

    public void NextStep() {
        if (currentStep == GameStep.CLEAN) {
            currentStep = GameStep.WAIT_CIENT;
        }
        else {
            currentStep++;
        }
        newGameStepEvent.Call(currentStep);

        Debug.Log(currentStep);
        BeAbleZoneCollider();
        if (currentStep == GameStep.WAIT_CIENT) {
            // grabGlass = false;
            clientController.ClientCome();
        }
    }

    private void BeAbleZoneCollider() {
        if (currentStep == GameStep.FILL) {
            fillCollider.enabled = true;
            cleanCollider.enabled = false;
        }
        else if (currentStep == GameStep.CLEAN) {
            fillCollider.enabled = false;
            cleanCollider.enabled = true;
        }
        else {
            fillCollider.enabled = false;
            cleanCollider.enabled = false;
        }
    }

}

[System.Serializable]
public enum GameStep
{
    WAIT_CIENT,
    FILL,
    SET,
    GET,
    CLEAN
}
