using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class MonoPanel : MonoBehaviour
{
    public virtual void Init() // 게임 시작하면 바로 호출되는 함수, 이벤트 등록용
    {

    }
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    public virtual void Hide()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnBackButton()
    {

    }
}

public abstract class PanelBase<T> : MonoPanel where T : MonoBehaviour
{
    private static T _instance;
    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                T[] objectList = Resources.FindObjectsOfTypeAll<T>();
                if (objectList.Length > 0)
                    (objectList[0] as PanelBase<T>).Awake();
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this as T;
    }
}
