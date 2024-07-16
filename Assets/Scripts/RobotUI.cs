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

    private void FixedUpdate()
    {
        if (environment.Robot.Modules.Count == 0)    
        {
            //goalArrow.SetActive(false);
            //forceArrow.SetActive(false);
            //geomGoalArrow.SetActive(false);
            //geomForceArrow.SetActive(false);

            return;
        }
   
        //goalArrow.SetActive(true);
        //forceArrow.SetActive(true);
        //geomGoalArrow.SetActive(true);
        //geomForceArrow.SetActive(true);


        //goalArrow.transform.right = environment.Goal.transform.position - goalArrow.transform.position;

        //geomGoalArrow.transform.position = environment.Robot.GeometricCenter;

        //geomGoalArrow.transform.right = (Vector2)environment.Goal.transform.position - environment.Robot.GeometricCenter;




        //forceArrow.transform.right = transform.TransformVector(environment.Robot.TotalForce);

        //geomForceArrow.transform.position = environment.Robot.GeometricCenter;

        //geomForceArrow.transform.right = transform.TransformVector(environment.Robot.TotalForce);


        //goalVelocityArrow.transform.right = normVelocity;


        //if (rotVelocity == Vector2.zero)
        //{
        //    goalRotVelocityArrow.SetActive(false);
        //}

        //else
        //{
        //    goalRotVelocityArrow.SetActive(true);

        //    goalRotVelocityArrow.transform.right = rotVelocity;
        //}

        //Debug.Log(rotVelocity);

        //goalRotVelocityArrow.transform.right = rotVelocity;

        //goalTransVelocityArrow.transform.right = transVelocity;


        //goalVelocityArrow.transform.position = environment.Goal.transform.position;

        //goalTransVelocityArrow.transform.position = environment.Goal.transform.position;

        //goalRotVelocityArrow.transform.position = environment.Goal.transform.position;
        
    }

}
