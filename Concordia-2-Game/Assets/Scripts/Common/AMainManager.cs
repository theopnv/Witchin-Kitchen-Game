using System.Collections.Generic;
using UnityEngine;

namespace con2
{
    public abstract class AMainManager : MonoBehaviour
    {
        public abstract List<IInputConsumer> GetInputConsumers(int playerIndex);
    }
}