using UnityEngine;
using UnityEngine.InputSystem;

public class LEInputs : MonoBehaviour
{
    Camera cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mp = Pointer.current.position.ReadValue();
            Ray ray = cam.ScreenPointToRay(mp);
            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100))
            {
                if (hitInfo.collider.attachedRigidbody != null && hitInfo.collider.attachedRigidbody.GetComponent<Piece>())
                {

                    Piece selectedPiece = hitInfo.collider.attachedRigidbody.GetComponent<Piece>();
                    if (selectedPiece)
                    {
                        LEPieces.Instance.SelectPiece(selectedPiece);
                    }
                }
            }

        }
        if (Mouse.current.leftButton.isPressed)
        {

        }
    }
}
