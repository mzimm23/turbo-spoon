using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeItems : MonoBehaviour
{
    [SerializeField]
    public Button[] abstractArt, second, menu;
    public Button[] activeButtons;


    // Start is called before the first frame update
    void Start()
    {
       foreach(var btn in abstractArt)
        {
            btn.gameObject.SetActive(true);
        }
        activeButtons = abstractArt;
    }

    public void EnableSecond()
    {
        foreach (var btn in activeButtons)
        {
            btn.gameObject.SetActive(false);
        }
        foreach (var btn in second)
        {
            btn.gameObject.SetActive(true);
        }
        activeButtons = second;
    }

    public void EnableAbstract()
    {
        foreach (var btn in activeButtons)
        {
            btn.gameObject.SetActive(false);
        }
        foreach (var btn in abstractArt)
        {
            btn.gameObject.SetActive(true);
        }
        activeButtons = abstractArt;
    }

    public void EnableMenu()
    {
        foreach (var btn in activeButtons)
        {
            btn.gameObject.SetActive(false);
        }
        foreach (var btn in menu)
        {
            btn.gameObject.SetActive(true);
        }
        activeButtons = menu;
    }

}
