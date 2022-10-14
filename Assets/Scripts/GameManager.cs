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

    private GameObject fillArea;
    private GameObject cleanArea;
    private GameObject throwArea;
    
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
        fillArea = GameObject.FindWithTag("Fill");
        cleanArea = GameObject.FindWithTag("Clean");
        throwArea = GameObject.FindWithTag("Throw");
        
        clientController = GameObject.FindWithTag("Client").GetComponent<ClientController>();
        UpdateActiveAreas();
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
        UpdateActiveAreas();
        if (currentStep == GameStep.WAIT_CIENT) {
            // grabGlass = false;
            clientController.ClientCome();
        }
    }

    private void UpdateActiveAreas() {
        if (currentStep == GameStep.FILL) {
            SetActiveAreas(fill: true);
        } else if (currentStep == GameStep.CLEAN) {
            SetActiveAreas(clean: true);
        } else if (currentStep == GameStep.SET) {
            SetActiveAreas(_throw: true);
        } else {
            SetActiveAreas();
        }
    }

    private void SetActiveAreas(bool fill=false, bool clean=false, bool _throw=false) {
        fillArea.SetActive(fill);
        cleanArea.SetActive(clean);
        throwArea.SetActive(_throw);
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
