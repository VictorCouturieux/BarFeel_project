using System.Collections.Generic;
using System.Collections;
using System.Globalization;
using UnityEngine;

public class ETextile : MonoBehaviour
{
    [SerializeField] private Vector2 textileAreaSize; // Size of area of cloth
    [SerializeField] private Vector2 blobCoordsOffset; // Offset of coordinates for blob
    [SerializeField] private bool doSwapXY; // If true, swaps X and Y axis
    [SerializeField] private bool doInvertX; // If true, reverse X axis
    [SerializeField] private bool doInvertY; // If true, reverse Y axis
    [SerializeField] private float blobMinimumDepth = 10; // Minimum depth of blob for it to be taken into account
    [SerializeField] private bool normalizeCoords = false; // Whether to normalize ALL coordinates besides depth
    [SerializeField] private float maxInactiveTime = 1.0f; // Max time before which a blob is considered 'inactive' and is removed
    [SerializeField] private float inactiveUpdateDelay = 0.1f; // Delay for inactive blob removal loop
#if UNITY_EDITOR
    [Header("Debug")]
    [SerializeField] private bool showDebug = false; // Whether to display debug gizmos or not
    [SerializeField] private bool showOnlyAverage = false; // Only show average gizmo
    [SerializeField] private bool showBlobSize = false; // Also display blob size
    [SerializeField] private Color[] debugColors; // Colors used for debug blobs
    [SerializeField] private Color averageBlobDebugColor; // Color used for debug average blob
#endif
    
    private Dictionary<int, Blob> activeBlobs;
    private Dictionary<int, float> latestBlobUpdate;
    
    private void Awake() {
        activeBlobs = new Dictionary<int, Blob>();
        latestBlobUpdate = new Dictionary<int, float>();
    }

    private IEnumerator Start() {
        while (true) {
            List<int> uids = new List<int>(activeBlobs.Keys);
            foreach (int uid in uids) {
                if (Time.time > latestBlobUpdate[uid] + maxInactiveTime) {
                    activeBlobs.Remove(uid);
                } 
            }
            yield return new WaitForSeconds(inactiveUpdateDelay);
        }
    }

