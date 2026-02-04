using Sirenix.OdinInspector;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UI;

namespace Noo.Tools
{
    [ExecuteAlways]
    public sealed class PixelPerfectRenderer : MonoBehaviour
    {
        public static event Action<PixelPerfectRenderer> OnScreenSizeChanged;

        [SerializeField]
        Camera gameCamera;

        [SerializeField]
        Camera screenCamera;

        [SerializeField]
        CanvasScaler screenCanvasScaler;

        [SerializeField]
        RawImage screenRenderer;

        [SerializeField, Tooltip("This is height in landscape mode and width in portrait mode")]
        int renderSizeInPixels = 256;

        [SerializeField]
        int pixelSizeRounding = 4;

        [SerializeField]
        int pixelsPerUnit = 1;

        [SerializeField]
        int cameraTextureDepth = 32;

        [SerializeField, Range(1f, 10f)]
        float zoom = 1f;

        [SerializeField]
        DefaultFormat cameraTextureFormat = DefaultFormat.HDR;

        [SerializeField, Tooltip("It will fixes scalers and graphics raycasters for these canvases")]
        Canvas[] affectedCanvases;

        [ShowInInspector, ReadOnly, Title("Read-only")]
        public RenderTexture GameRenderTexture { get; private set; }

        [ShowInInspector, ReadOnly]
        public int GameRenderScale { get; private set; }

        [ShowInInspector, ReadOnly]
        public Vector2Int GameRenderSize { get; private set; }

        [ShowInInspector, ReadOnly]
        public Vector2Int ScreenRenderSize { get; private set; }

        [ShowInInspector, ReadOnly]
        public Rect ScreenRect { get; private set; }

        [ShowInInspector, ReadOnly]
        public Vector2 WorldSize { get; private set; }

        [ShowInInspector, ReadOnly]
        public Vector2 WorldPosition { get; private set; }

        [ShowInInspector, ReadOnly]
        public Rect WorldRect => new(WorldPosition - WorldSize / 2, WorldSize);

        [ShowInInspector, ReadOnly]
        public bool IsLandscape { get; private set; }

        [ShowInInspector, ReadOnly]
        public float AspectRatio { get; private set; }

        float currentZoom;

        public float Zoom { get => zoom; set => zoom = Mathf.Max(1f, value); }

        public Vector2 ScreenToWorldSpace(Vector2 screenPosition)
        {
            var viewportPosition = new Vector2(screenPosition.x / ScreenRenderSize.x, screenPosition.y / ScreenRenderSize.y);
            var localPosition = ScreenRect.PointToNormalizedUnclamped(viewportPosition);
            var worldRect = new Rect(WorldPosition - WorldSize / 2f, WorldSize);
            return worldRect.NormalizedToPointUnclamped(localPosition);
        }

        private void Update()
        {
            UpdateRenderTargets();
        }

#if UNITY_EDITOR
        private void LateUpdate()
        {
            if (!Application.isPlaying)
            {
                UpdateRenderTargets();
            }
        }
#endif

