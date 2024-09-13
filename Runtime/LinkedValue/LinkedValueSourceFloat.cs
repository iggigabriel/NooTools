using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Noo.Tools
{
    [HideMonoScript, CreateAssetMenu(menuName = "Noo.Tools/Linked Value/Float", fileName = "New Float")]
    public class LinkedValueSourceFloat : LinkedValueSource<float> { }

    [Serializable]
    public class LinkedValueFloat : LinkedValue<float, LinkedValueSourceFloat> { }
}
