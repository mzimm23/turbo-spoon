using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;


[RequireComponent(typeof(ARRaycastManager))]
public class PlaceOnPlaneV2 : MonoBehaviour
{
    [SerializeField]
    private GameObject placedPrefab, previewObject, loadedGameObject, spawnedObject;

    [SerializeField]
    private Camera arCamera;

    private PlacementObject[] placedObjects;

    private Vector2 touchPosition = default;

    private ARRaycastManager arRaycastManager;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private PlacementObject lastSelectedObject;
    
    private Pose camMiddlePose;

    private bool canPlace, canMove = false;

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

    }

    public void ChangePrefabSelection(GameObject gameObject)
    {
        if (loadedGameObject == gameObject)
            return;
        loadedGameObject = gameObject;
        if (loadedGameObject != null)
        {
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
            Debug.Log($"Unable to find a game object with name /" + gameObject.name);
        }
    }


    void Update()
    {
        CrosshairCalculation();
        //text.text = "The current selected object is: " + lastSelectedObject;
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (IsPointerOverUI(touch))
            {
                return;
            }
            touchPosition = touch.position;
            if (touch.phase == TouchPhase.Began)
            {
                UpdateLog("Touch Began");

                if (lastSelectedObject.Locked == true)
                {
                    UpdateLog("Item Placed");
                    lastSelectedObject.transform.parent = null;
                    lastSelectedObject.Locked = false;
                    return;
                }
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                
                if (Physics.Raycast(ray, out hitObject))
                {
                    UpdateLog("     Hit: " + hitObject.transform.gameObject.name.ToString());
                    initialHit = hitObject.point;
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
                    if (hitObject.transform.GetComponent<PlacementObject>() == null)
                    {
                        UpdateLog("         DIDN'T HIT AN OBJECT");
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
                text.text = "Moved Position: " + touch.deltaPosition.normalized.ToString();
                if (lastSelectedObject == hitObject.transform.GetComponent<PlacementObject>() && lastSelectedObject != null)
                {
                    
                    if (lastSelectedObject.Selected == false)
                    {
                        if (touch.deltaPosition.x < 1 && touch.deltaPosition.x < 1)
                        {
                            UpdateLog("Maybe Didnt Move A Whole Lot?");
                        }
                    }
                }
                UpdateLog("Touch Ended");
            }

            if (arRaycastManager.Raycast(touchPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
            {
                Pose hitPose = hits[0].pose;
                if (lastSelectedObject.Selected == true && touch.phase == TouchPhase.Moved)
                {
                    if (lastSelectedObject.Locked == false)
                    {
                        lastSelectedObject.transform.position = hitPose.position;
                        lastSelectedObject.transform.rotation = hitPose.rotation;
                        if (touch.deltaPosition.x > .5 || touch.deltaPosition.x > .5)
                        {
                            UpdateLog("NOT A TAP");
                        }
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
            UpdateLog("     Deleted: " + lastSelectedObject.gameObject);
            Destroy(lastSelectedObject.gameObject);
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
        /*
        else if (Physics.Raycast(ray, out hit))
        {
            previewObject.transform.position = hit.point;
            previewObject.transform.up = hit.normal;
        }
        */
    }

}