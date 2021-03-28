using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogUpdate : MonoBehaviour
{
    [SerializeField] Text logText;
 
    public void UpdateLog(Text text)
    {
        logText.text += text;
    }
}
