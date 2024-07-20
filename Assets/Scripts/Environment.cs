using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    [field: SerializeField]
    public GameObject Goal { get; private set; }
    
    [field:SerializeField]
    public Robot Robot { get; private set; }

    private void Update()
    {

        //Ignore clicks on the UI

        if (Input.mousePosition.y <= 60)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            //Set goal position
            if (Input.GetKey(KeyCode.LeftControl))
            {
                Goal.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

                return;
            }

            Robot.AddRemoveModule(Camera.main.ScreenToWorldPoint(Input.mousePosition), true);
        }

        else if (Input.GetMouseButtonDown(1))
        {
            Robot.AddRemoveModule(Camera.main.ScreenToWorldPoint(Input.mousePosition), false);
        }
    }

    private void AddRemoveRobotModule()
    {

    }
}
