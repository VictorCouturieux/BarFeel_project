using UnityEngine;

public struct Blob {
    public int uid; // Unique ID of blob
    public Vector2 centroid; // centroid
    public Vector3 box;
    public bool state; // whether the blob is present
}

/*

struct blob {
  lnode_t node;
  uint8_t UID;
  status_t status;
  uint32_t timeTag;
  uint16_t pixels;
  boolean state;
  boolean lastState;
  box_t box;
  point_t centroid;
};


Serial.printf("\nDEBUG_FIND_BLOBS:%d\tLS:%d\tS:%d\tX:%f\tY:%f\tW:%d\tH:%d\tD:%d\t",
                blob->UID,
                blob->lastState,
                blob->state,
                blob->centroid.X,
                blob->centroid.Y,
                blob->box.W,
                blob->box.H,
                blob->box.D
                );

*/