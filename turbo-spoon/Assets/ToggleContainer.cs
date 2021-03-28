using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleContainer : MonoBehaviour
{

    [SerializeField]
    public GameObject container;

    public void ToggleActive()
    {
        if (container.activeSelf == true)
        {
            container.SetActive(false);
        }
        else
        {
            container.SetActive(true);
        }
    }
}
