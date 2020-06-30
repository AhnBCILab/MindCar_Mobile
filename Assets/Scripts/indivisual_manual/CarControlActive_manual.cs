using UnityEngine;
using System.Collections;

public class CarControlActive_manual : MonoBehaviour
{
    public GameObject otherobj;

    void Start()
    {
        otherobj.GetComponent<SportCar_Controller_manual>().enabled = true;
    }


}

