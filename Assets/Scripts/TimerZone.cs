using UnityEngine;
using UnityEngine.Animations;

public class TimerZone : MonoBehaviour
{

    private Collider _collider;
    [SerializeField] private PositionConstraint _positionConstraint;
    [SerializeField] private float timerDurationInSec = 2;

    private float timer = 0;
    
    void Start() {
        _collider = GetComponent<Collider>();
        _collider.isTrigger = true;
    }


    private void OnTriggerStay(Collider collision) {
        if (collision.gameObject.CompareTag("Glass")) {
            timer += Time.deltaTime;
            if (timer >= timerDurationInSec) {
                //Debug.Log("Timer Down");
                //todo : increase step completion for glass
                GameManager.instance.NextStep();
                _positionConstraint.constraintActive = true;
                timer = 0;
            }
        }
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.CompareTag("Glass")) {
            _positionConstraint.constraintActive = false;
            _positionConstraint.transform.position = transform.position;
        }
    }

    private void OnTriggerExit(Collider collision) {
        if (collision.gameObject.CompareTag("Glass")) {
            _positionConstraint.constraintActive = true;
            timer = 0;
        }
    }
}
