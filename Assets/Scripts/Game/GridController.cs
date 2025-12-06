using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using ZPackage;
using Dreamteck.Splines;

public class GridController : Mb
{
    public List<Slot> Slots;
    Camera cam;
    RaycastHit hit;
    Ray ray;
    [SerializeField] Transform draggingObject;
    ISlotObj draggingObjSlot;
    [SerializeField] GameObject shooterPF;
    [SerializeField] GameObject ConnectPF;
    Vector3 dragObjOriginPos, lastDragPos;
    LayerMask roadMask;
    Grid<Slot> Grid;
    public int X;
    public int Y;
    // Start is called before the first frame update
    void Start()
    {
        //
        Grid = new(X, Y, 1, transform.position + new Vector3(-X * 0.5f, -Y * 0.5f, 0), CreateSlot, transform);
        cam = FindAnyObjectByType<Camera>();
        roadMask = LayerMask.GetMask("Road");
        computer = Instantiate(ConnectPF, transform.position, Quaternion.identity).GetComponent<SplineComputer>();
    }
    Slot CreateSlot(Grid<Slot> grid, int x, int y)
    {
        GameObject insObj = Instantiate(shooterPF, grid.GetWorldPLacement(x, y).SwitchYZ(), Quaternion.identity, transform);
        // insObj.transform.localPosition = new Vector3(x + 0.5f, 0, y + 0.5f);
        return insObj.GetComponent<Slot>();
    }
    public bool dragging = false;
    Slot lastSelectedSlot;
    public List<Slot> selectedSlots;
    SplineComputer computer;

    // Update is called once per frame
    void Update()
    {
        // Old();
        if (IsDown)
        {


        }
        else if (IsClick)
        {
            // if (lastSelectedSlot != null)
            // {
            //     lastSelectedSlot.GetComponent<Renderer>().material.color = Color.gray;
            // }
            // Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
            Vector3 mousePos = MP;
            mousePos.z = cam.transform.position.y;
            Vector3 worldMouse = cam.ScreenToWorldPoint(mousePos).SwitchYZ();
            print(worldMouse);
            Slot foundSlot = Grid.GetGridObject(worldMouse);
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
                // if (isTurn(foundSlot, out Vector3 initialDir, out Vector3 currentDir))
                // {

                //     // SplinePoint last = computer.GetPoint(computer)[^1];
                //     // last.Position -= (float3)initialDir * 0.2f;

                //     // SplinePoint knot = new
                //     // SplinePoint(foundSlot.transform.position - currentDir * 0.8f);
                //     // knots.Add(knot);

                //     SplinePoint knot2 = new
                //     SplinePoint(foundSlot.transform.position);
                //     knots.Add(knot2);
                // }
                // else
                // {


                // }
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
        else if (IsUp)
        {
            lastSelectedSlot = null;
            selectedSlots.Clear();
            computer.SetPoints(new SplinePoint[0]);
            computer.Rebuild();
            // container.Splines[0].Clear();
        }
    }

    private bool isTurn(Slot foundSlot, out Vector3 initialDir, out Vector3 currentDir)
    {
        if (selectedSlots.Count >= 2)
        {
            Slot lastOne = selectedSlots[^1];
            Slot lastTwo = selectedSlots[^2];
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

    private void Old()
    {
        // countDown -= Time.deltaTime;
        // if (countDown < 0 && !dragging)
        // {
        //     countDown = 2;
        //     InstantiatePF();
        // }

        if (IsDown)
        {
            dragging = true;
        }
        else if (IsClick && dragging)
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            // Vector3 mousePos = Input.mousePosition;
            // mousePos.z = 6;
            // mouseWorldPos = cam.ScreenToWorldPoint(mousePos);
            if (draggingObject == null && Physics.Raycast(ray, out hit) && hit.transform.GetComponent<ISlotObj>() != null)
            {
                draggingObject = hit.transform;
                dragObjOriginPos = draggingObject.transform.position;
                draggingObjSlot = hit.transform.GetComponent<ISlotObj>();
                draggingObjSlot.Slot?.SetShooter(null);
                // draggingObject.GetSlot()?.SetShooter(null);
            }
            else if (draggingObject != null && Physics.Raycast(ray, out hit, 100, roadMask))
            {
                draggingObject.transform.position = hit.point + Vector3.up;
                lastDragPos = hit.point;
            }
        }
        else if (IsUp)
        {
            if (draggingObject != null)
            {
                Slot nearestSlot = null;
                float distance = 100;
                foreach (var item in Slots)
                {
                    float dis = Vector3.Distance(lastDragPos, item.transform.position);
                    if (dis < distance)
                    {
                        distance = dis;
                        nearestSlot = item;
                    }
                }
                if (distance < 1 && nearestSlot.Obj == null)
                {
                    nearestSlot.SetShooter(draggingObjSlot);
                }
                // else if (distance < 1 && nearestSlot.shooter != null && draggingObject.UpgradeAble() && nearestSlot.shooter.GetModelIndex() == draggingObject.GetModelIndex())
                else if (distance < 1 && nearestSlot.Obj != null && draggingObjSlot.IsUpgradeAble && nearestSlot.Obj.ModelIndex == draggingObjSlot.ModelIndex)
                {
                    nearestSlot.Obj.Upgrade();
                    Destroy(draggingObject.gameObject);
                }
                else
                {
                    draggingObjSlot.Slot?.SetShooter(draggingObjSlot);
                }
                draggingObject = null;
                draggingObjSlot = null;
            }

            dragging = false;
        }
    }
}

