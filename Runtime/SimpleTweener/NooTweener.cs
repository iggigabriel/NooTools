using Sirenix.OdinInspector;
using UnityEngine;

namespace Noo.Tools.NooTween
{
    public class NooTweener : MonoBehaviour
    {
        [SerializeField, HideLabel, InlineProperty]
        private NooTween tween;

        private NooTweenPlayer player;

        private void OnEnable()
        {
            player = tween.PlayOn(gameObject);
        }

        private void OnDisable()
        {
            player?.Dispose();
            player = null;
        }

        private void LateUpdate()
        {
            player?.Update(Time.smoothDeltaTime);
        }
    }
}
