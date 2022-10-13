using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    private GameStep currentStep = GameStep.WAIT_CIENT;
    public GameStep CurrentStep {
        get { return currentStep; }
    }

    private Collider fillCollider;
    private Collider cleanCollider;
    
    private float timer = 0;
    [SerializeField] private float timerDurationWaitClientInSec = 5;
    [SerializeField] private float timerDurationGetGlassInSec = 5;

    
    private Renderer testClientRenderer;
    private TestAnimClient testClientAnim;
    private int debugSec = 0;

    public enum GameStep
    {
        WAIT_CIENT,
        FILL,
        SET,
        GET,
        CLEAN
    }

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

        testClientRenderer = GameObject.Find("TestClient").GetComponent<Renderer>();
        testClientAnim = GameObject.Find("TestClient").GetComponent<TestAnimClient>();

        testClientAnim.ClientUp();
    }

    public void NextStep() {
        if (currentStep == GameStep.CLEAN) {
            currentStep = GameStep.WAIT_CIENT;
        }
        else {
            currentStep++;
        }
        Debug.Log(currentStep);
        BeAbleZoneCollider();
        if (currentStep == GameStep.WAIT_CIENT) {
            testClientAnim.ClientUp();
        } else if (currentStep == GameStep.GET) {
            testClientAnim.ClientDown();
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
