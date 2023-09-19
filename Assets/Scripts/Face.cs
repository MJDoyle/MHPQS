using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Face : MonoBehaviour
{
    private GameObject activeFaceIndicator;

    private GameObject goal;

    public Vector2 thrustForce = Vector2.zero;

    bool occluded = false;

    // Start is called before the first frame update
    void Start()
    {
        activeFaceIndicator = transform.GetChild(0).gameObject;

        activeFaceIndicator.SetActive(false);

        goal = GetComponentInParent<Robot>().goal;
    }


    //Update the controller
    private void FixedUpdate()
    {
        CheckOcclusionByThisModule();

        if (!occluded)
            CheckOcclusionByOtherModules();

        if (occluded)
        {
            activeFaceIndicator.SetActive(true);
            thrustForce = -transform.right;

            Debug.DrawLine(transform.position, transform.position + 0.3f * transform.right, Color.magenta);

            //Debug.DrawRay(transform.position, transform.right, Color.magenta);

        }

        else
        {
            activeFaceIndicator.SetActive(false);
            thrustForce = Vector2.zero;
        }
    }


    private void CheckOcclusionByThisModule()
    {
        //Goal in face space
        Vector2 goalPosition = transform.InverseTransformPoint(goal.transform.position);

        //If goal position is behind the face then it is occluded
        if (goalPosition.x > 0)
            occluded = false;

        else
            occluded = true;
    }

    private void CheckOcclusionByOtherModules()
    {
        List<RaycastHit2D> hits = new List<RaycastHit2D>();

        int numHits = Physics2D.Raycast(transform.position, goal.transform.position - transform.position, new ContactFilter2D(), hits);

        //Debug.DrawRay(transform.position, goal.transform.position - transform.position, Color.magenta);

        //One hit will be the module itself
        if (numHits > 1)
            occluded = true;

        else
            occluded = false;
    }
}
