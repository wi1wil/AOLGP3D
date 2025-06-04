using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    public GameObject Camera_1; 
    public GameObject Camera_2; 
    private int Manager;

    void Start()
    {
        Cam_1();
        Manager = 1;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            ManageCamera();
        }
    }

    public void ManageCamera()
    {
        if (Manager == 0)
        {
            Cam_1();
            Manager = 1;
        }
        else
        {
            Cam_2();
            Manager = 0;
        }
    }

    void Cam_1()
    {
        Camera_1.SetActive(true);
        Camera_2.SetActive(false);
    }

    void Cam_2()
    {
        Camera_1.SetActive(false);
        Camera_2.SetActive(true);
    }
}

