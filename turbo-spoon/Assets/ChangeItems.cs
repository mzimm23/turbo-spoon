using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeItems : MonoBehaviour
{
    [SerializeField]
    public RectTransform activeButton;
    [SerializeField]
    RectTransform myCanvas;
    [SerializeField]
    public float tweenTime = 0.5f;
    float screenBottom;


    // Start is called before the first frame update
    void Start()
    {
        screenBottom = myCanvas.rect.yMin;
    }

    public void ChangeMenu(RectTransform newObj)
    {
        if (activeButton == newObj)
            return;
        TweenOut(activeButton);
        activeButton = newObj;
        TweenIn(activeButton);
    }

    public void TweenOut(RectTransform obj)
    {
        LeanTween.cancel(obj);
        LeanTween.moveY(obj, (-544), tweenTime);
    }
    public void TweenIn(RectTransform obj)
    {
        myCanvas.GetComponent<ScrollRect>().content = obj;
        LeanTween.cancel(obj);
        LeanTween.moveY(obj, (-544), 0);
        LeanTween.moveY(obj, (0), tweenTime);
        
    }

    

}
