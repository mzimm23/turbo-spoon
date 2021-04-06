using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeItems : MonoBehaviour
{
    [SerializeField]
    public Button[] abstractArt, second;
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

    public void MYMETHOD(Button[] newButtons)
    {
        if (newButtons.Length > 0)
        {
            foreach (var btn in activeButtons)
            {
                btn.gameObject.SetActive(false);
            }
            foreach (var btn in newButtons)
            {
                btn.gameObject.SetActive(true);
            }
            activeButtons = newButtons;
        }
    }

}
