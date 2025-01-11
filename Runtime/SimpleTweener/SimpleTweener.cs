using Sirenix.OdinInspector;
using UnityEngine;

namespace Noo.Tools.SimpleTweener
{
    [HideMonoScript]
    public class SimpleTweener : MonoBehaviour
    {
        [SerializeField, HideLabel, InlineProperty]
        private SimpleTween tween;

        private void OnEnable()
        {
            tween.Play(gameObject);
        }

        private void OnDisable()
        {
            tween.Reset();
        }

        private void LateUpdate()
        {
            tween.Update(Time.smoothDeltaTime);
        }
    }
}
