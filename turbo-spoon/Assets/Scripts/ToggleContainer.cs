using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ToggleContainer : MonoBehaviour
{

    [SerializeField]
    GameObject scrollContainer;

    [SerializeField]
    public GameObject[] containers;

    [SerializeField]
    GameObject myCanvas;

    private bool isScrollContOnScreen = true;
    private float scrollContStartY;
    float screenBottom;

    public GameObject plane;



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

    public void togglePlane()
    {
        if (plane.activeSelf == true)
        {
            plane.SetActive(false);
        }
        else
        {
            plane.SetActive(true);
        }
    }

    public void TweenDown(GameObject obj)
    {
        float tweenTime = 0.25f;
        if (isScrollContOnScreen == true)
        {
            isScrollContOnScreen = false;
            LeanTween.cancel(obj);
            LeanTween.moveLocalY(obj, (screenBottom - 200), tweenTime);
        }
        else
        {
            isScrollContOnScreen = true;
            LeanTween.cancel(obj);
            LeanTween.moveLocalY(obj, scrollContStartY, tweenTime);
        }
    }

    private void Start()
    {
        scrollContStartY = scrollContainer.transform.localPosition.y;
        screenBottom = myCanvas.GetComponent<RectTransform>().rect.top;
        plane = GameObject.FindGameObjectWithTag("Plane");
    }
}
