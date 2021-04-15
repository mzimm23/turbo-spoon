using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class InteractableAdditions : MonoBehaviour
{
    [SerializeField]
    public ARPlacementInteractable placementInteractable;
    [SerializeField]
    public GameObject selectedObject;

    public void ChangePrefabSelection(GameObject gameObject)
    {
        GameObject loadedGameObject = gameObject;
        if (loadedGameObject != null)
        {
            placementInteractable.placementPrefab = loadedGameObject;
            placementInteractable.customReticle = loadedGameObject;
            Debug.Log($"Game object with name /" + loadedGameObject.name + " was loaded");
        }
        else
        {
            Debug.Log($"Unable to find a game object with name /" + gameObject.name);
        }
    }

    public void DeleteSelected()
    {
        Destroy(selectedObject.gameObject);
        selectedObject = null;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
