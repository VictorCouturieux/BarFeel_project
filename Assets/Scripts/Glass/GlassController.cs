using UnityEngine;

public class GlassController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private BlobMovement blobMovement;
    [SerializeField] private ETextile textile;
    [SerializeField] private Bounds movementBounds;

    [SerializeField] private GlassRenderingManager glassRendering;
    [SerializeField] private Vector3 throwDirection; // TODO: probably replace with target
    [SerializeField] private float throwForceMultiplier = 100f;
    [SerializeField] private float throwMinMagnitude = 100f;
    [SerializeField] private float throwMaxAngleDiff = 15f;
    public bool IsRaised { private set; get; }

    [Header("Debug")]
    [SerializeField] private bool showDebug = false;
    [SerializeField] private bool useMouseMovement = false;

    private Rigidbody rb;
	
    private void Awake() {
	    rb = GetComponent<Rigidbody>();
        IsRaised = false;
    }

    private void Update() {
        UpdateThrow();
        UpdatePosition();
    }

    private void UpdateThrow() {
        if (GameManager.instance.CurrentStep == GameManager.GameStep.SET) {
            if (useMouseMovement) {
                if (Input.GetMouseButton(0)) {
                    Throw();
                }
            } else {
                Vector2 blobAverageVelocity = blobMovement.GetAverageVelocity();
                if ((Vector2.Angle(blobAverageVelocity, throwDirection) < throwMaxAngleDiff) &&
                    (blobAverageVelocity.magnitude >= throwMinMagnitude)) {
                    Throw();
                }
            }
        }
    }

    private void Throw() {
        rb.AddForce(throwDirection * throwForceMultiplier);
        GameManager.instance.NextStep();
        glassRendering.FollowTargetStrictly = true;
    }

    private void UpdatePosition() {
        if (GameManager.instance.CurrentStep != GameManager.GameStep.GET) {
            glassRendering.FollowTargetStrictly = false;
            rb.velocity = Vector3.zero;

            if (useMouseMovement) {
                if ((Input.mousePosition.x < Screen.width) && (Input.mousePosition.y < Screen.height)) {
                    SetRaised(false);
                    Vector2 normalizedMousePos = new Vector2(
                        Input.mousePosition.x / Screen.width,
                        Input.mousePosition.y / Screen.height
                    );
                    transform.position = GetInBoundsPosition(normalizedMousePos);
                } else {
                    SetRaised(true);
                }
            } else {
                Blob averageBlob;
                if (textile.GetAverageBlob(out averageBlob)) {
                    SetRaised(false);
                    transform.position = GetInBoundsPosition(averageBlob.centroid);
                } else {
                    SetRaised(true);
                }
            }
        } else {
            glassRendering.FollowTargetStrictly = true;
        }
    }

    private Vector3 GetInBoundsPosition(Vector2 normalizedPosition) {
        Vector3 minCorner = movementBounds.min;
        Vector3 areaSize = movementBounds.max - minCorner;
        Vector3 originOffset = new Vector3(areaSize.x * normalizedPosition.x, 0, areaSize.z * normalizedPosition.y);
        return minCorner + originOffset;
    }

    private void SetRaised(bool isRaised) {
        IsRaised = isRaised;
        glassRendering.SetRaised(isRaised);
    }

    private void OnDrawGizmos() {
        if (showDebug) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(movementBounds.center, movementBounds.size);
        }
    }
}
