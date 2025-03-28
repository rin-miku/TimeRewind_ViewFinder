using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RewindBase : MonoBehaviour
{
    public abstract void Record();
    public abstract void Rewind();
}
