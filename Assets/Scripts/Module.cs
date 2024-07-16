using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Module : MonoBehaviour
{
    public GameObject[] faces;

    public GameObject leftFace;

    public GameObject rightFace;

    public GameObject topFace;

    public GameObject bottomFace;

    public Vector2 ThrustForce { get; private set; } = Vector2.zero;

    public Vector2 CalculateThrustForce(GameObject goal)
    {

        //Face - calculate ooclusion

        ThrustForce = Vector2.zero;

        foreach (GameObject face in faces)
        {
            if (face.activeSelf)
            {
                ThrustForce += face.GetComponent<Face>().ThrustForce;
            }
        }

        return ThrustForce;
    }
}