        public void UpdateRenderTargets()
        {
            if (!screenCamera) return;

            var screenRenderSize = new Vector2Int(screenCamera.pixelWidth, screenCamera.pixelHeight);
            var screenAspectRatio = (float)screenRenderSize.x / screenRenderSize.y;

            var renderScale = (renderSizeInPixels / pixelSizeRounding) * pixelSizeRounding;

            if (ScreenRenderSize != screenRenderSize || GameRenderScale != renderScale) // Screen size changed or game size changed
            {
                ScreenRenderSize = screenRenderSize;
                GameRenderScale = renderScale;

                IsLandscape = screenRenderSize.x > screenRenderSize.y;

                AspectRatio = Mathf.Clamp(screenAspectRatio, 1f / 2.34f, 2.34f); // Max ultrawide screen

                GameRenderSize = IsLandscape ?
                    new Vector2Int(Mathf.CeilToInt((GameRenderScale * AspectRatio) / pixelSizeRounding) * pixelSizeRounding, GameRenderScale) :
                    new Vector2Int(GameRenderScale, Mathf.CeilToInt((GameRenderScale / AspectRatio) / pixelSizeRounding) * pixelSizeRounding);

                AspectRatio = (float)GameRenderSize.x / GameRenderSize.y;

                if (GameRenderTexture != null) GameRenderTexture.Release();

                GameRenderTexture = new(GameRenderSize.x, GameRenderSize.y, cameraTextureDepth, cameraTextureFormat);
                GameRenderTexture.hideFlags = HideFlags.DontSave;

                var scale = IsLandscape ? screenRenderSize.y / (float)GameRenderSize.y : screenRenderSize.x / (float)GameRenderSize.x;
                var scaledScreenSize = (Vector2)GameRenderSize * scale / ScreenRenderSize;

                ScreenRect = new Rect((scaledScreenSize - Vector2.one) / -2f, scaledScreenSize);

                WorldSize = (Vector2)GameRenderSize / pixelsPerUnit;

                if (gameCamera)
                {
                    gameCamera.targetTexture = GameRenderTexture;
                    gameCamera.orthographicSize = (float)GameRenderSize.y / pixelsPerUnit / 2f;
                }

                if (screenRenderer)
                {
                    screenRenderer.texture = GameRenderTexture;
                    screenRenderer.rectTransform.sizeDelta = new Vector2(GameRenderSize.x, GameRenderSize.y) / GameRenderScale * zoom;
                }

                if (screenCanvasScaler)
                {
                    screenCanvasScaler.matchWidthOrHeight = IsLandscape ? 1f : 0f;
                }

                foreach (var canvas in affectedCanvases)
                {
                    if (canvas.TryGetComponent<PixelPerfectGraphicRaycaster>(out var raycaster))
                    {
                        raycaster.screenScale = scale;
                    }
                }

                OnScreenSizeChanged?.Invoke(this);
            }

            if (currentZoom != zoom)
            {
                currentZoom = zoom;

                if (screenRenderer)
                {
                    screenRenderer.materialForRendering.SetFloat("_Zoom", zoom);
                    screenRenderer.SetMaterialDirty();
                    screenRenderer.rectTransform.sizeDelta = new Vector2(GameRenderSize.x, GameRenderSize.y) / GameRenderScale * zoom;
                }
            }
        }

        private void OnValidate()
        {
            renderSizeInPixels = Mathf.Max(renderSizeInPixels, pixelSizeRounding);
            pixelsPerUnit = Mathf.Max(pixelsPerUnit, 1);
        }

        public bool CaptureScreenshot(out string screenshotPath)
        {
            try
            {
                var tmpTex = new RenderTexture(GameRenderTexture.width, GameRenderTexture.height, 32, RenderTextureFormat.ARGB32);

                gameCamera.targetTexture = tmpTex;
                gameCamera.Render();

                var dstTex = new Texture2D(GameRenderTexture.width, GameRenderTexture.height, TextureFormat.ARGB32, false);

                RenderTexture.active = tmpTex;
                dstTex.ReadPixels(new Rect(0, 0, GameRenderTexture.width, GameRenderTexture.height), 0, 0);
                RenderTexture.active = null;

                gameCamera.targetTexture = GameRenderTexture;

                var pngBytes = dstTex.EncodeToPNG();

                var screenshotName = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmssff}.png";
                screenshotPath = Path.Combine(Application.persistentDataPath, "Screenshots", screenshotName);

                Directory.CreateDirectory(Path.GetDirectoryName(screenshotPath));

                File.WriteAllBytes(screenshotPath, pngBytes);

                tmpTex.Release();

                Destroy(dstTex);
                Destroy(tmpTex);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                screenshotPath = string.Empty;
                return false;
            }

            return true;
        }

        public Vector2 ScreenToWorldPoint(Vector2 screenPosition)
        {
            var viewportPosition = screenCamera.ScreenToViewportPoint(screenPosition);
            return gameCamera.ViewportToWorldPoint(viewportPosition);
        }
    }
}
