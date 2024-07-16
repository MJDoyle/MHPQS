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
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            //Set goal position
            if (Input.GetKey(KeyCode.LeftControl))
            {
                Goal.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

                return;
            }

            Robot.AddRemoveModule((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }

    private void AddRemoveRobotModule()
    {

    }
}
