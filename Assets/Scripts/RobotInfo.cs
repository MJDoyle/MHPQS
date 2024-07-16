using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RobotInfo : MonoBehaviour
{

    private Robot robot;

    [SerializeField]
    private Text infoText;

    // Start is called before the first frame update
    void Start()
    {
        robot = GetComponent<Robot>();
    }

    // Update is called once per frame
    void Update()
    {
        infoText.text = "Torque: " + robot.TotalTorque + " XForce: " + robot.TotalForce.x + " YDist: " + "todo" + "\nKx: " + robot.Kx + " Ky: " + robot.Ky + " Kthp: " + robot.Kth_pos + " Kthn: " + robot.Kth_neg;
    }
}
