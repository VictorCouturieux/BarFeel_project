using UnityEngine;

public class GlassRenderingManager : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector3 raisedOffset;
    [SerializeField] private float movementSmoothingTime = 0.5f;
    [SerializeField] private float movementMaxSpeed = 2.0f;
    private Vector3 offset;
    private Vector3 dampVelocity;

    [Header("Fill")]
    [SerializeField] private Material liquidFillMaterial;
    [SerializeField] private float fillSmoothingTime = 0.25f;
    [SerializeField] private float fillMaxSpeed = 0.5f;
    private float targetFillRatio;
    private float currentFillRatio;
    private float fillDampVelocity;

    [Header("Visibility")]
    [SerializeField] private Material[] materials;
    private float[] initialMaterialsAlpha;

    [HideInInspector] public bool FollowTargetStrictly;

    private void Awake() {
        FollowTargetStrictly = false;
        currentFillRatio = 0f;
        targetFillRatio = 0f;

        initialMaterialsAlpha = new float[materials.Length];
        for (int i = 0; i < materials.Length; i++) {
            initialMaterialsAlpha[i] = materials[i].GetColor("_AlbedoColor").a;
        } 
    }

    private void Update() {
        UpdatePosition();
        UpdateFillRatio();
    }

    private void UpdatePosition() {
        float movementMaxSpeedMultiplier = 1f;
        float movementSmoothingTimeMultiplier = 1f;
        if (FollowTargetStrictly) {
            movementMaxSpeedMultiplier *= 10f;
            movementSmoothingTimeMultiplier *= 0.1f;
        }

        transform.position = Vector3.SmoothDamp(
                transform.position, 
                (targetTransform.position + offset), 
                ref dampVelocity, 
                movementSmoothingTime * movementSmoothingTimeMultiplier, 
                movementMaxSpeed * movementMaxSpeedMultiplier
            );
    } 

    private void UpdateFillRatio() {
        currentFillRatio = Mathf.SmoothDamp(
            currentFillRatio, 
            targetFillRatio, 
            ref fillDampVelocity, 
            fillSmoothingTime, 
            fillMaxSpeed
        );
        liquidFillMaterial.SetFloat("_FillRatio", currentFillRatio);
    }

    public void SetRaised(bool isRaised) {
        if (isRaised) {
            offset = raisedOffset;
        } else {
            offset = Vector3.zero;
        }
    }

    public void SetLiquidFillRatio(float ratio) {
        float fillLerpedRatio = Mathf.Lerp(0.0f, 1.01f, ratio);
        targetFillRatio = fillLerpedRatio;
    }

    public void SetVisible(bool visible) {
        if (visible) {
            for (int i = 0; i < materials.Length; i++) {
                Color color = materials[i].GetColor("_AlbedoColor");
                color.a = initialMaterialsAlpha[i];
                materials[i].SetColor("_AlbedoColor", color);
            } 
        } else {
            for (int i = 0; i < materials.Length; i++) {
                Color color = materials[i].GetColor("_AlbedoColor");
                color.a = 0;
                materials[i].SetColor("_AlbedoColor", color);
            } 
        }
    }
}
