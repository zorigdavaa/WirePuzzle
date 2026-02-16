using UnityEngine;

public class Lightning : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float switchDistance = 0.25f;
    public float offsetSpeed = 0.5f;
    public Material mat;
    public int switchSpeed = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mat = lineRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        int index = (int)(Time.time * switchSpeed);
        float textureY = index * switchDistance;
        mat.mainTextureOffset = new Vector2(Time.time * offsetSpeed, textureY);
    }
    public void SetPostions(Vector3[] poses)
    {
        // lineRenderer.SetPosition(0, transform.position);
        // lineRenderer.SetPosition(1, transform.position + Vector3.up * switchDistance);
        lineRenderer.positionCount = poses.Length;
        lineRenderer.SetPositions(poses);
    }
}
