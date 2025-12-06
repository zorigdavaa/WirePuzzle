using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using ZPackage;

public class Piece : Mb
{
    public PieceType Type;
    Coroutine rotCor;
    Transform pieceSlot;
    List<Node> Nodes;
    public void Rotate()
    {
        if (rotCor == null)
        {

            rotCor = StartCoroutine(LocalCor());
        }
        IEnumerator LocalCor()
        {
            float t = 0f;
            float time = 0f;
            float duration = 0.3f;
            Quaternion initial = transform.rotation;
            Quaternion target = Quaternion.Euler(0, transform.rotation.eulerAngles.y + 90, 0);

            while (time < duration)
            {
                time += Time.deltaTime;
                t = time / duration;
                transform.rotation = Quaternion.Lerp(initial, target, t);
                yield return null;
            }
            rotCor = null;
        }
    }
    public void SetPieceSlot(Transform slot)
    {
        pieceSlot = slot;
    }

    public List<Node> GetNodes()
    {
        if (Nodes == null)
        {
            Nodes = new List<Node>();
            foreach (Transform child in transform)
            {
                Nodes.Add(child.GetOrAddComponent<Node>());
            }
        }
        return Nodes;
    }

    internal void StartDrag(bool v)
    {
        if (v)
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            transform.localScale = Vector3.one * 0.5f;
        }
    }
}
public enum PieceType
{
    T, S, P, Plus, O
}