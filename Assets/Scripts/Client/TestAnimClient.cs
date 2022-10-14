using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimClient : MonoBehaviour
{
    
    [SerializeField] private Animator _testClientAnimator;

    public void ClientUp() {
        _testClientAnimator.SetTrigger("ClientCome");
    }
    
    public void ClientDown() {
        _testClientAnimator.SetTrigger("ClientBack");
    }

    public void EndAnim() {
        GameManager.instance.NextStep();
    }

}
