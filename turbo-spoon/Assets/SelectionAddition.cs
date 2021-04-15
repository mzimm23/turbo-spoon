using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;


public class SelectionAddition : MonoBehaviour
{
    [SerializeField]
    public InteractableAdditions additionsScript;

    private void Start()
    {
        additionsScript = FindObjectOfType<InteractableAdditions>();
    }

    public void SetSelectedObject(GameObject intObj)
    {
        additionsScript.selectedObject = intObj;
    }
    public void RemoveSelectedObject()
    {
        additionsScript.selectedObject = null;
    }

}
