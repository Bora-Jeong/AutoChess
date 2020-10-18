using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PanelBase<T> : Singleton<T> where T : MonoBehaviour
{
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }
}
