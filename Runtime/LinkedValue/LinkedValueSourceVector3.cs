using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Noo.Tools
{
    [HideMonoScript, CreateAssetMenu(menuName = "Noo.Tools/Linked Value/Vector3", fileName = "New Vector3")]
    public class LinkedValueSourceVector3 : LinkedValueSource<Vector3> { }

    [Serializable]
    public class LinkedValueVector3 : LinkedValue<Vector3, LinkedValueSourceVector3> { }
}
