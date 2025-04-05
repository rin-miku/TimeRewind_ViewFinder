using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RewindController : MonoBehaviour
{
    private PlayerController playerController;
    private CRTEffect crtEffect;
    private AudioSource audioSource;
    [SerializeField]private int rewindBufferLength;
    private Coroutine rewindCoroutine;
    private List<RewindBase> rewinds;

    private enum RewindState { None, Fixed, Interval};
    private RewindState rewindState;

    private void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
        crtEffect = FindAnyObjectByType<CRTEffect>();
        audioSource = GetComponent<AudioSource>();
        rewinds = FindObjectsByType<RewindBase>(FindObjectsSortMode.None).ToList();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartRewind(RewindState.Fixed);
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            StopRewind();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            StartRewind(RewindState.Interval);
        }
        if (Input.GetKeyUp(KeyCode.T))
        {
            StopRewind();
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
            if (rewindBufferLength <= 0) StopRewind();
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
                StopRewind();
                yield break;
            } 
            yield return new WaitForSeconds(interval);
        }
    }

    private void StartRewind(RewindState state)
    {
        rewindState = state;
        playerController.enabled = false;
        crtEffect.enabled = true;
        audioSource.Play();

        if (state.Equals(RewindState.Interval))
        {
            rewindCoroutine = StartCoroutine(RewindByInterval(0.005f));
        }
    }

    private void StopRewind()
    {
        if (rewindState.Equals(RewindState.Interval))
        {
            StopCoroutine(rewindCoroutine);
        }

        rewindState = RewindState.None;
        playerController.enabled = true;
        crtEffect.enabled = false;
        audioSource.Stop();
    }
}
