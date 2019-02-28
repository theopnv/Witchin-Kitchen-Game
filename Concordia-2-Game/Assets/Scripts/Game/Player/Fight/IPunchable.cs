using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPunchable
{
    void Punch(Vector3 knockVelocity, float stunTime);
}
