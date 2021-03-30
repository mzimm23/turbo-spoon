using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ToggleContainer : MonoBehaviour
{

    [SerializeField]
    public GameObject[] containers;

    public void ToggleActive()
    {
        foreach(GameObject obj in containers)
        {
            if (obj.activeSelf == true)
            {
                obj.SetActive(false);
            }
            else
            {
                obj.SetActive(true);
            }
        }
    }
}
