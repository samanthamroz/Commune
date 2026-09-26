using System;
using System.Collections;
using ClickIt.Backend;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace ClickIt {
    [RequireComponent(typeof(PlayerInput))]
    public class ClickItCore : MonoBehaviour {
        internal static ClickItCore Instance;
        private MouseController mouseController;
        private IValidater validater;
        private IMouse mouse;
        private InteractableRaycaster raycaster;
        private IMouseButtonRouter router;

        void Awake() { //This is called automatically by Unity at the very beginning of every run of the game
            if (Instance != null && Instance != this) {
                Debug.LogWarning($"[ClickIt] Duplicate ClickIt instance found on {gameObject.name}. Destroying duplicate.");
                Destroy(gameObject);
                return; // IMPORTANT: Exit immediately
            }
            
            if (Instance == null) {
                Instance = this;
                InitializeSystem();
                DontDestroyOnLoad(gameObject);
            }
        }

        private void InitializeSystem() {
            validater = new ClickItValidater();
            mouse = new Backend.Mouse();
            raycaster = new InteractableRaycaster(); 
            router = new MouseButtonRouter(raycaster);

            InputActionAsset inputActions = Resources.Load<InputActionAsset>("ClickIt!_InputActions");
            mouseController = new MouseController(router, mouse, inputActions);

            DoSceneStart();
            SceneManager.sceneLoaded += SceneStart;
        }

        void OnDestroy() {
            if (Instance == this) {
                // Unsubscribe from scene events
                SceneManager.sceneLoaded -= SceneStart;
                
                // Dispose of mouse controller
                if (mouseController != null) {
                    (mouseController as IDisposable)?.Dispose();
                }
                
                Instance = null;
            }
        }

        void SceneStart(Scene scene, LoadSceneMode mode) { //Required Unity Function Signature
            DoSceneStart();
        }

        private void DoSceneStart() {
            #if UNITY_EDITOR
            validater.ValidateScene();
            #endif
        }

        //Access to mouse status
        public Vector2 GetMousePosition() {
            return mouse.ScreenPosition;
        }
        public bool IsMouseButtonDown(MouseButton button) {
            return mouse.IsButtonDown(button);
        }

        public void SimulateClickAtMousePosition(MouseButton button) {
            switch (button) {
                case MouseButton.left:
                    router.DoLeftClick(GetMousePosition());
                    break;
                case MouseButton.middle:
                    router.DoMiddleClick(GetMousePosition());
                    break;
                case MouseButton.right:
                    router.DoRightClick(GetMousePosition());
                    break;
            }
        }
        public void SimulateClick(MouseButton button, Vector2 screenPosition) {
            switch (button) {
                case MouseButton.left:
                    router.DoLeftClick(screenPosition);
                    break;
                case MouseButton.middle:
                    router.DoMiddleClick(screenPosition);
                    break;
                case MouseButton.right:
                    router.DoRightClick(screenPosition);
                    break;
            }
        }
        public void SimulateRelease(MouseButton button) {
            switch (button) {
                case MouseButton.left:
                    router.DoLeftRelease();
                    break;
                case MouseButton.middle:
                    router.DoMiddleRelease();
                    break;
                case MouseButton.right:
                    router.DoRightRelease();
                    break;
            }
        }
        
        public GameObject GetInteractableObjectAtPosition(Vector2 screenPosition) {
            return raycaster.GetInteractableGameObjectAtPosition(screenPosition);
        }
        public GameObject GetInteractableObjectAtMouse() {
            return raycaster.GetInteractableGameObjectAtPosition(GetMousePosition());
        }

        //Delay
        internal void DoDelayedAction(Action delayedAction, float timeToDelaySeconds) {
            StartCoroutine(DelayedAction(delayedAction, timeToDelaySeconds));
        }
        internal IEnumerator DelayedAction(Action delayedAction, float timeToDelaySeconds) {
            yield return new WaitForSeconds(timeToDelaySeconds);
            delayedAction();
        }
    }
}