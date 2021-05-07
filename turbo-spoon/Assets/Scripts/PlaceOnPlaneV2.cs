using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;


[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlaneV2 : MonoBehaviour
{
    [SerializeField]
    private GameObject placedPrefab, previewObject, loadedGameObject, spawnedObject, empty;

    [SerializeField]
    private Camera arCamera;

    private PlacementObject[] placedObjects;

    //public ARPlaneManager planeManager;

    private Vector2 touchPosition, initialTouch = default;

    private ARRaycastManager arRaycastManager;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private PlacementObject lastSelectedObject;
    
    private Pose camMiddlePose;

    private bool canPlace, canMove, hasMoved= false;

    Vector2 minDistance = new Vector2(5, 5);

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
        //planeManager = FindObjectOfType<ARPlaneManager>();

    }

    public void ChangePrefabSelection(GameObject gameObject)
    {
        UpdateLog("Changing...");
        if (loadedGameObject == gameObject)
        {
            if(lastSelectedObject.transform.parent != null)
            {
                UpdateLog("Object Already Selected");
                return;
            }
        }
        loadedGameObject = gameObject;
        if (loadedGameObject != null)
        {
            /*
            if (planeManager.trackables.count == 0) //Testing if no planes do nothing
            {
                return;
            }
            */
            UpdateLog("Object is "+loadedGameObject.name);
            if(previewObject.transform.childCount > 0)
            {
                Destroy(previewObject.transform.GetChild(0).gameObject);
            }
            placedPrefab = loadedGameObject;
            lastSelectedObject = Instantiate(placedPrefab, camMiddlePose.position, camMiddlePose.rotation).GetComponent<PlacementObject>();
            lastSelectedObject.transform.parent = previewObject.transform;
        }
        else
        {
            UpdateLog($"Unable to find a game object with name /" + gameObject.name);
        }
    }


    void Update()
    {
        CrosshairCalculation();


        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            touchPosition = touch.position;

            if (IsPointerOverUI(touch))
            {
                return;
            }

            if (touch.phase == TouchPhase.Began)
            {
                
                UpdateLog("Touch Began");
                if (lastSelectedObject.transform.parent != null && !lastSelectedObject.CompareTag("Empty")) //Maybe change the empty part
                {
                    UpdateLog("Item Placed");
                    lastSelectedObject.transform.parent = null;
                    lastSelectedObject.Selected = false;
                    return;
                }
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                
                if (Physics.Raycast(ray, out hitObject))
                {
                    UpdateLog("     Hit: " + hitObject.transform.gameObject.name.ToString());
                    initialHit = hitObject.point;
                   
                    if (hitObject.transform.GetComponent<PlacementObject>() == null)
                    {
                        UpdateLog("         DIDN'T HIT AN OBJECT");
                        ChangePrefabSelection(empty);
                        return; //Added this, might break it
                    }
                    lastSelectedObject = hitObject.transform.GetComponent<PlacementObject>();
                    if (lastSelectedObject != null)
                    {
                        
                        PlacementObject[] allOtherObjects = FindObjectsOfType<PlacementObject>();
                        foreach (PlacementObject placementObject in allOtherObjects)
                        {
                            placementObject.Selected = placementObject == lastSelectedObject;
                        }
                    }//Maybe add return here if unselect doesnt work
                }
                else
                {
                    UpdateLog("No Raycast Hit I Guess... "+hitObject);
                    
                }
            }   

            if (touch.phase == TouchPhase.Ended)
            {
                if (lastSelectedObject == hitObject.transform.GetComponent<PlacementObject>() && hitObject.transform.GetComponent<PlacementObject>() != null) // If you tap an object
                {
                    if(hasMoved == false)
                    {
                        if (lastSelectedObject.Locked == false) // De-select
                        {
                            lastSelectedObject.Locked = true;
                            lastSelectedObject.ToggleSelectedIndicator();
                            UpdateLog("         Deselected: " + lastSelectedObject);
                            UpdateLog("         LastSelectedObject: " + lastSelectedObject.name.ToString());
                        }
                        else // Select
                        {
                            lastSelectedObject.Locked = false;
                            lastSelectedObject.ToggleSelectedIndicator();
                            UpdateLog("         Re-Selected: " + lastSelectedObject);
                        }
                    }
                }
                UpdateLog("Touch Ended");
                hasMoved = false;
            }

            if (touch.phase == TouchPhase.Canceled)
            {
                UpdateLog("Touch Cancelled");
            }

            if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                if (lastSelectedObject.Selected == true && touch.phase == TouchPhase.Moved)
                {
                    if (lastSelectedObject.Locked == false)
                    {
                        hasMoved = true; // Move this to TouchPhase.Moved (Outside of the locked and selected check) to only accept selection by tap (not drag)
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
        if(lastSelectedObject.Selected && !lastSelectedObject.Locked) //If this doesnt work try changing the if to if(lastSelectedObject == defaultObject)
        {
            UpdateLog("     Deleted: " + lastSelectedObject.gameObject);
            Destroy(lastSelectedObject.gameObject);
            ChangePrefabSelection(empty);
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
        /*
        else if (Physics.Raycast(ray, out hit))
        {
            previewObject.transform.position = hit.point;
            previewObject.transform.up = hit.normal;
        }
        */
    }

}