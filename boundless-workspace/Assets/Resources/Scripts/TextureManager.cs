using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TextureManager {

    private Texture2D texture;

    private Color[] colors;
    private Color[] clearColors; // array that clears the entire texture dimension
    private int colorIndex;

    private int brushStroke;
    private Color[] colorRect;
    
    private bool circularBrush;
    private bool isErasing;

    const int MIN_PX = 1024;

    private Vector2 previousPoint;

    // aspectRatio = width / height
    public TextureManager(float aspectRatio)
    {
        int width, height;

        if (aspectRatio < 1)  // width < height
        {  
            width = MIN_PX;
            height = (int)(width / aspectRatio);
        }
        else
        {
            height = MIN_PX;
            width = (int)(height * aspectRatio);
        }

        texture = new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false);
        texture.filterMode = FilterMode.Point;

        this.colors = new Color[] { Color.black, Color.clear };
        this.colorIndex = 0;
        brushStroke = 1;

        colorRect = new Color[brushStroke * brushStroke];
        for (int i = 0; i < colorRect.Length; i++) colorRect[i] = getCurrentColor();
        circularBrush = true;

        previousPoint = new Vector2(-1,-1); // null state of the vector since Vector2 is a non-nullable object

        clearColors = new Color[width * height];
        for(int i = 0;i < clearColors.Length;i++)
            clearColors[i] = Color.clear;

        Clear();
    }

    public void Clear()
    {
        texture.SetPixels(0,0,texture.width,texture.height,clearColors);
        texture.Apply();
    }

    private Color getCurrentColor()
    {
        return this.colors[this.colorIndex];
    }

    public Texture2D GetTexture()
    {
        return texture;
    }

    public void BeginPaint(Vector2 point)
    {
        Debug.Log("BeginPaint Starting");
       // Debug.Log("previousPoint: " + previousPoint);
        float currX = point.x * texture.width; // scale x,y back to normal texture coordinates
        float currY = point.y * texture.height;
       Debug.Log("currentPoint: " + currX + ", " + currY);
        PaintPoint(new Vector2(currX, currY), getCurrentColor());
        previousPoint.x = currX;
        previousPoint.y = currY;

        texture.Apply();
    }

    public void ContinuePaint(Vector2 point)
    {
        //Debug.Log("ContinuePaint Starting");
       // Debug.Log("previousPoint: " + previousPoint);
        int prevX = (int)previousPoint.x;
        int prevY = (int)previousPoint.y;

        int currX = (int)(point.x * texture.width); // scale x,y back to normal texture coordinates
        int currY = (int)(point.y * texture.height);

        //Debug.Log("currentPoint: " + currX + ", " + currY);
        LinearInterpolate(prevX, prevY, currX, currY);

        previousPoint.x = currX;
        previousPoint.y = currY;

        texture.Apply();
    }

    public void ResumePaint(Vector2 point)
    {
        Debug.Log("ResumePaint Starting");
        
        this.BeginPaint(point);
    }

    public void PausePaint()
    {
        Debug.Log("PausePaint Starting");
        
        this.EndPaint();
    }

    public void EndPaint()
    {
        Debug.Log("EndPaint Starting");
        
        this.DrawingStopped();
    }

    public void PaintPoint(Vector2 point) // point will contain x,y coordinates betwen 0.0 ~ 1.0 
    {
        int prevX = (int)previousPoint.x;
        int prevY = (int)previousPoint.y;

        int currX = (int)(point.x * texture.width); // scale x,y back to normal texture coordinates
        int currY = (int)(point.y * texture.height);

        if (prevX != -1)
            LinearInterpolate(prevX, prevY, currX, currY);
        else
            PaintPoint(new Vector2(currX, currY), getCurrentColor());

        previousPoint.x = currX;
        previousPoint.y = currY;
       
        texture.Apply();
    }

    private bool InBounds(int x, int y)
    {
        return x >= 0 && x < texture.width && y >= 0 && y < texture.height;
    }

    
    public void PaintPoint(Vector2 point,Color color)
    {
        int x = (int)point.x;
        int y = (int)point.y;
        //if (InBounds(x, y)) return;
        x -=  brushStroke / 2; // center the x,y coordinate
        y -=  brushStroke / 2;
            if (brushStroke == 1)
                texture.SetPixel(x, y, getCurrentColor());
            else
            {
                if (circularBrush)
                {
                    for (int i = 0; i < colorRect.Length; i++)
                    {
                        int x1 = i % brushStroke;
                        int y1 = i / brushStroke;
                        int xCenter = brushStroke / 2;
                        int yCenter = brushStroke / 2;
                        float distance = (float)Math.Sqrt(Math.Pow(x1 - xCenter, 2) + Math.Pow(y1 - yCenter, 2));
                    if (distance > (float)brushStroke / 2)
                        colorRect[i] = texture.GetPixel(x + x1, y + y1);
                    else
                        colorRect[i] = getCurrentColor();
                    }
                }
                texture.SetPixels(x, y, brushStroke, brushStroke, colorRect);
            }
        
    }


    private void LinearInterpolate(int x1, int y1, int x2, int y2)
    {

        int dx = Math.Sign(x2 - x1);
        if (dx == 0) dx = 1; // edge case when drawing vertical line
        double y = y1;
        double yOffset = (double)(Math.Abs(y2 - y1) + 1) / (Math.Abs(x2 - x1) + 1);
        // yOffset tells you for every x pixel how many y pixels you need to stack up/down

        int sign = Math.Sign(y2 - y1); // whether y is increasing or decresing

        for (int x = x1; x != x2 + dx; x += dx)
        { // start from x1 and iterate until x2
            for (int i = 0; i < yOffset; i++)
            { // fill in y pixels 
                //tex.SetPixel(x, (int)y + (i * sign), Color.black);
                PaintPoint(new Vector2(x, (int)y + (i * sign)), getCurrentColor());
                //(int)y is the starting y coordinate
                //(i*sign) is the amount of incrementation
            }
            y += sign * yOffset;// increment y coordinate by the offset

        }
    }

    public void UseEraser()
    {
        this.colorIndex = 1;
    }

    public void UseBrush()
    {
        this.colorIndex = 0;
    }

    public void SetBrushStroke(int brushStroke)
    {
        if (brushStroke < 1) return;
        this.brushStroke = brushStroke;
        this.colorRect = new Color[brushStroke * brushStroke];
        //for (int i = 0; i < colors.Length; i++) this.colors[i] = this.brushColor;
    }

    public void DrawingStopped()
    {
        previousPoint.Set(-1, -1);
    }

    // copy color
    public void SetBrushColor(Color color)
    {
        colors[0] = color;
    }

    // r,g,b values from 0 to 1 inclusive
    // alpha of 0 is completely transparent
    // alpha of 1 is completely opaque
    public void SetBrushColor(float r,float g,float b,float alpha = 1)
    {
        this.SetBrushColor(new Color(r, g, b, alpha));
    }
}
