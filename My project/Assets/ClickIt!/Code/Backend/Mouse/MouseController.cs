using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ClickIt.Backend {
    internal class MouseController : IDisposable {
        private IMouse mouse;
        private InputActionAsset inputActions;
        private IMouseButtonRouter router;

        internal MouseController(IMouseButtonRouter router, IMouse mouse, InputActionAsset actionAsset) {
            this.router = router;

            this.mouse = mouse;
            inputActions = actionAsset;

            var mouseMap = inputActions.FindActionMap("Main");

            //Subscribe methods to trigger when an input action event occurs
            //Performed = any change in ctx
            //Started = when ctx becomes 1
            //Cancelled = when ctx becomes 0
            mouseMap.FindAction("MouseMove").performed += ctx => OnMouseMove(ctx);
            mouseMap.FindAction("LeftMouseButton").started += ctx => OnLeftButton(ctx);
            mouseMap.FindAction("LeftMouseButton").canceled += ctx => OnLeftButton(ctx);
            mouseMap.FindAction("RightMouseButton").started += ctx => OnRightButton(ctx);
            mouseMap.FindAction("RightMouseButton").canceled += ctx => OnRightButton(ctx);
            mouseMap.FindAction("MiddleMouseButton").started += ctx => OnMiddleButton(ctx);
            mouseMap.FindAction("MiddleMouseButton").canceled += ctx => OnMiddleButton(ctx);

            //Start listening for input
            mouseMap.Enable();
        }

        public void Dispose() {
            var mouseMap = inputActions.FindActionMap("Main");
            if (mouseMap != null) {
                mouseMap.FindAction("MouseMove").performed -= OnMouseMove;
                mouseMap.FindAction("LeftMouseButton").started -= OnLeftButton;
                mouseMap.FindAction("LeftMouseButton").canceled -= OnLeftButton;
                mouseMap.FindAction("RightMouseButton").started -= OnRightButton;
                mouseMap.FindAction("RightMouseButton").canceled -= OnRightButton;
                mouseMap.FindAction("MiddleMouseButton").started -= OnMiddleButton;
                mouseMap.FindAction("MiddleMouseButton").canceled -= OnMiddleButton;
                
                mouseMap.Disable();
            }
        }

        void OnMouseMove(InputAction.CallbackContext context) {
            Vector2 mouseScreenPosition = context.ReadValue<Vector2>();
            mouse.SetScreenPosition(mouseScreenPosition);
        }

        void OnLeftButton(InputAction.CallbackContext context) {
            bool isButtonDown = context.ReadValue<float>() == 1;

            if (isButtonDown) { //click down
                mouse.SetButtonDown(MouseButton.left);
                router.DoLeftClick(mouse.ScreenPosition);
            }
            else { //release
                mouse.SetButtonUp(MouseButton.left);
                router.DoLeftRelease();
            }
        }

        void OnRightButton(InputAction.CallbackContext context) {
            bool isButtonDown = context.ReadValue<float>() == 1;

            if (isButtonDown) { //click down
                mouse.SetButtonDown(MouseButton.right);
                router.DoRightClick(mouse.ScreenPosition);
            }
            else { //release
                mouse.SetButtonUp(MouseButton.right);
                router.DoRightRelease();
            }
        }

        void OnMiddleButton(InputAction.CallbackContext context) {
            bool isButtonDown = context.ReadValue<float>() == 1;

            if (isButtonDown) { //click down
                mouse.SetButtonDown(MouseButton.middle);
                router.DoMiddleClick(mouse.ScreenPosition);
            }
            else { //release
                mouse.SetButtonUp(MouseButton.middle);
                router.DoMiddleRelease();
            }
        }
    }
}