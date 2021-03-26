using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEditor;

public class PlaceOnPlane : MonoBehaviour
{
    [SerializeField]
    ARRaycastManager m_RaycastManager;
    private List<GameObject> placedPrefabList = new List<GameObject>();
    [SerializeField]
    private Camera arCamera;
    [SerializeField]
    private int maxPrefabSpawnCount = 0;
    private int placedPrefabCount;

    static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

    [SerializeField]
    GameObject m_ObjectToPlace;

    [SerializeField]
    GameObject menuObj;

    private Vector2 touchPosition = default;

    private ARRaycastManager arRaycastManager;

    private bool onTouchHold = false;

    private PlacementObject lastSelectedObject;


    private GameObject spawnedObject;

    

    // Update is called once per frame
    void Update()
    {


        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            touchPosition = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = arCamera.ScreenPointToRay(touch.position);
                RaycastHit hitObject;
                if (Physics.Raycast(ray, out hitObject))
                {
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
            }

            if (touch.phase == TouchPhase.Ended)
            {
                lastSelectedObject.Selected = false;
            }
        }

        if (arRaycastManager.Raycast(touchPosition, s_Hits, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = s_Hits[0].pose;

            if (lastSelectedObject == null)
            {
                lastSelectedObject = Instantiate(m_ObjectToPlace, hitPose.position, hitPose.rotation).GetComponent<PlacementObject>();
            }
            else
            {
                if (lastSelectedObject.Selected)
                {
                    lastSelectedObject.transform.position = hitPose.position;
                    lastSelectedObject.transform.rotation = hitPose.rotation;
                }
            }
        }
    }
    
    public void SetprefabType(GameObject prefabType)
    {
        m_ObjectToPlace = prefabType;
        Debug.Log("Object changed to "+prefabType);
    }

    private void SpawnPrefab(Pose hitPose)
    {
        spawnedObject = Instantiate(m_ObjectToPlace, hitPose.position, hitPose.rotation);
        placedPrefabList.Add(spawnedObject);
        placedPrefabCount++;
    }

    public void EnnableMenu()
    {
        if(menuObj.activeSelf == false)
        {
            menuObj.SetActive(true);
        }
        else
        {
            menuObj.SetActive(false);
        }
    }
}
