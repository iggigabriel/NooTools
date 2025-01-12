using System;
using System.Collections;
using UnityEngine;

namespace Noo.Tools
{
    /// <summary>
    /// Monolithic class that handles all the flow utility and dispatches many useful events
    /// </summary>
    public static class App
    {
        #region Initialization

        static GameObject gameObject;
        static AppComponent component;

        [NonSerialized]
        static Vector2Int? screenSize;

#if UNITY_EDITOR
        public static Vector2Int ScreenSize => screenSize.HasValue && Application.isPlaying ? screenSize.Value : new Vector2Int(Screen.width, Screen.height);
#else
        public static Vector2Int ScreenSize => screenSize ?? new Vector2Int(Screen.width, Screen.height);
#endif

        public static bool IsLandscape => ScreenSize.x > ScreenSize.y;
        public static bool IsPortrait => ScreenSize.y > ScreenSize.x;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            if (gameObject) return;

            gameObject = new GameObject("App");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            component = gameObject.AddComponent<AppComponent>();

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR_WIN // Temporary fix for Unity 6.0 Alt-f4 bug
            static bool FirstQuit() 
            {
                Application.wantsToQuit -= FirstQuit;
                static void Quit() { Application.Quit(); OnUpdate -= Quit; }
                OnUpdate += Quit;
                return false;
            }
            Application.wantsToQuit += FirstQuit;
#endif
        }

        public class AppComponent : MonoBehaviour
        {
            private void Update()
            {
                CheckIfResolutionChanged();

                OnUpdate?.Invoke();
            }

            private void LateUpdate()
            {
                OnLateUpdate?.Invoke();
            }

            private void FixedUpdate()
            {
                OnFixedUpdate?.Invoke();
            }

            private void CheckIfResolutionChanged()
            {
                if (!screenSize.HasValue || screenSize.Value != new Vector2Int(Screen.width, Screen.height))
                {
                    screenSize = new Vector2Int(Screen.width, Screen.height);
                    OnScreenSizeChange?.Invoke();
                }
            }
        }

#endregion

        #region Public Events

        public static event Action OnUpdate;
        public static event Action OnLateUpdate;
        public static event Action OnFixedUpdate;

        public static event Action OnScreenSizeChange;

        #endregion

        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            if (component == null) Initialize();
            return component.StartCoroutine(routine);
        }

        public static void StopCoroutine(Coroutine coroutine)
        {
            if (component == null) Initialize();
            component.StopCoroutine(coroutine);
        }

        public static void RunNextFrame(Action action)
        {
            StartCoroutine(Delay(action));

            static IEnumerator Delay(Action action)
            {
                yield return null;
                action?.Invoke();
            }
        }
    }
}
