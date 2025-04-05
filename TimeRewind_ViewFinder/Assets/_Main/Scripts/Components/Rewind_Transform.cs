using UnityEngine;

public struct TransformValues
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}

[RequireComponent(typeof(Transform))]
public class Rewind_Transform : RewindBase
{
    private RewindBuffer<TransformValues> buffer = new RewindBuffer<TransformValues>();

    public override void Record()
    {
        TransformValues transformValues;
        transformValues.position = transform.position;
        transformValues.rotation = transform.rotation;
        transformValues.scale = transform.localScale;
        buffer.WriteBuffer(transformValues);
    }

    public override void Rewind()
    {
        TransformValues transformValues = buffer.ReadBuffer();
        transform.position = transformValues.position;
        transform.rotation = transformValues.rotation;
        transform.localScale = transformValues.scale;
    }
}
