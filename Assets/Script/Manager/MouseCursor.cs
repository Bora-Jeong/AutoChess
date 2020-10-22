using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    [SerializeField]
    private Texture2D cursorTexture;
    [SerializeField]
    private bool hotSpotIsCenter = false;
    [SerializeField]
    private Vector2 adjustHotSpot = Vector2.zero;

    private Vector2 _hotSpot;
    private void Start()
    {
        StartCoroutine("MyCursor");
    }

    IEnumerator MyCursor()
    {
        yield return new WaitForEndOfFrame();

        if (hotSpotIsCenter)
        {
            _hotSpot.x = cursorTexture.width / 2;
            _hotSpot.y = cursorTexture.height / 2;
        }
        else
        {
            _hotSpot = adjustHotSpot;
        }

        Cursor.SetCursor(cursorTexture, _hotSpot, CursorMode.Auto);
    }
}
