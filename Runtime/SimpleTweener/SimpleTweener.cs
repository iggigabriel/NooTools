using Sirenix.OdinInspector;
using UnityEngine;

namespace Noo.Tools
{
    [HideMonoScript]
    public class SimpleTweener : MonoBehaviour
    {
        [SerializeField, HideLabel, InlineProperty]
        private SimpleTween tween;
    }
}
