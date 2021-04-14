using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;


[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlaneV2 : MonoBehaviour
{
    [SerializeField]
    private GameObject placedPrefab;

    [SerializeField]
    private Camera arCamera;

    private PlacementObject[] placedObjects;

    private Vector2 touchPosition = default;

    private ARRaycastManager arRaycastManager;

    //private bool canDelete = true;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private PlacementObject lastSelectedObject;

    Vector3 initialHit;

    private GameObject PlacedPrefab
    {
        get
        {
            return placedPrefab;
        }
        set
        {
            placedPrefab = value;
        }
    }


    void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();

    }

    public void ChangePrefabSelection(GameObject gameObject)
    {
        UpdateLog("Trying to change object to " + gameObject.name);
        GameObject loadedGameObject = gameObject;
        if (loadedGameObject != null)
        {
            PlacedPrefab = loadedGameObject;
            Debug.Log($"Game object with name /" + placedPrefab.name + " was loaded");
            UpdateLog($"Game object with name /" + placedPrefab.name + " was loaded");
        }
        else
        {
            Debug.Log($"Unable to find a game object with name /" + gameObject.name);
        }
    }


    void Update()
    {
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (IsPointerOverUI(touch)) return;
            touchPosition = touch.position;
            if (touch.phase == TouchPhase.Began)
            {
                UpdateLog("Touch Began");
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                if (Physics.Raycast(ray, out hitObject))
                {
                    UpdateLog("     Hit: " + hitObject.transform.gameObject.name.ToString());
                    initialHit = hitObject.point;
                    
                    if(lastSelectedObject == hitObject.transform.GetComponent<PlacementObject>() && lastSelectedObject != null)
                    {
                        if (lastSelectedObject.Selected == true)
                        {
                            lastSelectedObject.Selected = false;
                            lastSelectedObject.ToggleSelectedIndicator();
                            //canDelete = false; // Experimental
                            UpdateLog("         Deselected: "+lastSelectedObject);
                            UpdateLog("         LastSelectedObject: " + lastSelectedObject.name.ToString());
                            return;
                        }
                        else
                        {
                            lastSelectedObject.Selected = true;
                            lastSelectedObject.ToggleSelectedIndicator();
                            //canDelete = true; // Experimental
                            UpdateLog("         Re-Selected: " + lastSelectedObject);
                        }
                    }
                    if (hitObject.transform.GetComponent<PlacementObject>() == null)
                    {
                        UpdateLog("         DIDN'T HIT AN OBJECT");
                    }
                    lastSelectedObject = hitObject.transform.GetComponent<PlacementObject>();
                    if (lastSelectedObject != null)
                    {
                        text.text = "The current selected object is: " + lastSelectedObject.name;
                        PlacementObject[] allOtherObjects = FindObjectsOfType<PlacementObject>();
                        foreach (PlacementObject placementObject in allOtherObjects)
                        {
                            placementObject.Selected = placementObject == lastSelectedObject;
                        }
                    }//Maybe add return here if unselect doesnt work
                }
                else
                {
                    UpdateLog("Deselected "+lastSelectedObject);
                    //canDelete = false; //Experimental
                    lastSelectedObject.Selected = false;
                    lastSelectedObject.ToggleSelectedIndicator(); //Not sure if this one will work
                }
            }   
            if (touch.phase == TouchPhase.Ended)
            {
                UpdateLog("Touch Ended");
                //lastSelectedObject.Selected = false; original location
            }
            //Moved this inside v
            if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                if (lastSelectedObject == null)
                {
                    UpdateLog("     LastSelected Before Spawn: "+lastSelectedObject);
                    lastSelectedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation).GetComponent<PlacementObject>();
                    //canDelete = true; //Experimantal
                    UpdateLog("     LastSelected After Spawn: " + lastSelectedObject);
                }
                else
                {
                    if (lastSelectedObject.Selected)
                    {
                        UpdateLog("- Trying to update position of "+lastSelectedObject.ToString());
                        //lastSelectedObject.transform.position = lastSelectedObject.transform.position + (hitPose.position - initialHit);
                        lastSelectedObject.transform.position = hitPose.position;
                        lastSelectedObject.transform.rotation = hitPose.rotation;
                    }
                }
            }

        }
        ///text.text = "The current selected object is: " + lastSelectedObject.name;
    }

    [SerializeField] Text logText, text;
    public void UpdateLog(string text)
    {
        logText.text += "\n" + text;
    }
    
    bool IsPointerOverUI(Touch touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touch.position.x, touch.position.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    public void DeleteSelected()
    {
        if(lastSelectedObject.Selected) //If this doesnt work try changing the if to if(lastSelectedObject == defaultObject)
        {
            Destroy(lastSelectedObject.gameObject);
            UpdateLog("     Deleted: " + lastSelectedObject);
            lastSelectedObject = null;
        }
    }

}