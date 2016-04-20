﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FindClosestIntersect {
    public float searchTolerance = 2.0f;

    public Vector3? search(IList<Vector3> curve, Vector3 point)
    {
        Vector3? ret = null;
        float closestDistance = searchTolerance + 1;
        for (int x = 0; x < curve.Count - 1 || ret != null; x++)
        {
            Vector3 closestPoint;
            if(x == curve.Count - 1)
            {
                closestPoint = curve[x];
            }
            else
            {
                Vector3 pointA = curve[x];
                Vector3 pointB = curve[x + 1];
                closestPoint = GetClosestPointOnLineSegment(pointA, pointB, point);
            }

            float distance = Vector3.Distance(closestPoint, point);
            if (distance <= searchTolerance && distance < closestDistance)
            {
                ret = closestPoint;
                closestDistance = distance;
            }
        }
        return ret;
    }


    public static Vector3 GetClosestPointOnLineSegment(Vector3 A, Vector3 B, Vector3 P)
    {
        Vector3 AP = P - A;       //Vector from A to P   
        Vector3 AB = B - A;       //Vector from A to B  

        float magnitudeAB = AB.magnitude;     //Magnitude of AB vector (it's length squared)     
        float ABAPproduct = Vector2.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
        float distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  
        if (distance < 0)     //Check if P projection is over vectorAB     
        {
            return A;

        }
        else if (distance > 1)
        {
            return B;
        }
        else
        {
            return A + AB * distance;
        }

    }
}