    private void OnSerialValues(string[] values) {
        Blob blob;
        if (ParseBlob(values, out blob)) {
            if (blob.state && (blob.box.z > blobMinimumDepth)) {
                activeBlobs[blob.uid] = blob;
                latestBlobUpdate[blob.uid] = Time.time;
            } else {
                activeBlobs.Remove(blob.uid);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos() {
        if (showDebug) {
            Gizmos.color = Color.black;
            Vector3 debugArea;
            if (normalizeCoords) {
                debugArea = new Vector3(1, 0.01f, 1);
            } else {
                debugArea = new Vector3(textileAreaSize.x, 0.01f, textileAreaSize.y);
            }
            Gizmos.DrawCube(transform.position + debugArea/2f, debugArea);
            
            if (activeBlobs != null) {
                if (!showOnlyAverage) {
                    foreach (Blob blob in activeBlobs.Values) {
                        if (debugColors != null && debugColors.Length > 0) {
                            Gizmos.color = debugColors[blob.uid % debugColors.Length];
                        } else {
                            Gizmos.color = Color.white;
                        }
                        DrawDebugBlob(blob);
                    }
                }

                Blob averageBlob;
                if ((activeBlobs.Count > 0) && GetAverageBlob(out averageBlob)) {
                    Gizmos.color = averageBlobDebugColor;
                    DrawDebugBlob(averageBlob);
                }
            }
        }
    }

    private void DrawDebugBlob(Blob blob) {
        const float normalizedSizeMultiplier = 1f / 50f;

        float debugHeight = blob.box.z / 10f;
        if (normalizeCoords) {
            debugHeight *= normalizedSizeMultiplier;
        }

        Vector3 debugPos = new Vector3(blob.centroid.x, debugHeight / 2f, blob.centroid.y);
        Vector3 debugSize;
        if (showBlobSize) {
            debugSize = new Vector3(blob.box.x, debugHeight, blob.box.y);
        } else if (normalizeCoords) { 
            debugSize = new Vector3(normalizedSizeMultiplier, debugHeight, normalizedSizeMultiplier);
        } else {
            debugSize = new Vector3(1, debugHeight, 1);
        }
        Gizmos.DrawCube(debugPos, debugSize);
    }
#endif

    // DEBUG_FIND_BLOBS -> UID
    // LS -> lastState
    // S -> state
    // X -> centroid.X
    // Y -> centroid.Y
    // W -> box.W
    // H -> box.H
    // D -> box.D
    private bool ParseBlob(string[] values, out Blob output) {
        bool success = false;

        output = new Blob();
        foreach (string value in values) {
            string[] splitValue = value.Split(':');
            if (splitValue.Length == 2) {
                switch (splitValue[0]) {
                    case "DEBUG_FIND_BLOBS":
                        output.uid = int.Parse(splitValue[1]);
                        success = true;
                        break;
                    case "X": 
                        output.centroid.x = 58.0f - float.Parse(splitValue[1], CultureInfo.InvariantCulture);
                        success = true;
                        break;
                    case "Y":
                        output.centroid.y = float.Parse(splitValue[1], CultureInfo.InvariantCulture);
                        success = true;
                        break;
                    case "W":
                        output.box.x = int.Parse(splitValue[1]);
                        success = true;
                        break;
                    case "H":
                        output.box.y = int.Parse(splitValue[1]);
                        success = true;
                        break;
                    case "D":
                        output.box.z = int.Parse(splitValue[1]);
                        success = true;
                        break;
                    case "S":
                        output.state = (int.Parse(splitValue[1]) == 1);
                        break;
                }
            }
        }

        if (success) {
            output.centroid.x = ((output.centroid.x + blobCoordsOffset.x) % textileAreaSize.x);
            output.centroid.y = ((output.centroid.y + blobCoordsOffset.y) % textileAreaSize.y);
            if (doInvertX) {
                output.centroid.x = (textileAreaSize.x - output.centroid.x);
            }

            if (doInvertY) {
                output.centroid.y = (textileAreaSize.y - output.centroid.y);
            }

            if (doSwapXY) {
                float swapVar = output.centroid.x;
                output.centroid.x = output.centroid.y;
                output.centroid.y = swapVar;
            }

            if (normalizeCoords) {
                output.box.x /= textileAreaSize.x;
                output.box.y /= textileAreaSize.y;
                output.centroid.x /= textileAreaSize.x;
                output.centroid.y /= textileAreaSize.y;
            }
        }

        return success;
    }

    public void GetAllBlobs(List<Blob> blobs) {
        blobs.Clear();
        foreach (Blob blob in activeBlobs.Values) {
            blobs.Add(blob);
        }
    }

    public bool GetAverageBlob(out Blob output) {
        output = new Blob();
        if (activeBlobs == null) {
            return false;
        }

        int blobCount = activeBlobs.Count;
        if (blobCount == 0) {    
            return false;
        } else {
            output.state = true;
            float totalDepth = 0;

            Vector3 minCorner = Vector3.positiveInfinity;
            Vector3 maxCorner = Vector3.negativeInfinity;
            foreach (Blob blob in activeBlobs.Values) {
                output.uid = blob.uid;
                minCorner = Vector3.Min(minCorner, new Vector3(blob.centroid.x, blob.centroid.y, 0) - blob.box / 2f);
                maxCorner = Vector3.Max(maxCorner, new Vector3(blob.centroid.x, blob.centroid.y, 0) + blob.box / 2f);
                totalDepth += blob.box.z;
                output.centroid += blob.centroid * blob.box.z;
            }
            output.centroid *= (1f / totalDepth);
            output.box = maxCorner - minCorner;
            return true;
        }
    }
}
