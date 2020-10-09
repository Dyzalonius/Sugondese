using System.Collections.Generic;
using UnityEngine.Events;

namespace Dyzalonius.Sugondese.Entities
{
    [System.Serializable]
    public class OnBallsChangeEvent : UnityEvent<List<BallType>>
    {

    }
}