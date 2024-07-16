using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{

    [SerializeField]
    private Environment environment;

    [SerializeField]
    private Module modulePrefab;

    public Dictionary<Vector2Int, Module> Modules { get; private set; }

    //Offset of (0, 0) module position from robot CoM world position
    private Vector2 gridOffset = Vector2.zero;

    private float moduleSize = 1f;



    public Vector2 GeometricCenter { get; private set; } = Vector2.zero;



    public Vector2 TotalForce { get; private set; } = Vector2.zero;

    public float TotalTorque { get; private set; }



    public float YDist { get; private set; } = 0;

    public float Kth_pos { get; private set; } = 0;

    public float Kth_neg { get; private set; } = 0;

    public float Kx { get; private set; } = 0;

    public float Ky { get; private set; } = 0;



    // Start is called before the first frame update
    void Start()
    {
        Modules = new Dictionary<Vector2Int, Module>();
    }

    //Update controller and robot movement
    private void FixedUpdate()
    {
        if (Modules.Count == 0)
            return;

        foreach (KeyValuePair<Vector2Int, Module> module in Modules)
        {
            module.Value.CalculateThrustForce(environment.Goal);
        }

        //calculate forces and torques on robot

        TotalForce = Vector2.zero;

        TotalTorque = 0;

        foreach (KeyValuePair<Vector2Int, Module> module in Modules)
        {
            TotalForce += module.Value.GetComponent<Module>().ThrustForce;

            TotalTorque -= module.Value.transform.localPosition.x * module.Value.GetComponent<Module>().ThrustForce.y - module.Value.transform.localPosition.y * module.Value.GetComponent<Module>().ThrustForce.x;
        }


        //Calculate goal velocity
        Vector2 transVelocity = new Vector2(-TotalForce.x * Kx, -TotalForce.y * Ky);

        float thetaDot = 0;

        if (TotalTorque >= 0)
        {
            thetaDot = TotalTorque * Kth_pos;
        }

        else
        {
            thetaDot = TotalTorque * Kth_neg;
        }

        Vector2 rotVelocity = Vector2.zero;

        Vector2 relativeGoalPos = environment.Goal.transform.position - transform.position;

        YDist = relativeGoalPos.y;

        rotVelocity.x = -(relativeGoalPos).magnitude * thetaDot * Mathf.Sin(Mathf.Deg2Rad * Vector2.SignedAngle(new Vector2(1, 0), relativeGoalPos));

        rotVelocity.y = (relativeGoalPos).magnitude * thetaDot * Mathf.Cos(Mathf.Deg2Rad * Vector2.SignedAngle(new Vector2(1, 0), relativeGoalPos));

        Vector2 totalVelocity = transVelocity + rotVelocity;

        Vector2 normVelocity = totalVelocity.normalized;


        
    }



    void CalculateGeometricCenter()
    {
        //Update geometric center

        if (Modules.Count > 0)
        {
            float minX = 0;
            float maxX = 0;
            float minY = 0;
            float maxY = 0;

            foreach (KeyValuePair<Vector2Int, Module> module in Modules)
            {
                minX = module.Value.transform.position.x;

                maxX = module.Value.transform.position.x;

                minY = module.Value.transform.position.y;

                maxY = module.Value.transform.position.y;

                break;
            }

            foreach (KeyValuePair<Vector2Int, Module> module in Modules)
            {
                if (module.Value.transform.position.x < minX)
                    minX = module.Value.transform.position.x;

                if (module.Value.transform.position.x > maxX)
                    maxX = module.Value.transform.position.x;

                if (module.Value.transform.position.y < minY)
                    minY = module.Value.transform.position.y;

                if (module.Value.transform.position.y > maxY)
                    maxY = module.Value.transform.position.y;

            }

            GeometricCenter = new Vector2((minX + maxX) / 2f, (minY + maxY) / 2f);
        }
    }

    void CalculateKValues()
    {
        //Calculate K values

        //Translational

        int xfaces = 0;
        int yfaces = 0;

        //number of x faces
        foreach (KeyValuePair<Vector2Int, Module> module in Modules)
        {
            if (!Modules.ContainsKey(module.Key + new Vector2Int(1, 0)))
            {
                xfaces++;
            }

            if (!Modules.ContainsKey(module.Key + new Vector2Int(0, 1)))
            {
                yfaces++;
            }
        }

        Debug.Log("k_x: " + 1f / (float)xfaces);
        Debug.Log("k_y: " + 1f / (float)yfaces);

        Kx = 1f / (float)xfaces;
        Ky = 1f / (float)yfaces;

        //Rotational +

        float kRotP = 0;

        Debug.Log("Calculating k rot");

        foreach (KeyValuePair<Vector2Int, Module> module in Modules)
        {

            Vector2 crPos = module.Key + gridOffset;

            //+x face
            if (!Modules.ContainsKey(module.Key + new Vector2Int(1, 0)) && crPos.y > 0)
            {
                Vector2 facePos = crPos + new Vector2(0.5f, 0);

                kRotP += facePos.y * facePos.y / facePos.magnitude;

                Debug.Log("kRotP1");
                Debug.Log(kRotP);
                Debug.Log(facePos);
            }

            //-x face
            if (!Modules.ContainsKey(module.Key + new Vector2Int(-1, 0)) && crPos.y < 0)
            {
                Vector2 facePos = crPos + new Vector2(-0.5f, 0);

                kRotP += facePos.y * facePos.y / facePos.magnitude;

                Debug.Log("kRotP2");
                Debug.Log(kRotP);
                Debug.Log(facePos);
            }

            //+y face
            if (!Modules.ContainsKey(module.Key + new Vector2Int(0, 1)) && crPos.x < 0)
            {
                Vector2 facePos = crPos + new Vector2(0, 0.5f);

                kRotP += facePos.x * facePos.x / facePos.magnitude;

                Debug.Log("kRotP3");
                Debug.Log(kRotP);
                Debug.Log(facePos);
            }

            //-y face
            if (!Modules.ContainsKey(module.Key + new Vector2Int(0, -1)) && crPos.x > 0)
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

        Kth_pos = kRotP;

        //Rotational -

        float kRotN = 0;

        foreach (KeyValuePair<Vector2Int, Module> module in Modules)
        {

            Vector2 crPos = module.Key + gridOffset;

            //+x face
            if (!Modules.ContainsKey(module.Key + new Vector2Int(1, 0)) && crPos.y < 0)
            {
                Vector2 facePos = crPos + new Vector2(0.5f, 0);

                kRotN += facePos.y * facePos.y / facePos.magnitude;

                Debug.Log("kRotN1");
                Debug.Log(kRotN);
            }

            //-x face
            if (!Modules.ContainsKey(module.Key + new Vector2Int(-1, 0)) && crPos.y > 0)
            {
                Vector2 facePos = crPos + new Vector2(-0.5f, 0);

                kRotN += facePos.y * facePos.y / facePos.magnitude;

                Debug.Log("kRotN2");
                Debug.Log(kRotN);
            }

            //+y face
            if (!Modules.ContainsKey(module.Key + new Vector2Int(0, 1)) && crPos.x > 0)
            {
                Vector2 facePos = crPos + new Vector2(0, 0.5f);

                kRotN += facePos.x * facePos.x / facePos.magnitude;

                Debug.Log("kRotN3");
                Debug.Log(kRotN);
            }

            //-y face
            if (!Modules.ContainsKey(module.Key + new Vector2Int(0, -1)) && crPos.x < 0)
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

        Kth_neg = kRotN;
    }


    public void AddRemoveModule(Vector2 mousePositionInWorld)
    {
        Vector2 mousePositionInRobot = transform.InverseTransformPoint(mousePositionInWorld);

        Vector2 mousePositionInModuleGrid = mousePositionInRobot - gridOffset;

        Vector2Int mousePositionInModuleCoords = new Vector2Int(Mathf.FloorToInt(mousePositionInModuleGrid.x / moduleSize + 0.5f), Mathf.FloorToInt(mousePositionInModuleGrid.y / moduleSize + 0.5f));

        bool moduleInThisPosition = false;

        if (Modules.ContainsKey(mousePositionInModuleCoords))
            moduleInThisPosition = true;

        bool moduleNeighbouringThisPosition = false;

        if (Modules.ContainsKey(mousePositionInModuleCoords + new Vector2Int(1, 0)))
            moduleNeighbouringThisPosition = true;

        if (Modules.ContainsKey(mousePositionInModuleCoords + new Vector2Int(-1, 0)))
            moduleNeighbouringThisPosition = true;

        if (Modules.ContainsKey(mousePositionInModuleCoords + new Vector2Int(0, 1)))
            moduleNeighbouringThisPosition = true;

        if (Modules.ContainsKey(mousePositionInModuleCoords + new Vector2Int(0, -1)))
            moduleNeighbouringThisPosition = true;

        Vector2 modulePositionInRobot = (Vector2)mousePositionInModuleCoords * moduleSize;


        //If there is a neighbour tile (or no tiles at all) and no tile in this position already, then add a tile
        if (Input.GetMouseButtonDown(0) && !moduleInThisPosition && (moduleNeighbouringThisPosition || Modules.Count == 0))
        {
            Modules[mousePositionInModuleCoords] = Instantiate(modulePrefab, modulePositionInRobot, transform.rotation, transform);
        }

        //If there is a module in this position, delete it
        if (Input.GetMouseButtonDown(1) && moduleInThisPosition)
        {
            Destroy(Modules[mousePositionInModuleCoords]);
            Modules.Remove(mousePositionInModuleCoords);
        }

        //Update module positions, robot com etc.

        //Find average world position of modules
        Vector3 averagePos = Vector3.zero;

        foreach (KeyValuePair<Vector2Int, Module> module in Modules)
        {
            averagePos += module.Value.transform.position;
        }

        averagePos /= Modules.Count;

        //Set robot position to this
        Vector3 deltaVector = averagePos - transform.position;

        transform.position += deltaVector;

        //and shift all module positions accordingly
        foreach (KeyValuePair<Vector2Int, Module> module in Modules)
        {
            module.Value.transform.position -= deltaVector;
        }

        gridOffset -= (Vector2)deltaVector;

        //Set internal faces as inactive and external faces as active
        foreach (KeyValuePair<Vector2Int, Module> module in Modules)
        {
            module.Value.rightFace.SetActive(!Modules.ContainsKey(module.Key + new Vector2Int(1, 0)));

            module.Value.leftFace.SetActive(!Modules.ContainsKey(module.Key + new Vector2Int(-1, 0)));

            module.Value.topFace.SetActive(!Modules.ContainsKey(module.Key + new Vector2Int(0, 1)));

            module.Value.bottomFace.SetActive(!Modules.ContainsKey(module.Key + new Vector2Int(0, -1)));
        }

        CalculateKValues();

        CalculateGeometricCenter();
    }

}
