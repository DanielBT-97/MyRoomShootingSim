using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions {
    public static void SetXPos(this Transform t, float newXPos) {
        Vector3 pos = t.position;
        pos.x = newXPos;
        t.position = pos;
    }

    public static void SetYPos(this Transform t, float newYPos) {
        Vector3 pos = t.position;
        pos.y = newYPos;
        t.position = pos;
    }

    public static void SetZPos(this Transform t, float newZPos) {
        Vector3 pos = t.position;
        pos.z = newZPos;
        t.position = pos;
    }
}
