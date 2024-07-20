using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
    [field: SerializeField]
    public Face[] Faces { get; private set; }

    public Face leftFace;

    public Face rightFace;

    public Face topFace;

    public Face bottomFace;

    public Vector2 ThrustForce { get; private set; } = Vector2.zero;

    public Vector2 CalculateThrustForce(GameObject goal)
    {
        ThrustForce = Vector2.zero;

        foreach (Face face in Faces)
        {
            if (face.gameObject.activeSelf)
            {
                face.DetermineOcclusion(goal);

                ThrustForce += face.ThrustForce;
            }
        }

        return ThrustForce;
    }
}
