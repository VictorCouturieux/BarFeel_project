using UnityEngine;
using UnityEngine.Animations;

public class GlassController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private PositionConstraint positionConstraint;
    [SerializeField] private BlobMovement blobMovement;
    [SerializeField] private ETextile textile;
    [SerializeField] private Bounds movementBounds;

    [Header("Rendering")]
    [SerializeField] private GlassRenderingManager glassRendering;

    [Header("Events")]
    [SerializeField] private VoidGameEvent stepRatioEvent;
    [SerializeField] private GameStepGameEvent newGameStepEvent;
    [SerializeField] private PlayerSoundGameEvent playerSound;
    [SerializeField] private VoidGameEvent glassReceivedEvent;

    [Header("Throw")]
    [SerializeField] private Vector3 throwDirection; // TODO: probably replace with target
    [SerializeField] private float throwForceMultiplier = 100f;
    [SerializeField] private float throwMinMagnitude = 100f;
    [SerializeField] private float throwMaxAngleDiff = 15f;
    public bool IsRaised { private set; get; }

    [Header("Steps Progress")]
    [SerializeField] private int fillStepCount;
    [SerializeField] private int cleanStepCount;
    private int currentFillStep;
    private int currentCleanStep;


    [Header("Debug")]
    [SerializeField] private bool showDebug = false;
    [SerializeField] private bool useMouseMovement = false;

    private Rigidbody rb;
	
    private void Awake() {
	    rb = GetComponent<Rigidbody>();
    }

    private void Start() {
        IsRaised = false;
        currentFillStep = 0;
        currentCleanStep = 0;
        glassRendering.SetLiquidFillRatio(0f);
    }

    private void OnEnable() {
        stepRatioEvent.AddCallback(OnStepRatio);
        newGameStepEvent.AddCallback(OnNewGameStep);
        glassReceivedEvent.AddCallback(OnGlassReceived);
    }

    private void Update() {
        UpdateThrow();
        UpdatePosition();
    }

    private void OnGlassReceived() {
        rb.velocity = Vector3.zero;
    }

    private void OnNewGameStep(GameStep step) {
        switch (step) {
            case GameStep.CLEAN:
                currentFillStep = 0;
                glassRendering.SetLiquidFillRatio(0f);
                currentCleanStep = 0;
                // TODO : set clean ratio to 0
                break;
        }
    }

    private void OnStepRatio() {
        if (IsRaised) {
            return;
        }
        
        if (GameManager.instance.CurrentStep == GameStep.FILL) {
            currentFillStep++;
            glassRendering.SetLiquidFillRatio(((float)currentFillStep / (float)fillStepCount) + 0.001f);
            playerSound.Call(PlayerSoundType.GLASS_FILL, transform.position);
            if (currentFillStep >= fillStepCount) {
                EndStep();
            }
        } else if (GameManager.instance.CurrentStep == GameStep.CLEAN) {
            currentCleanStep++;
            // TODO : increase clean ratio (somehow)
            playerSound.Call(PlayerSoundType.GLASS_WIPE, transform.position);
            if (currentCleanStep >= cleanStepCount) {
                EndStep();
            }
        }
    }

    private void EndStep() {
        GameManager.instance.NextStep();
        positionConstraint.constraintActive = true;
    }

    private void UpdateThrow() {
        if (GameManager.instance.CurrentStep == GameStep.SET && !IsRaised) {
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
        playerSound.Call(PlayerSoundType.GLASS_LAUNCH, transform.position);
        playerSound.Call(PlayerSoundType.GLASS_SLIP, transform.position);
        GameManager.instance.NextStep();
        glassRendering.FollowTargetStrictly = true;
    }

    private void UpdatePosition() {
        if (GameManager.instance.CurrentStep != GameStep.GET) {
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

    private void OnDisable() {
        stepRatioEvent.RemoveCallback(OnStepRatio);
        newGameStepEvent.RemoveCallback(OnNewGameStep);
        glassReceivedEvent.RemoveCallback(OnGlassReceived);

    }
}
