using UnityEngine;
using System.Collections;

public class CarControlDisactive_auto : MonoBehaviour
{
    public GameObject otherobj;

    void Start()
    {
        otherobj.GetComponent<SportCar_Controller_auto>().enabled = false;
    }


}

