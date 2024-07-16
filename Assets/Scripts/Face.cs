using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : MonoBehaviour
{
    private GameObject activeFaceIndicator;

    public Vector2 ThrustForce { get; private set; } = Vector2.zero;

    bool occluded = false;

    // Start is called before the first frame update
    void Start()
    {
        activeFaceIndicator = transform.GetChild(0).gameObject;

        activeFaceIndicator.SetActive(false);
    }



    public void DetermineOcclusion(GameObject goal)
    {
        if (OccludedByThisModule(goal) || OccludedByOtherModules(goal))
        {
            activeFaceIndicator.SetActive(true);
            ThrustForce = -transform.right;

            Debug.DrawLine(transform.position, transform.position + 0.3f * transform.right, Color.magenta);

            //Debug.DrawRay(transform.position, transform.right, Color.magenta);

        }

        else
        {
            activeFaceIndicator.SetActive(false);
            ThrustForce = Vector2.zero;
        }
    }



    private bool OccludedByThisModule(GameObject goal)
    {
        //Goal in face space
        Vector2 goalPosition = transform.InverseTransformPoint(goal.transform.position);

        //If goal position is behind the face then it is occluded
        if (goalPosition.x > 0)
            return false;

        else
            return true;
    }

    private bool OccludedByOtherModules(GameObject goal)
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        int numHits = Physics2D.Raycast(transform.position, goal.transform.position - transform.position, new ContactFilter2D(), hits);

        //Debug.DrawRay(transform.position, goal.transform.position - transform.position, Color.magenta);

        //One hit will be the module itself
        if (numHits > 1)
            return true;

        else
            return false;
    }
}
