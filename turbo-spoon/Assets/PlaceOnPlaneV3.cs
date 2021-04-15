using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class PlaceOnPlaneV3 : ARBaseGestureInteractable
{
    [SerializeField] private Camera arCam;
    [SerializeField] private ARRaycastManager rRaycastManager;
    [SerializeField] public GameObject crosshair;
    public GameObject placedPrefab;

    private List<ARRaycastHit> _hits = new List<ARRaycastHit>();

    private Touch touch;
    private Pose pose;

    protected override void OnStartManipulation(TapGesture gesture)
    {
        if (gesture.isCanceled)
        {
            return;
        }
        if (gesture.targetObject != null || IsPointerOverUI(gesture))
        {
            return;
        }
        GameObject spawnedObject = Instantiate(placedPrefab, pose.position, pose.rotation);
    }
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CrosshairCalculation();
    }

    bool IsPointerOverUI(TapGesture touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touch.startPosition.x, touch.startPosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    void CrosshairCalculation()
    {
        Vector3 origin = Camera.main.ViewportToScreenPoint(new Vector3(0.5f, 0.5f, 0));

        if (GestureTransformationUtility.Raycast(origin, _hits, TrackableType.PlaneWithinPolygon))
        {
            pose =_hits[0].pose;
            crosshair.transform.position = pose.position;
            crosshair.transform.rotation = pose.rotation;
        }
    }

    public void ChangePrefabSelection(GameObject gameObject)
    {
        GameObject loadedGameObject = gameObject;
        if (loadedGameObject != null)
        {
            placedPrefab = loadedGameObject;
            crosshair = loadedGameObject;
            Debug.Log($"Game object with name /" + placedPrefab.name + " was loaded");
        }
        else
        {
            Debug.Log($"Unable to find a game object with name /" + gameObject.name);
        }
    }

}
