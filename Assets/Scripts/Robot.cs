using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{

    [SerializeField]
    private GameObject modulePrefab;

    public GameObject goal;

    private Dictionary<Vector2Int, GameObject> modules;

    //Offset of (0, 0) module position from robot CoM world position
    private Vector2 gridOffset = Vector2.zero;

    private float moduleSize = 1f;

    [SerializeField]
    private GameObject goalArrow;

    [SerializeField]
    private GameObject forceArrow;


    [SerializeField]
    private GameObject goalVelocityArrow;

    [SerializeField]
    private GameObject goalTransVelocityArrow;

    [SerializeField]
    private GameObject goalRotVelocityArrow;

    private float kx = 0;

    private float ky = 0;

    private float kthp = 0;

    private float kthn = 0;

    public float Torque { get; private set; }

    public float XForce { get; private set; }

    public float YDist { get; private set; }

    public float Kth_pos { get; private set; }

    public float Kth_neg { get; private set; }

    public float Kx { get; private set; }

    public float Ky { get; private set; }



    // Start is called before the first frame update
    void Start()
    {
        modules = new Dictionary<Vector2Int, GameObject>();
    }

    //Update controller and robot movement
    private void FixedUpdate()
    {
        if (modules.Count == 0)
        {
            goalArrow.SetActive(false);
            forceArrow.SetActive(false);
        }

        else
        {
            //calculate forces and torques on robot

            goalArrow.SetActive(true);
            forceArrow.SetActive(true);

            goalArrow.transform.right = goal.transform.position - goalArrow.transform.position;

            Vector2 totalForce = Vector2.zero;

            float totalTorque = 0;

            foreach (KeyValuePair<Vector2Int, GameObject> module in modules)
            {
                totalForce += module.Value.GetComponent<Module>().ThrustForce();

                totalTorque -= module.Value.transform.localPosition.x * module.Value.GetComponent<Module>().ThrustForce().y - module.Value.transform.localPosition.y * module.Value.GetComponent<Module>().ThrustForce().x;
            }

            XForce = totalForce.x;
            Torque = totalTorque;

            forceArrow.transform.right = transform.TransformVector(totalForce);


            //Calculate goal velocity
            Vector2 transVelocity = new Vector2(-totalForce.x * kx, -totalForce.y * ky);

            float thetaDot = 0;

            if (totalTorque >= 0)
            {
                thetaDot = totalTorque * kthp;
            }

            else
            {
                thetaDot = totalTorque * kthn;
            }



            Vector2 rotVelocity = Vector2.zero;

            Vector2 relativeGoalPos = goal.transform.position - transform.position;

            YDist = relativeGoalPos.y;

            //xdot = -rth_dotsin(th)
            //ydot = rth_dotcos(th)

            rotVelocity.x = -(relativeGoalPos).magnitude * thetaDot * Mathf.Sin(Mathf.Deg2Rad * Vector2.SignedAngle(new Vector2(1, 0), relativeGoalPos));

            rotVelocity.y = (relativeGoalPos).magnitude * thetaDot * Mathf.Cos(Mathf.Deg2Rad * Vector2.SignedAngle(new Vector2(1, 0), relativeGoalPos));




            Vector2 totalVelocity = transVelocity + rotVelocity;

            Vector2 normVelocity = totalVelocity.normalized;

            goalVelocityArrow.transform.right = normVelocity;


            if (rotVelocity == Vector2.zero)
            {
                goalRotVelocityArrow.SetActive(false);
            }

            else
            {
                goalRotVelocityArrow.SetActive(true);

                goalRotVelocityArrow.transform.right = rotVelocity;
            }

            Debug.Log(rotVelocity);

            goalRotVelocityArrow.transform.right = rotVelocity;

            goalTransVelocityArrow.transform.right = transVelocity;


            goalVelocityArrow.transform.position = goal.transform.position;

            goalTransVelocityArrow.transform.position = goal.transform.position;

            goalRotVelocityArrow.transform.position = goal.transform.position;

        }
    }

    void Update()
    {


        






            //Handle user input

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            //Set goal position
            if (Input.GetKey(KeyCode.LeftControl))
            {
                goal.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

                return;
            }

            Vector2 mousePositionInWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 mousePositionInRobot = transform.InverseTransformPoint(mousePositionInWorld);

            Vector2 mousePositionInModuleGrid = mousePositionInRobot - gridOffset;

            Vector2Int mousePositionInModuleCoords = new Vector2Int(Mathf.FloorToInt(mousePositionInModuleGrid.x / moduleSize + 0.5f), Mathf.FloorToInt(mousePositionInModuleGrid.y / moduleSize + 0.5f));

            bool moduleInThisPosition = false;

            if (modules.ContainsKey(mousePositionInModuleCoords))
                moduleInThisPosition = true;

            bool moduleNeighbouringThisPosition = false;

            if (modules.ContainsKey(mousePositionInModuleCoords + new Vector2Int(1, 0)))
                moduleNeighbouringThisPosition = true;

            if (modules.ContainsKey(mousePositionInModuleCoords + new Vector2Int(-1, 0)))
                moduleNeighbouringThisPosition = true;

            if (modules.ContainsKey(mousePositionInModuleCoords + new Vector2Int(0, 1)))
                moduleNeighbouringThisPosition = true;

            if (modules.ContainsKey(mousePositionInModuleCoords + new Vector2Int(0, -1)))
                moduleNeighbouringThisPosition = true;

            Vector2 modulePositionInRobot = (Vector2)mousePositionInModuleCoords * moduleSize;


            //If there is a neighbour tile (or no tiles at all) and no tile in this position already, then add a tile
            if (Input.GetMouseButtonDown(0) && !moduleInThisPosition && (moduleNeighbouringThisPosition || modules.Count == 0))
            {
                modules[mousePositionInModuleCoords] = Instantiate(modulePrefab, modulePositionInRobot, transform.rotation, transform);
            }

            //If there is a module in this position, delete it
            if (Input.GetMouseButtonDown(1) && moduleInThisPosition)
            {
                Destroy(modules[mousePositionInModuleCoords]);
                modules.Remove(mousePositionInModuleCoords);
            }

            //Update module positions, robot com etc.

            //Find average world position of modules
            Vector3 averagePos = Vector3.zero;

            foreach (KeyValuePair<Vector2Int, GameObject> module in modules)
            {
                averagePos += module.Value.transform.position;
            }

            averagePos /= modules.Count;

            //Set robot position to this
            Vector3 deltaVector = averagePos - transform.position;

            transform.position += deltaVector;

            //and shift all module positions accordingly
            foreach (KeyValuePair<Vector2Int, GameObject> module in modules)
            {
                module.Value.transform.position -= deltaVector;
            }

            gridOffset -= (Vector2)deltaVector;



            //Update faces
            foreach (KeyValuePair<Vector2Int, GameObject> module in modules)
            {
                module.Value.GetComponent<Module>().rightFace.SetActive(!modules.ContainsKey(module.Key + new Vector2Int(1, 0)));

                module.Value.GetComponent<Module>().leftFace.SetActive(!modules.ContainsKey(module.Key + new Vector2Int(-1, 0)));

                module.Value.GetComponent<Module>().topFace.SetActive(!modules.ContainsKey(module.Key + new Vector2Int(0, 1)));

                module.Value.GetComponent<Module>().bottomFace.SetActive(!modules.ContainsKey(module.Key + new Vector2Int(0, -1)));
            }

            //Calculate K values

            //Translational

            int xfaces = 0;
            int yfaces = 0;

            //number of x faces
            foreach (KeyValuePair<Vector2Int, GameObject> module in modules)
            {
                if (!modules.ContainsKey(module.Key + new Vector2Int(1, 0)))
                {
                    xfaces++;
                }

                if (!modules.ContainsKey(module.Key + new Vector2Int(0, 1)))
                {
                    yfaces++;
                }
            }

            Debug.Log("k_x: " + 1f / (float)xfaces);
            Debug.Log("k_y: " + 1f / (float)yfaces);

            kx = 1f / (float)xfaces;
            ky = 1f / (float)yfaces;

            Kx = kx;
            Ky = ky;


            //Rotational +

            float kRotP = 0;

            Debug.Log("Calculating k rot");

            foreach (KeyValuePair<Vector2Int, GameObject> module in modules)
            {

                Vector2 crPos = module.Key + gridOffset;

                //+x face
                if (!modules.ContainsKey(module.Key + new Vector2Int(1, 0)) && crPos.y > 0)
                {
                    Vector2 facePos = crPos + new Vector2(0.5f, 0);

                    kRotP += facePos.y * facePos.y / facePos.magnitude;

                    Debug.Log("kRotP1");
                    Debug.Log(kRotP);
                    Debug.Log(facePos);
                }

                //-x face
                if (!modules.ContainsKey(module.Key + new Vector2Int(-1, 0)) && crPos.y < 0)
                {
                    Vector2 facePos = crPos + new Vector2(-0.5f, 0);

                    kRotP += facePos.y * facePos.y / facePos.magnitude;

                    Debug.Log("kRotP2");
                    Debug.Log(kRotP);
                    Debug.Log(facePos);
                }

                //+y face
                if (!modules.ContainsKey(module.Key + new Vector2Int(0, 1)) && crPos.x < 0)
                {
                    Vector2 facePos = crPos + new Vector2(0, 0.5f);

                    kRotP += facePos.x * facePos.x / facePos.magnitude;

                    Debug.Log("kRotP3");
                    Debug.Log(kRotP);
                    Debug.Log(facePos);
                }

                //-y face
                if (!modules.ContainsKey(module.Key + new Vector2Int(0, -1)) && crPos.x > 0)
                {
                    Vector2 facePos = crPos + new Vector2(0, -0.5f);

                    kRotP += facePos.x * facePos.x / facePos.magnitude;

                    Debug.Log("kRotP4");
                    Debug.Log(kRotP);
                    Debug.Log(facePos);
                }
            }


            if (kRotP != 0)
                kRotP = 1f / kRotP;

            Debug.Log("k_th+: " + kRotP);

            kthp = kRotP;

            Kth_pos = kthp;


            //Rotational -

            float kRotN = 0;

            foreach (KeyValuePair<Vector2Int, GameObject> module in modules)
            {

                Vector2 crPos = module.Key + gridOffset;

                //+x face
                if (!modules.ContainsKey(module.Key + new Vector2Int(1, 0)) && crPos.y < 0)
                {
                    Vector2 facePos = crPos + new Vector2(0.5f, 0);

                    kRotN += facePos.y * facePos.y / facePos.magnitude;

                    Debug.Log("kRotN1");
                    Debug.Log(kRotN);
                }

                //-x face
                if (!modules.ContainsKey(module.Key + new Vector2Int(-1, 0)) && crPos.y > 0)
                {
                    Vector2 facePos = crPos + new Vector2(-0.5f, 0);

                    kRotN += facePos.y * facePos.y / facePos.magnitude;

                    Debug.Log("kRotN2");
                    Debug.Log(kRotN);
                }

                //+y face
                if (!modules.ContainsKey(module.Key + new Vector2Int(0, 1)) && crPos.x > 0)
                {
                    Vector2 facePos = crPos + new Vector2(0, 0.5f);

                    kRotN += facePos.x * facePos.x / facePos.magnitude;

                    Debug.Log("kRotN3");
                    Debug.Log(kRotN);
                }

                //-y face
                if (!modules.ContainsKey(module.Key + new Vector2Int(0, -1)) && crPos.x < 0)
                {
                    Vector2 facePos = crPos + new Vector2(0, -0.5f);

                    kRotN += facePos.x * facePos.x / facePos.magnitude;

                    Debug.Log("kRotN4");
                    Debug.Log(kRotN);
                }
            }

            if (kRotN != 0)
                kRotN = 1f / kRotN;

            Debug.Log("k_th-: " + kRotN);


            kthn = kRotN;


            Kth_neg = kthn;

        }
    }
}
