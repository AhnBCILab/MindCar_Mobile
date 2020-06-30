using UnityEngine;
using System.Collections;

public class CarControlDisactive_manual : MonoBehaviour
{
    public GameObject otherobj;

    void Start()
    {
        otherobj.GetComponent<SportCar_Controller_manual>().enabled = false;
    }


}

