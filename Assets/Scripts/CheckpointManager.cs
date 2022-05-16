using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private float MaxTimeToReachNextCheckpoint = 30f;
    private float TimeLeft = 30f;

    public KartAgent kartAgent;
    public Checkpoint nextCheckPointToReach;

    private int CurrentCheckpointIndex;
    private List<Checkpoint> Checkpoints;
    private Checkpoint lastCheckpoint;

    public event Action<Checkpoint> reachedCheckpoint;

    void Start()
    {
        Checkpoints = FindObjectOfType<Checkpoints>().checkPoints;
        ResetCheckpoints();
    }

    public void ResetCheckpoints()
    {
        CurrentCheckpointIndex = 0;
        Checkpoints.ForEach(cpt => cpt.gameObject.SetActive(false));
        TimeLeft = MaxTimeToReachNextCheckpoint;
        SetNextCheckpoint();
    }

    private void Update()
    {
        TimeLeft -= Time.deltaTime;

        Checkpoints[CurrentCheckpointIndex].gameObject.SetActive(true);

        if (TimeLeft < 0f)
        {
            kartAgent.AddReward(-1f);
            kartAgent.EndEpisode();
        }
    }

    public void CheckPointReached(Checkpoint checkpoint)
    {
        if (nextCheckPointToReach != checkpoint) return;

        lastCheckpoint = Checkpoints[CurrentCheckpointIndex];
        lastCheckpoint.gameObject.SetActive(false);
        reachedCheckpoint?.Invoke(checkpoint);
        CurrentCheckpointIndex++;

        if (CurrentCheckpointIndex >= Checkpoints.Count)
        {
            kartAgent.AddReward(0.5f);
            kartAgent.EndEpisode();
        }
        else
        {
            kartAgent.AddReward((0.5f) / Checkpoints.Count);
            SetNextCheckpoint();
        }
    }

    private void SetNextCheckpoint()
    {
        if (Checkpoints.Count > 0)
        {
            TimeLeft = MaxTimeToReachNextCheckpoint;
            nextCheckPointToReach = Checkpoints[CurrentCheckpointIndex];

        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name.Contains("Wall") || other.gameObject.name.Contains("Border"))
        {
            kartAgent.AddReward(-0.001f);
            Debug.Log(other.gameObject.name);
        }
    }
}
