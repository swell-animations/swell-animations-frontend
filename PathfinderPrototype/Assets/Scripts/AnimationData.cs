﻿using UnityEngine;
using System.Collections;
using swellanimations;

public class AnimationData
{

    public static string NodeToString(Node node)
    {
        return "Name: " + node.name +
        " positionX: " + node.position.x +
        " positionY: " + node.position.y +
        " positionX: " + node.position.z +
        " rotationX: " + node.eularAngles.x +
        " rotationY: " + node.eularAngles.y +
        " rotationZ: " + node.eularAngles.z;
    }

    //The following are used for debug purposes
    public static void PrintAllNodes(Node node, string spacing)
    {
        Debug.Log(spacing + NodeToString(node)); ;
        foreach (Node childNode in node.children)
        {
            PrintAllNodes(childNode, spacing + "-");
        }

    }

    public static Node GenerateNode(Transform model)
    {
        Node node = CreateNodeFromGameObject(model);
        GenerateChildren(model, node);
        return node;
    }

    public static Node CreateNodeFromGameObject(Transform transform)
    {
        Vector position = new Vector()
        {
            x = transform.position.x,
            y = transform.position.y,
            z = transform.position.z
        };
        Vector eulerAngles = new Vector()
        {
            x = transform.eulerAngles.x,
            y = transform.eulerAngles.y,
            z = transform.eulerAngles.z
        };
        return new Node()
        {
            name = transform.gameObject.name,
            position = position,
            eularAngles = eulerAngles
        };
    }

    public static void GenerateChildren(Transform children, Node parent)
    {
        foreach (Transform transform in children)
        {
            Node child = CreateNodeFromGameObject(transform);
            child.parent = parent;
            parent.children.Add(child);
            GenerateChildren(transform, child);
        }
    }

}