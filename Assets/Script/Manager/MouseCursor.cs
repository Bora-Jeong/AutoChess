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
        StartCoroutine(MyCursor(cursorTexture));
    }

    public void SetCursor(Sprite sprite = null)
    {
        if(sprite == null)
            StartCoroutine(MyCursor(cursorTexture));
        else
            StartCoroutine(MyCursor(sprite.texture));
    }

    IEnumerator MyCursor(Texture2D texture)
    {
        yield return new WaitForEndOfFrame();

        if (hotSpotIsCenter)
        {
            _hotSpot.x = texture.width / 2;
            _hotSpot.y = texture.height / 2;
        }
        else
        {
            _hotSpot = adjustHotSpot;
        }

        Cursor.SetCursor(texture, _hotSpot, CursorMode.Auto);
    }
}
