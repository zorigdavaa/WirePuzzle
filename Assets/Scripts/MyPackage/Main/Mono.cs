using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace ZPackage
{
    public class Mb : MonoBehaviour
    {
        private static InputAction clickAction;
        public static InputAction ClickAction
        {
            get
            {
                if (clickAction == null)
                {
                    clickAction = InputSystem.actions.FindAction("Click");
                    clickAction.Enable();
                }
                return clickAction;
            }
        }

        ///<summary>Эхлэж байгааг шалгана</summary>
        public static bool IsStarting => GameManager.Instance.State == GameState.Starting;

        ///<summary>Тоглож байгааг шалгана</summary>
        public static bool IsPlaying => GameManager.Instance.State == GameState.Playing;

        ///<summary>Зогсоосон байгааг шалгана</summary>
        public static bool IsPause => GameManager.Instance.State == GameState.Pause;

        ///<summary>Хожсон эсэхийг шалгана</summary>
        public static bool IsLevelCompleted => GameManager.Instance.State == GameState.LevelCompleted;

        ///<summary>Хожигдсон эсэхийг шалгана</summary>
        public static bool IsGameOver => GameManager.Instance.State == GameState.GameOver;

        ///<summary>Тохиргоо хийж байгааг шалгана</summary>
        public static bool IsSettings => GameManager.Instance.State == GameState.Settings;

        ///<summary>Input.GetMouseButton(0)</summary>
        // public static bool IsClick => Input.GetMouseButton(0);
        public static bool IsClick => ClickAction.IsPressed();

        ///<summary>Input.GetMouseButtonDown(0)</summary>
        // public static bool IsDown => Input.GetMouseButtonDown(0);
        public static bool IsDown => ClickAction.WasPressedThisFrame();

        ///<summary>Input.GetMouseButtonUp(0)</summary>
        // public static bool IsUp => Input.GetMouseButtonUp(0);
        public static bool IsUp => ClickAction.WasReleasedThisFrame();

        ///<summary>Input.mousePosition</summary>
        public static Vector3 MP => Pointer.current.position.ReadValue();

        [HideInInspector]
        public Vector3 mp;

        ///<summary>Time.deltaTime</summary>
        public static float DT => Time.deltaTime;

        ///<summary>gameObject</summary>
        public GameObject go => gameObject;

        ///<summary>transform</summary>
        public Transform tf => transform;

        ///<summary>Rigidbody</summary>
        public Rigidbody rb
        {
            get
            {
                if (!_rb)
                    _rb = go.GetComponent<Rigidbody>();
                return _rb;
            }
        }
        Rigidbody _rb;
        public void WaitAndCall(float waitTime, Action call)
        {
            StartCoroutine(LocalCoroutine());
            IEnumerator LocalCoroutine()
            {
                yield return new WaitForSeconds(waitTime);
                call();
            }
        }
        public static GameObject GetUIObjectUnderPointer()
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = MP;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0)
            {
                Debug.Log(results[0].gameObject + " is top most UI");
                return results[0].gameObject; // Topmost UI element
            }

            return null;
        }
    }
}
