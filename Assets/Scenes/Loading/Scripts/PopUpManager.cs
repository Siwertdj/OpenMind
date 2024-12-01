using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    public Canvas parent;

    public void ClosePopUp()
    {
        parent.gameObject.SetActive(false);
    }
}
