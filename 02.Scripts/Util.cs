using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static bool GroundCheck(Transform[] points, LayerMask layer)
    {
        int layermask = LayerMask.GetMask("Ground", "Obstacle");

        for (int i = 0; i < points.Length; i++)
        {
            if (Physics.Raycast(points[i].position, new Vector3(0, -1f, 0), 0.15f, layer))
            {
                return true;
            }
        }

        return false;
    }
}
