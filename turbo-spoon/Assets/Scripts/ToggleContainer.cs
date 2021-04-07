using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;


public class ToggleContainer : MonoBehaviour
{

    [SerializeField]
    GameObject scrollContainer;

    [SerializeField]
    public GameObject[] containers;

    [SerializeField]
    GameObject myCanvas;

    private bool buttonPressed = true;
    private float scrollContStartY;
    float screenBottom;

    public ARPlaneManager planeManager;
    private bool planeBool = true;

    [SerializeField]
    public Button button;

    [SerializeField]
    public Sprite[] buttonStates;




    public void ToggleActive()
    {
        foreach(GameObject obj in containers)
        {
            if (obj.activeSelf == true)
            {
                obj.SetActive(false);
            }
            else
            {
                obj.SetActive(true);
            }
        }
    }

    public void TogglePlane()
    {
        if (planeBool == true)
        {
            //planeManager.GetComponent<ARPlaneManager>().enabled = false;
            planeManager.SetTrackablesActive(false);
            //planeManager.enabled = false;//This might break it
            planeBool = false;
            //SetAllPlanesActive(false);
            
        }
        else
        {
            //planeManager.gameObject.SetActive(true);
            planeManager.GetComponent<ARPlaneManager>().enabled = true;
            planeBool = true;
            //SetAllPlanesActive(true);
        }
    }

    public void TweenDown(GameObject obj)
    {
        float tweenTime = 0.20f;
        if (buttonPressed == true)
        {
            buttonPressed = false;
            button.image.sprite = buttonStates[1];
            LeanTween.cancel(obj);
            LeanTween.moveLocalY(obj, (screenBottom - 300), tweenTime);
        }
        else
        {
            buttonPressed = true;
            button.image.sprite = buttonStates[0];
            LeanTween.cancel(obj);
            LeanTween.moveLocalY(obj, scrollContStartY, tweenTime);
        }
    }

   private void Update()
    {
        if(planeBool == false)
        {
            planeManager.SetTrackablesActive(false); // This is terribly inneficcient
        }
    }


    private void Start()
    {
        scrollContStartY = scrollContainer.transform.localPosition.y;
        screenBottom = myCanvas.GetComponent<RectTransform>().rect.top;
        //planeManager = FindObjectOfType<ARPlaneManager>();
    }

    private void SetAllPlanesActive(bool value)
    {
        foreach(var plane in planeManager.trackables)
        {
            plane.gameObject.SetActive(value);
        }
        planeBool = value;
    }

    
}
