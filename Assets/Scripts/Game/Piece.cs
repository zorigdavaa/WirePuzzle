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
    public int Order = 0;
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
    public List<Vector2> GetNodesAsOffset()
    {
        var nodes = GetNodes();
        var result = new List<Vector2>();
        // Debug.Log($"Getting offsets for piece {nodes.Count} nodes");
        if (nodes == null || nodes.Count == 0)
            return result;

        Transform first = nodes[0].transform;
        //Skip first node
        for (int i = 1; i < nodes.Count; i++)
        {
            Vector3 localPos = first.InverseTransformPoint(nodes[i].transform.position).SwitchYZ();
            result.Add(new Vector2(
                Mathf.RoundToInt(localPos.x),
                Mathf.RoundToInt(localPos.y)
            ));
        }

        // foreach (var item in nodes)
        // {

        // }

        return result;
    }


    internal void StartDrag(bool v)
    {
        if (v)
        {
            transform.localScale = Vector3.one;
        }
        else
        {
            // transform.localScale = Vector3.one * 0.5f;
            transform.localScale = Vector3.one * Z.CurrentLevel.Data.PieceScale;
        }
    }

    public void SetColor(Material material)
    {
        foreach (var item in GetNodes())
        {

            item.transform.GetChild(0).GetComponent<Renderer>().material = material;
        }
    }
    public GameObject SilHoutte = null;
    public GameObject GetSilhoutte()
    {
        if (SilHoutte == null)
        {
            GameObject newSilhoute = new GameObject("Silhoutte");
            foreach (var item in Nodes)
            {
                Node copyNode = Instantiate(item);
                copyNode.transform.SetParent(newSilhoute.transform);
                copyNode.transform.localPosition = item.transform.localPosition;
                SetTransparent(copyNode.transform.GetChild(0).GetComponent<Renderer>().material, 0.1f);
                Destroy(copyNode);
            }
            SilHoutte = newSilhoute;
            // newSilhoute.transform.SetParent(transform);
        }
        return SilHoutte;
    }
    public void SetTransparent(Material mat, float alpha)
    {
        mat.SetFloat("_Surface", 1); // 0 = Opaque, 1 = Transparent
        mat.SetFloat("_Blend", 0);   // Alpha blend
        // Disable depth writing
        mat.SetInt("_ZWrite", 0);

        Color c = mat.GetColor("_BaseColor");
        c.a = alpha;
        mat.SetColor("_BaseColor", c);
        // Enable required keywords
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
    }
}
public enum PieceType
{
    T, S, P, Plus, O
}