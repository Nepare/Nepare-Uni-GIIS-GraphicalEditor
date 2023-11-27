using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelController : MonoBehaviour
{
    public int x, y;
    private SpriteRenderer spite_renderer;
    private bool isFilled;

    void Awake()
    {
        spite_renderer = GetComponent<SpriteRenderer>();
        isFilled = false;
    }

    public void SetCoords(int x_in, int y_in)
    {
        x = x_in;
        y = y_in;
    } 
    
    public List<int> GetCoords()
    {
        return new List<int> { x, y };  
    }

    public void ChangeColor(float alpha, bool isRed)
    {
        Color new_color = new Color(isRed ? 1f : 1f - alpha, isRed ? 0f : 1f - alpha, isRed ? 0f : 1f - alpha);
        spite_renderer.color = new_color;
        isFilled = true;
    }

    public void ChangeColor(Color color)
    {
        spite_renderer.color = color;
        isFilled = true;
    }

    public void ChangeToWhite()
    {
        spite_renderer.color = Color.white;
        isFilled = false;
    }

    public bool IsFilled()
    {
        return isFilled;
    }
}
