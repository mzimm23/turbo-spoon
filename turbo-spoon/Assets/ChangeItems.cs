using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeItems : MonoBehaviour
{
    [SerializeField]
    public GameObject activeButton;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeMenu(GameObject newObj)
    {
        activeButton.SetActive(false);
        newObj.SetActive(true);
        activeButton = newObj;
    }

}
