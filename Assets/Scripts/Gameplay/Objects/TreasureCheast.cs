using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureCheast : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    
    public void Show()
    {
        spriteRenderer.enabled = true;
    }

    public void Hide()
    {
        spriteRenderer.enabled = false;
    }
}
