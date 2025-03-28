using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewindController : MonoBehaviour
{
    private bool needRewind;
    private List<RewindBase> rewinds;

    // Start is called before the first frame update
    void Start()
    {
        rewinds = FindObjectsByType<RewindBase>(FindObjectsSortMode.None).ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            needRewind = true;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            needRewind = false;
        }
    }

    private void FixedUpdate()
    {
        if (needRewind)
        {
            foreach(RewindBase rewind in rewinds)
            {
                rewind.Rewind();
            }
        }
        else
        {
            foreach(RewindBase rewind in rewinds)
            {
                rewind.Record();
            }
        }
    }
}
