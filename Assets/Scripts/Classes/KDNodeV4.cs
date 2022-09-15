using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KDNodeV4 
{
    #region Vars

    public KDNodeV4 leftChild { get; set; }
    public KDNodeV4 rightChild { get; set; }
    public KDNodeV4 parent { get; set; }
    public Vector2 medianPoint { get; set; }
    public task task { get; set; }
    public int axis { get; set; }

    #endregion

    #region Constructor

    public KDNodeV4(int axis, Vector2 medianPoint, KDNodeV4 leftChild, KDNodeV4 rightChild, task task)
    {
        this.axis = axis;
        this.medianPoint = medianPoint;
        this.leftChild = leftChild;
        this.rightChild = rightChild;
        this.task = task;
    }

    #endregion
}