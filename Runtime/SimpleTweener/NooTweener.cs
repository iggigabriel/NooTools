using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

namespace Noo.Tools.NooTween
{
    public class NooTweener : MonoBehaviour
    {
        public float delay;
        public float speed = 1f;

        [SerializeField, HideLabel, InlineProperty]
        private NooTween tween;

        private NooTweenPlayer player;

        private void OnEnable()
        {
            StartCoroutine(Play());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            player?.Dispose();
            player = null;
        }

        IEnumerator Play()
        {
            if (delay > 0f) yield return new WaitForSeconds(delay / speed);
            player = tween.PlayOn(gameObject);
        }

        private void LateUpdate()
        {
            player?.Update(Time.smoothDeltaTime * speed);
        }
    }
}
