using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThGold.Common;
using ThGold.Event;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

namespace GamePlay
{
    public class InputManager : MonoSingleton<InputManager>
    {
        #region Player

        public Vector2 moveValue = Vector2.zero;

        #endregion

        public void OnMove(InputAction.CallbackContext context)
        {
            moveValue = context.ReadValue<Vector2>();
            Debug.Log(moveValue);
            //Debug.Log(moveValue);
        }

        public void OnClick(InputAction.CallbackContext context)
        {
            switch (context.phase)
            {
                case InputActionPhase.Started:
                    Debug.Log("操作开始");
                    break;
                case InputActionPhase.Performed:
                    if (context.interaction is PressInteraction)
                    {
                        Debug.Log("执行点击逻辑");
                    }

                    if (context.interaction is MultiTapInteraction)
                    {
                        Debug.Log("执行双击逻辑");
                    }
                    else if (context.interaction is HoldInteraction)
                    {
                        Debug.Log("执行长按逻辑");
                    }
                    else if (context.interaction is SlowTapInteraction)
                    {
                        Debug.Log("长点击");
                    }
                    else
                    {
                        Debug.Log(context.interaction);
                    }

                    break;
                case InputActionPhase.Waiting:
                    if (context.interaction is HoldInteraction)
                    {
                        Debug.Log("Waiting:执行长按逻辑");
                    }
                    else if (context.interaction is SlowTapInteraction)
                    {
                        Debug.Log("Waiting:长点击");
                    }

                    break;
                    ;
                case InputActionPhase.Canceled:
                    Debug.Log("Canceled:取消");
                    break;
            }
        }

        public void StartBehavior()
        {
        }

        public void StopBehavior()
        {
        }

        public void ShutDownBehavior()
        {
        }
    }
}