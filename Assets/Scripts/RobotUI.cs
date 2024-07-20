using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotUI : MonoBehaviour
{
    [SerializeField]
    private Environment environment;

    [SerializeField]
    private GameObject goalArrow;
    [SerializeField]
    private GameObject forceArrow;

    [SerializeField]
    private GameObject geomGoalArrow;
    [SerializeField]
    private GameObject geomForceArrow;

    [SerializeField]
    private GameObject goalVelocityArrow;
    [SerializeField]
    private GameObject goalTransVelocityArrow;
    [SerializeField]
    private GameObject goalRotVelocityArrow;

    private bool showGoalArrows = true;

    private bool showGeomArrows = true;

    private bool showComArrows = true;

    private void FixedUpdate()
    {
        goalArrow.SetActive(false);
        forceArrow.SetActive(false);
        geomGoalArrow.SetActive(false);
        geomForceArrow.SetActive(false);
        goalVelocityArrow.SetActive(false);
        goalTransVelocityArrow.SetActive(false);
        goalRotVelocityArrow.SetActive(false);

        if (environment.Robot.Modules.Count == 0)
            return;

        if (showGoalArrows)
        {
            goalVelocityArrow.SetActive(true);
            goalTransVelocityArrow.SetActive(true);
            goalRotVelocityArrow.SetActive(true);
        }

        if (showGeomArrows)
        {
            geomGoalArrow.SetActive(true);
            geomForceArrow.SetActive(true);
        }

        if (showComArrows)
        {
            goalArrow.SetActive(true);
            forceArrow.SetActive(true);
        }

        goalArrow.transform.right = environment.Goal.transform.position - goalArrow.transform.position;

        geomGoalArrow.transform.position = environment.Robot.transform.TransformPoint(environment.Robot.GeometricCenter);

        geomGoalArrow.transform.right = environment.Goal.transform.position - environment.Robot.transform.TransformPoint(environment.Robot.GeometricCenter);




        forceArrow.transform.right = transform.TransformVector(environment.Robot.TotalForce);

        geomForceArrow.transform.position = environment.Robot.transform.TransformPoint(environment.Robot.GeometricCenter);

        geomForceArrow.transform.right = transform.TransformVector(environment.Robot.TotalForce);




        //Calculate goal velocity
        Vector2 transVelocity = new Vector2(-environment.Robot.TotalForce.x * environment.Robot.Kx, -environment.Robot.TotalForce.y * environment.Robot.Ky);

        float thetaDot = 0;

        if (environment.Robot.TotalTorque >= 0)
        {
            thetaDot = environment.Robot.TotalTorque * environment.Robot.Kth_pos;
        }

        else
        {
            thetaDot = environment.Robot.TotalTorque * environment.Robot.Kth_neg;
        }

        Vector2 rotVelocity = Vector2.zero;

        Vector2 relativeGoalPos = environment.Goal.transform.position - environment.Robot.transform.position;

        rotVelocity.x = -(relativeGoalPos).magnitude * thetaDot * Mathf.Sin(Mathf.Deg2Rad * Vector2.SignedAngle(new Vector2(1, 0), relativeGoalPos));

        rotVelocity.y = (relativeGoalPos).magnitude * thetaDot * Mathf.Cos(Mathf.Deg2Rad * Vector2.SignedAngle(new Vector2(1, 0), relativeGoalPos));

        Vector2 totalVelocity = transVelocity + rotVelocity;

        Vector2 normVelocity = totalVelocity.normalized;




        goalVelocityArrow.transform.right = normVelocity;


        if (rotVelocity == Vector2.zero)
        {
            goalRotVelocityArrow.SetActive(false);
        }

        else if (showGoalArrows)
        {
            goalRotVelocityArrow.SetActive(true);

            goalRotVelocityArrow.transform.right = rotVelocity;
        }

        //Debug.Log(rotVelocity);

        goalRotVelocityArrow.transform.right = rotVelocity;

        goalTransVelocityArrow.transform.right = transVelocity;


        goalVelocityArrow.transform.position = environment.Goal.transform.position;

        goalTransVelocityArrow.transform.position = environment.Goal.transform.position;

        goalRotVelocityArrow.transform.position = environment.Goal.transform.position;

    }

    public void ToggleGoalArrows()
    {
        showGoalArrows = !showGoalArrows;
    }

    public void ToggleGeomArrows()
    {
        showGeomArrows = !showGeomArrows;
    }

    public void ToggleComArrows()
    {
        showComArrows = !showComArrows;
    }
}
