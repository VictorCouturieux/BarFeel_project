using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class BlobMovement : MonoBehaviour
{
    [SerializeField] private ETextile textile;
    [Min(1)]
    [SerializeField] private int trackedVelocitiesCount = 5;
    #if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool showDebug = false;
    #endif

    private bool noPos;
    private Vector2 latestBlobPos;
    private Queue<Vector2> latestVelocities;

    private void Awake() {
        latestVelocities = new Queue<Vector2>();
    }

    private void Start() {
        noPos = true;
        for (int i = 0; i < trackedVelocitiesCount; i++) {
            latestVelocities.Enqueue(Vector2.zero);
        }
    }

    private void Update() {
        Blob averageBlob;
        latestVelocities.Dequeue();
        if (textile.GetAverageBlob(out averageBlob)) {
            if (!noPos) {
                latestVelocities.Enqueue((averageBlob.centroid - latestBlobPos) / Time.deltaTime);
            } else {
                latestVelocities.Enqueue(Vector2.zero);
            }
            latestBlobPos = averageBlob.centroid;
            noPos = false;
        } else {
            latestVelocities.Enqueue(Vector2.zero);
            noPos = true;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (showDebug && (latestVelocities != null)) {
            Gizmos.DrawLine(transform.position, transform.position + ((Vector3)GetAverageVelocity()));
        }
    }
#endif

    public Vector2 GetAverageVelocity() {
        Vector2 velocitySum = Vector2.zero;

        foreach (Vector2 velocity in latestVelocities) {
            velocitySum += velocity;
        }

        return velocitySum / trackedVelocitiesCount;
    }
}
