using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewindController : MonoBehaviour
{
    private PlayerController playerController;
    private CRTEffect crtEffect;
    [SerializeField]private int rewindBufferLength;
    private Coroutine rewindCoroutine;
    private List<RewindBase> rewinds;

    private enum RewindState { None, Fixed, Interval};
    private RewindState rewindState;

    private void Start()
    {
        crtEffect = FindAnyObjectByType<CRTEffect>();
        playerController = FindAnyObjectByType<PlayerController>();
        rewinds = FindObjectsByType<RewindBase>(FindObjectsSortMode.None).ToList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            rewindState = RewindState.Fixed;
            playerController.enabled = false;
            crtEffect.enabled = true;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            rewindState = RewindState.None;
            playerController.enabled = true;
            crtEffect.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            rewindState = RewindState.Interval;
            rewindCoroutine = StartCoroutine(RewindByInterval(0.005f));
            playerController.enabled = false;
            crtEffect.enabled = true;
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            rewindState = RewindState.None;
            StopCoroutine(rewindCoroutine);
            playerController.enabled = true;
            crtEffect.enabled = false;
        }
    }

    private void FixedUpdate()
    {
        if (rewindState.Equals(RewindState.None))
        {
            foreach (RewindBase rewind in rewinds)
            {
                rewind.Record();
            }
            rewindBufferLength++;
        }

        if (rewindState.Equals(RewindState.Fixed))
        {
            foreach (RewindBase rewind in rewinds)
            {
                rewind.Rewind();
            }
            rewindBufferLength--;
            if (rewindBufferLength <= 0) rewindState = RewindState.None;
        }
    }

    private IEnumerator RewindByInterval(float interval)
    {
        while (true)
        {
            foreach (RewindBase rewind in rewinds)
            {
                rewind.Rewind();
            }
            rewindBufferLength--;
            if (rewindBufferLength <= 0) 
            {
                rewindState = RewindState.None;
                yield break;
            } 
            yield return new WaitForSeconds(interval);
        }
    }
}
