using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ21.Utils
{
    public class CursorController : MonoBehaviour
    {
        [Header("Cursor")]
        public Texture2D cursorGraphic;
        public Vector2 cursorHotspot = new Vector2(0, 0);

        private void Awake()
        {
            SetCursorGraphic();
        }

        private void OnApplicationFocus(bool focus)
        {
            if(focus)
                SetCursorGraphic();
        }

        private void SetCursorGraphic()
        {
            CursorMode mode = CursorMode.ForceSoftware;
            Vector2 hotSpot = new Vector2(cursorGraphic.width * cursorHotspot.x, cursorGraphic.height * cursorHotspot.y);
            Cursor.SetCursor(cursorGraphic, hotSpot, mode);
        }
    }
}