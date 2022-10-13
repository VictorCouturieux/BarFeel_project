using UnityEngine;

public class GlassRenderingManager : MonoBehaviour
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector3 raisedOffset;
    [SerializeField] private float movementSmoothingTime = 0.5f;
    [SerializeField] private float maxSpeed = 1.0f;
    
    private Vector3 offset;
    private Vector3 dampVelocity;
    [HideInInspector] public bool FollowTargetStrictly;

    private void Awake() {
        FollowTargetStrictly = false;
    }

    private void Update() {
        if (FollowTargetStrictly) {
            transform.position = targetTransform.position;
        } else {
            transform.position = Vector3.SmoothDamp(
                transform.position, 
                (targetTransform.position + offset), 
                ref dampVelocity, 
                movementSmoothingTime, 
                maxSpeed
            );
        }
    }

    public void SetRaised(bool isRaised) {
        if (isRaised) {
            offset = raisedOffset;
        } else {
            offset = Vector3.zero;
        }
    }

    // TODO : add fill functions
}
