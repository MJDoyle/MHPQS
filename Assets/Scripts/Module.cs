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

    public Vector2 ThrustForce()
    {
        Vector2 thrustForce = Vector2.zero;

        foreach (GameObject face in faces)
        {
            if (face.activeSelf)
            {
                thrustForce += face.GetComponent<Face>().thrustForce;
            }
        }

        return thrustForce;
    }
}
