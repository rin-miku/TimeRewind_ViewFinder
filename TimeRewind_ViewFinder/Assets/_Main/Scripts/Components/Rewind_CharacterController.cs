using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterControllerValues
{
    public Vector3 velocity;
}

[RequireComponent(typeof(CharacterController))]
public class Rewind_CharacterController : RewindBase
{
    private CharacterController characterController;
    private RewindBuffer<CharacterControllerValues> buffer;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    public override void Record()
    {
        CharacterControllerValues characterControllerValues;
        characterControllerValues.velocity = characterController.attachedRigidbody.velocity;
        buffer.WriteBuffer(characterControllerValues);
    }

    public override void Rewind()
    {
        
    }
}
