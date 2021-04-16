using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;


[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlaneV2 : MonoBehaviour
{
    [SerializeField]
    private GameObject placedPrefab, previewObject;

    [SerializeField]
    private Camera arCamera;

    private PlacementObject[] placedObjects;

    private Vector2 touchPosition = default;

    private ARRaycastManager arRaycastManager;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private PlacementObject lastSelectedObject;
    
    private Pose camMiddlePose;

    private bool canPlace, canMove = false;

    Vector3 initialHit;

    RaycastHit hitObject;

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
            previewObject = placedPrefab;
            canPlace = true;
        }
        else
        {
            Debug.Log($"Unable to find a game object with name /" + gameObject.name);
        }
    }


    void Update()
    {
        //text.text = "The current preview object is" + previewObject.name;
        CrosshairCalculation();
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (IsPointerOverUI(touch)) return;
            touchPosition = touch.position;
            if (touch.phase == TouchPhase.Began)
            {
                UpdateLog("Touch Began");
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                
                if (Physics.Raycast(ray, out hitObject))
                {
                    UpdateLog("     Hit: " + hitObject.transform.gameObject.name.ToString());
                    initialHit = hitObject.point;
                    
                   
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
                    //UpdateLog("Deselected "+lastSelectedObject);
                    //lastSelectedObject.Selected = false;
                    //lastSelectedObject.ToggleSelectedIndicator();
                }
            }   
            if (touch.phase == TouchPhase.Ended)
            {
                UpdateLog("Touch Ended");
            }
            if (touch.phase == TouchPhase.Stationary)
            {
                if (lastSelectedObject == hitObject.transform.GetComponent<PlacementObject>() && lastSelectedObject != null) // If you tap the selected objecct
                {
                    if (lastSelectedObject.Selected == true) // De-select
                    {
                        lastSelectedObject.Selected = false;
                        lastSelectedObject.ToggleSelectedIndicator();
                        UpdateLog("         Deselected: " + lastSelectedObject);
                        UpdateLog("         LastSelectedObject: " + lastSelectedObject.name.ToString());
                        return;
                    }
                    else // Select
                    {
                        lastSelectedObject.Selected = true;
                        lastSelectedObject.ToggleSelectedIndicator();
                        UpdateLog("         Re-Selected: " + lastSelectedObject);
                        return; // Not sure if this will work or break it
                    }
                }
            }
            if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                if (lastSelectedObject == null)
                {
                    UpdateLog("     LastSelected Before Spawn: "+lastSelectedObject);
                    lastSelectedObject = Instantiate(placedPrefab, camMiddlePose.position, camMiddlePose.rotation).GetComponent<PlacementObject>();
                    previewObject = null;
                    canPlace = false;
                    UpdateLog("     LastSelected After Spawn: " + lastSelectedObject);
                }
                else
                {
                    if (lastSelectedObject.Selected && touch.phase == TouchPhase.Moved)
                    {
                        //lastSelectedObject.transform.position = lastSelectedObject.transform.position + (hitPose.position - initialHit);
                        lastSelectedObject.transform.position = hitPose.position;
                        lastSelectedObject.transform.rotation = hitPose.rotation;
                    }
                }
            }
        }
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

    private RaycastHit hit;
    void CrosshairCalculation()
    {
        Vector3 origin = arCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));
        Ray ray = arCamera.ScreenPointToRay(origin);

        if (arRaycastManager.Raycast(ray, hits))
        {
            camMiddlePose = hits[0].pose;
            previewObject.transform.position = camMiddlePose.position;
            previewObject.transform.rotation = camMiddlePose.rotation;
        }
        else if (Physics.Raycast(ray, out hit))
        {
            previewObject.transform.position = hit.point;
            previewObject.transform.up = hit.normal;
        }
    }

}