using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewindController : MonoBehaviour
{
    private PlayerController playerController;
    private bool needRewind;
    private List<RewindBase> rewinds;

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        rewinds = FindObjectsByType<RewindBase>(FindObjectsSortMode.None).ToList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            needRewind = true;
            playerController.enabled = false;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            needRewind = false;
            playerController.enabled = true;
        }
    }

    private void FixedUpdate()
    {
        if (needRewind)
        {
            foreach (RewindBase rewind in rewinds)
            {
                rewind.Rewind();
            }
        }
        else
        {
            foreach (RewindBase rewind in rewinds)
            {
                rewind.Record();
            }
        }
    }
}
