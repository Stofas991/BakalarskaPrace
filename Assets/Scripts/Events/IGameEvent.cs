using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameEvent
{
    void InitEvent();
    void StartEvent();
    bool UpdateEvent();
    void EndEvent();
}
