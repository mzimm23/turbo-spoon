using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.Interaction.Toolkit.AR;

public class InputManager : ARBaseGestureInteractable
{
    [SerializeField] private Camera arCam;
    [SerializeField] private ARRaycastManager _raycastManager;
    [SerializeField] public GameObject obj;

    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private Touch touch;
    private Pose pose;


    protected override bool CanStartManipulationForGesture(TapGesture gesture)
    {
        if (gesture.targetObject == null)
        {
            UpdateLog("Can manipulate "+gesture.targetObject.name);
            return true;
        }
        else
        {
            UpdateLog("Nothing to manipulate");
            return false;
        }
    }

    protected override void OnEndManipulation(TapGesture gesture)
    {
        UpdateLog("OnEndManipulation manipulation has been entered");
        if (gesture.isCanceled)
            return;
        if(gesture.targetObject != null || IsPointerOverUI(gesture))
            return;
        if(GestureTransformationUtility.Raycast(gesture.startPosition, hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            UpdateLog("Trying to place: "+obj);
            pose = hits[0].pose;
            GameObject placedObject = Instantiate(obj, pose.position, pose.rotation);
            /*
            var anchorObject = new GameObject("PlacementAnchor");
            anchorObject.transform.position = pose.position;
            anchorObject.transform.rotation = pose.rotation;
            placedObject.transform.parent = anchorObject.transform;
            */

        }
    }

    private void FixedUpdate()
    {
        
    }

    bool IsPointerOverUI(TapGesture touch)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touch.startPosition.x, touch.startPosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }

    [SerializeField] Text logText;
    public void UpdateLog(string text)
    {
        logText.text += "\n" + text;
    }

    public void ChangePrefabSelection(GameObject gameObject)
    {
        UpdateLog("Trying to change object to " + gameObject.name);
        GameObject loadedGameObject = gameObject;
        if (loadedGameObject != null)
        {
            obj = loadedGameObject;
            Debug.Log($"Game object with name " + obj.name + " was loaded");
            UpdateLog($"Game object with name " + obj.name + " was loaded");
        }
        else
        {
            Debug.Log($"Unable to find a game object with name " + gameObject.name);
        }
    }
}
