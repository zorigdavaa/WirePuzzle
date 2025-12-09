using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZPackage;
using ZPackage.Helper;
using Random = UnityEngine.Random;
using UnityEngine.Pool;
using ZPackage.Utility;
using System.Linq;
using Dreamteck.Splines;

public class Player : Mb
{
    GridNode lastSelectedSlot;
    public List<GridNode> selectedSlots;
    SplineComputer computer;
    [SerializeField] GameObject ConnectPF;
    Camera cam;
    LayerMask rayMask;
    GridController gridController;
    PieceController pieceController;
    RaycastHit hit;
    Ray ray;

    public Piece selectedPiece;
    private float pressStartTime;
    [SerializeField] bool isDragging = false;
    [SerializeField] bool pressed = false;
    public float holdTime = 0.15f;    // how long before it becomes a "hold"
    float moveHeight = 0.5f;     // height while dragging
    void Start()
    {
        gridController = FindAnyObjectByType<GridController>();
        pieceController = FindAnyObjectByType<PieceController>();
        cam = FindAnyObjectByType<Camera>();
        rayMask = LayerMask.GetMask("Piece");
        // computer = Instantiate(ConnectPF, transform.position, Quaternion.identity).GetComponent<SplineComputer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Old();
        if (IsDown)
        {
            pressed = true;
            pressStartTime = Time.time;

            // Raycast to find object
            ray = cam.ScreenPointToRay(MP);
            if (Physics.Raycast(ray, out hit, 30, rayMask))
            {
                if (hit.collider.attachedRigidbody != null && hit.collider.attachedRigidbody.GetComponent<Piece>())
                {

                    selectedPiece = hit.collider.attachedRigidbody.GetComponent<Piece>();
                }
            }
        }
        // Holding logic
        else if (pressed && selectedPiece != null)
        {
            float pressedDuration = Time.time - pressStartTime;

            // Once hold threshold passed → begin dragging
            if (!isDragging && pressedDuration > holdTime)
            {
                isDragging = true;
                pressed = false;
                selectedPiece.StartDrag(true);
            }
        }
        else if (isDragging && selectedPiece != null)
        {
            ray = cam.ScreenPointToRay(MP);
            Plane ground = new Plane(Vector3.up, Vector3.zero);

            if (ground.Raycast(ray, out float dist))
            {
                Vector3 point = ray.GetPoint(dist);
                point.y = moveHeight;  // floating while dragging
                selectedPiece.transform.position = point;
            }
        }
        if (IsUp)
        {
            // Debug.Log("up");
            if (selectedPiece != null)
            {
                // float pressedDuration = Time.time - pressStartTime;

                // if (!isDragging && pressedDuration < holdTime)
                if (!isDragging)
                {
                    // TAP → ROTATE
                    selectedPiece.GetComponent<Piece>().Rotate();
                }
                else if (isDragging)
                {
                    if (gridController.IsPlaceAble(selectedPiece, out List<Slot> freeSlots))
                    {
                        gridController.Place(selectedPiece, freeSlots);
                        pieceController.NotifyPlaced(selectedPiece);
                        Destroy(selectedPiece.gameObject);
                        Z.LS.CurrentLevel.CheckConnected();
                    }
                    else
                    {
                        PlaceObject();
                    }
                }
            }
            // ClearLine();
            pressed = false;
            isDragging = false;
            selectedPiece = null;
        }
    }

    private void ClearLine()
    {
        lastSelectedSlot = null;
        selectedSlots.Clear();
        computer.SetPoints(new SplinePoint[0]);
        computer.Rebuild();
        // ray = cam.ScreenPointToRay(MP);
        // if (Physics.Raycast(ray, out hit, 30, rayMask))
        // {
        //     if (hit.collider.attachedRigidbody.GetComponent<Piece>())
        //     {
        //         hit.collider.attachedRigidbody.GetComponent<Piece>().Rotate();
        //     }
        // }
    }

    private void PlaceObject()
    {
        if (pieceController.HasSlot(selectedPiece))
        {
            pieceController.GotoSlot(selectedPiece);
        }
        // // Snap to ground
        // Vector3 pos = selectedObject.transform.position;
        // pos.y = 0;
        // selectedObject.transform.position = pos;
    }

    private void drawLine()
    {
        Vector3 mousePos = MP;
        mousePos.z = cam.transform.position.y;
        Vector3 worldMouse = cam.ScreenToWorldPoint(mousePos).SwitchYZ();
        // print(worldMouse);
        GridNode foundSlot = gridController.Grid.GetGridObject(worldMouse);
        if (foundSlot != lastSelectedSlot && foundSlot != null && !selectedSlots.Contains(foundSlot))
        {
            if (lastSelectedSlot != null)
            {

                lastSelectedSlot.GetComponent<Renderer>().material.color = Color.gray;
            }
            lastSelectedSlot = foundSlot;
            lastSelectedSlot.GetComponent<Renderer>().material.color = Color.red;
            // Vector3 lastPos = selectedSlots.Count > 0 ? selectedSlots.Last().transform.position : foundSlot.transform.position;
            // Vector3 forward = (foundSlot.transform.position - lastPos).normalized;
            List<SplinePoint> knots = new List<SplinePoint>();
            SplinePoint knot = new SplinePoint(foundSlot.transform.position);
            knots.Add(knot);
            selectedSlots.Add(lastSelectedSlot);
            foreach (var item in knots)
            {
                computer.SetPoint(computer.pointCount, item);
            }
            // computer.Spline = spline;
            computer.GetComponent<SplineMesh>().GetChannel(0).count = computer.pointCount * 4;
            computer.Rebuild();
        }
    }

    private bool isTurn(Slot foundSlot, out Vector3 initialDir, out Vector3 currentDir)
    {
        if (selectedSlots.Count >= 2)
        {
            GridNode lastOne = selectedSlots[^1];
            GridNode lastTwo = selectedSlots[^2];
            initialDir = (lastOne.transform.position - lastTwo.transform.position).normalized;
            currentDir = (foundSlot.transform.position - lastOne.transform.position).normalized;
            if (initialDir != currentDir)
            {
                return true;
            }
        }
        initialDir = Vector3.zero;
        currentDir = Vector3.zero;
        return false;
    }
}
