﻿/******************************************************************************
 *
 *                      GeaVR
 *                https://www.geavr.eu/
 *             https://github.com/GeaVR/GeaVR
 * 
 * GeaVR is an open source software that allows the user to experience a wide 
 * range of geological and geomorphological sites in immersive virtual reality,
 * including data collection.
 *
 * Main Developers:      
 * 
 *     Fabio Luca Bonali (fabio.bonali@unimib.it)
 *     Martin Kearl (martintkearl@gmail.com)
 *     Fabio Roberto Vitello (fabio.vitello@inaf.it)
 * 
 * Developed thanks to the contribution of following projects:
 *
 *     ACPR15T4_ 00098 “Agreement between the University of Milan Bicocca and the 
 *     Cometa Consortium for the experimentation of cutting-edge interactive 
 *     technologies for the improvement of science teaching and dissemination” of 
 *     Italian Ministry of Education, University and Research (ARGO3D)
 *     PI: Alessandro Tibaldi (alessandro.tibaldi@unimib.it)
 *     
 *     Erasmus+ Key Action 2 2017-1-UK01-KA203- 036719 “3DTeLC – Bringing the  
 *     3D-world into the classroom: a new approach to Teaching, Learning and 
 *     Communicating the science of geohazards in terrestrial and marine 
 *     environments”
 *     PI: Malcolm Whitworth (malcolm.Whitworth@port.ac.uk)
 * 
 ******************************************************************************
 * Copyright (c) 2016-2022
 * GPL-3.0 License
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// based on
// https://forum.unity.com/threads/new-ui-and-line-drawing.253772/

public class UIGraphRenderer : Graphic {
    private List<Vector2> Points = new List<Vector2>();
    
    private bool BoundsSet = false;
    private float MinX, MaxX, MinY, MaxY;
    
    public Text xAxisMax;
    public Text yAxisMax;
    public Text xAxisMin;
    public Text yAxisMin;

    public float LineWidth = 3.0f;
    public float backgroundLineWorldHeight = 10.0f;    
    public  Color axesColour = Color.black;
    
   protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        //vh.

        // set default bounds
        if (!BoundsSet)
        {
            MinX = 0.0f;
            MaxX = 1.0f;
            MinY = 0.0f;
            MaxY = 1.0f;            
        }

        Vector2 origin = GetRelativePosition(new Vector2(0.0f, 0.0f));
        // draw axes
        DrawLine(vh,
            GetPointInRectangleSpace(new Vector2(origin.x, 0.0f)),
            GetPointInRectangleSpace(new Vector2(origin.x, 1.0f)),
            new Vector2(-LineWidth, 0.0f),
            axesColour
            );

        DrawLine(vh,
            GetPointInRectangleSpace(new Vector2(0.0f, origin.y)) - new Vector2(LineWidth, 0.0f),
            GetPointInRectangleSpace(new Vector2(1.0f, origin.y)) + new Vector2(LineWidth, 0.0f),
            new Vector2(0.0f, -LineWidth),
            axesColour
            );


        // draw chart
        if (Points == null || Points.Count < 2)
        {
            Vector2 corner1 = GetPointInRectangleSpace(new Vector2(0.0f, 0.0f));
            Vector2 corner2 = GetPointInRectangleSpace(new Vector2(1.0f, 1.0f));
            DrawLine(vh, corner1, corner2, new Vector2(0.0f, LineWidth), color);
        }
        else
        {
            int stride = 1 + (Points.Count / 16000); // if too many points, skip rendering some

            for (int i = 0; i < Points.Count - 1; i += stride)
            {
                DrawLine(vh, i, i + 1, new Vector2(0.0f, LineWidth));
            }
        }        
    }


    protected void DrawLine(VertexHelper vh, int p1, int p2, Vector2 LineOffset)
    {
        Vector2 point1 = GetPointInRectangleSpace(GetRelativePosition(Points[p1]));
        Vector2 point2 = GetPointInRectangleSpace(GetRelativePosition(Points[p2]));

        DrawLine(vh, point1, point2, LineOffset, color);
    }

    protected void DrawLine(VertexHelper vh, Vector2 point1, Vector2 point2, Vector2 LineOffset, Color _colour)
    {
        int cvc = vh.currentVertCount;
        UIVertex vert = UIVertex.simpleVert;

        vert.position = new Vector2(point1.x, point1.y);
        vert.color = _colour;
        vh.AddVert(vert);

        vert.position = new Vector2(point1.x, point1.y) + LineOffset;
        vert.color = _colour;
        vh.AddVert(vert);

        vert.position = new Vector2(point2.x, point2.y);
        vert.color = _colour;
        vh.AddVert(vert);

        vert.position = new Vector2(point2.x, point2.y) + LineOffset;
        vert.color = _colour;
        vh.AddVert(vert);
                
        vh.AddTriangle(0 + cvc, 1 + cvc, 2 + cvc);
        vh.AddTriangle(1 + cvc, 2 + cvc, 3 + cvc);
    }

    protected Vector2 GetRelativePosition(Vector2 Pos)
    {        
        float x = (Pos.x - MinX) / (MaxX - MinX);
        float y = (Pos.y - MinY) / (MaxY - MinY);

        x = Mathf.Clamp(x, 0.0f, 1.0f);
        y = Mathf.Clamp(y, 0.0f, 1.0f);
        return new Vector2(x, y);
    }

    protected Vector2 GetPointInRectangleSpace(Vector2 Pos)
    {
        Vector2 newPoint = Pos;

        newPoint.x -= rectTransform.pivot.x;
        newPoint.y -= rectTransform.pivot.y;

        newPoint.x *= rectTransform.rect.width;
        newPoint.y *= rectTransform.rect.height;

        return newPoint;
    }

    public void AddPoint(Vector2 Point)
    {
        Points.Add(Point);

        if (!BoundsSet) // set bounds
        {
            MaxX = Point.x;
            MinX = Point.x;
            MaxY = Point.y;
            MinY = Point.y; 
            BoundsSet = true;
        }
        else
        {
            if (Point.x > MaxX) { MaxX = Point.x; }
            else if (Point.x < MinX) { MinX = Point.x; }

            if (Point.y > MaxY) { MaxY = Point.y; }
            else if (Point.y < MinY) { MinY = Point.y; }
        }
        if (xAxisMax) { xAxisMax.text = Points[Points.Count - 1].x.ToString("0.000"); }
        if (yAxisMax) { yAxisMax.text = MaxY.ToString("0.000"); }
        if (xAxisMin) { xAxisMin.text = Points[0].x.ToString("0.000"); }
        if (yAxisMin) { yAxisMin.text = MinY.ToString("0.000"); }
        SetAllDirty();
    }

    public void ClearPoints()
    {
        Points.Clear();
        SetAllDirty();
    }


}
