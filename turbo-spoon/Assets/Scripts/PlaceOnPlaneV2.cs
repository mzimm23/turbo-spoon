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

    private bool onTouchHold = false;

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private PlacementObject lastSelectedObject;

    [SerializeField]
    private Button redButton, greenButton, blueButton;

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
                    UpdateLog("Hit " + hitObject.ToString());
                    initialHit = hitObject.point;
                    lastSelectedObject = hitObject.transform.GetComponent<PlacementObject>();
                    if (lastSelectedObject != null)
                    {
                        PlacementObject[] allOtherObjects = FindObjectsOfType<PlacementObject>();
                        foreach (PlacementObject placementObject in allOtherObjects)
                        {
                            placementObject.Selected = placementObject == lastSelectedObject;
                        }
                    }
                }
                else
                {
                    UpdateLog("Deselected "+lastSelectedObject);
                    lastSelectedObject.Selected = false;
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
                    lastSelectedObject = Instantiate(placedPrefab, hitPose.position, hitPose.rotation).GetComponent<PlacementObject>();
                }
                else
                {
                    if (lastSelectedObject.Selected)
                    {
                        //UpdateLog("- Trying to update position of "+lastSelectedObject.ToString());
                        //lastSelectedObject.transform.position = lastSelectedObject.transform.position + (hitPose.position - initialHit);
                        lastSelectedObject.transform.position = hitPose.position;
                        lastSelectedObject.transform.rotation = hitPose.rotation;
                    }
                }
            }

        }
    }

    [SerializeField] Text logText;
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
   
}