using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    [field: SerializeField]
    public GameObject Goal { get; private set; }
    
    [field:SerializeField]
    public Robot Robot { get; private set; }
}
