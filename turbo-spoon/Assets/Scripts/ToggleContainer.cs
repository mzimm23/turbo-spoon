using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject plane;

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

   


    private void Start()
    {
        scrollContStartY = scrollContainer.transform.localPosition.y;
        screenBottom = myCanvas.GetComponent<RectTransform>().rect.top;
        plane = GameObject.FindGameObjectWithTag("Plane");
    }
}
