using UnityEngine;

public struct RigidbodyValues
{
    public bool useGravity;
    public Vector3 velocity;
}

[RequireComponent(typeof(Rigidbody))]
public class Rewind_Rigidbody : RewindBase
{
    private Rigidbody rb;
    private RewindBuffer<RigidbodyValues> buffer = new RewindBuffer<RigidbodyValues>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void Record()
    {
        RigidbodyValues rigidbodyValues;
        rigidbodyValues.useGravity = rb.useGravity;
        rigidbodyValues.velocity = rb.velocity;
        buffer.WriteBuffer(rigidbodyValues);
    }

    public override void Rewind()
    {
        RigidbodyValues rigidbodyValues = buffer.ReadBuffer();
        rb.useGravity = rigidbodyValues.useGravity;
        rb.velocity = rigidbodyValues.velocity;
    }
}
