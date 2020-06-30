using UnityEngine;
using System.Collections;

public class CarControlActive_auto : MonoBehaviour
{
    public GameObject otherobj;

    void Start()
    {
        otherobj.GetComponent<SportCar_Controller_auto>().enabled = true;
    }


}

