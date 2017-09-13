using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlacementManager : MonoBehaviour
{

    //TODO:restrict these to only be able to set to checkpoints
    //also these should have the largest num_in_seq. that number goes down to 0
    public TrackPoint firstCheckPoint_A;

    public TrackPoint firstCheckPoint_B;

    public TrackPoint firstCheckPoint_C;

    public int laps = 1;

    public TrackPoint.PathChoice[] validPaths;

    private const int skipTrackPoints = 10;

    private Dictionary<RacePlayer, PlayerPlacement> listOfPlayers = new Dictionary<RacePlayer, PlayerPlacement>();

    public void addPlayers(List<RacePlayer> players)
    {
        players.ForEach(player =>
        {
            listOfPlayers.Add(player, new PlayerPlacement(firstCheckPoint_A, firstCheckPoint_B, firstCheckPoint_C));
        });
    }

    public bool updateTrackPoint(RacePlayer player, TrackPoint trackPoint)//return true if this valid next checkPoint
    {

        PlayerPlacement playerPlacement = listOfPlayers[player];

        switch (trackPoint.pathChoice)
        {
            case TrackPoint.PathChoice.PATH_A:
                if (trackPoint.isCheckPoint && playerPlacement.latestCheckPoint_A.isNextValidCheckPoint(trackPoint))
                {
                    playerPlacement.latestCheckPoint_A = trackPoint;
                    playerPlacement.latestPointPath_A = trackPoint.num_in_seq;
                    return true;
                }

                if (trackPoint.num_in_seq > playerPlacement.latestPointPath_A - skipTrackPoints)
                {
                    playerPlacement.latestPointPath_A = trackPoint.num_in_seq;
                }
                break;

            case TrackPoint.PathChoice.PATH_B:
                if (trackPoint.isCheckPoint && playerPlacement.latestCheckPoint_B.isNextValidCheckPoint(trackPoint))
                {
                    playerPlacement.latestCheckPoint_B = trackPoint;
                    playerPlacement.latestPointPath_B = trackPoint.num_in_seq;
                    return true;
                }

                if (trackPoint.num_in_seq > playerPlacement.latestPointPath_B - skipTrackPoints)
                {
                    playerPlacement.latestPointPath_B = trackPoint.num_in_seq;
                }
                break;
        }

        return false;
    }

    //returns true if the racer finished
    public bool crossFinish(RacePlayer player)
    {
        if (validCross(player)) { 
            if (!listOfPlayers[player].finished && listOfPlayers[player].lap == laps)
            {
                listOfPlayers[player].finished = true;
                return true;
            } else
            {
                listOfPlayers[player].lap++;
                Debug.Log(player.name + " entered lap " + listOfPlayers[player].lap);
            }
        }
        return false;
    }

    private bool validCross(RacePlayer player)
    {
        foreach(var path in validPaths)
        {
            bool valid = false;
            switch (path)
            {
                case TrackPoint.PathChoice.PATH_A:
                    if (listOfPlayers[player].latestPointPath_A < skipTrackPoints)
                        valid = true; break;
                case TrackPoint.PathChoice.PATH_B:
                    if (listOfPlayers[player].latestPointPath_B < skipTrackPoints)
                        valid = true; break;
                case TrackPoint.PathChoice.PATH_C:
                    if (listOfPlayers[player].latestPointPath_C < skipTrackPoints)
                        valid = true; break;

            }

            if (valid)
                return true;
        }
        return false;
    }

    int getPlacementOf(string player)
    {
        return 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private class PlayerPlacement
    {
        public int lap;

        public bool finished;

        public int latestPointPath_A;

        public int latestPointPath_B;

        public int latestPointPath_C;

        public TrackPoint latestCheckPoint_A;

        public TrackPoint latestCheckPoint_B;

        public TrackPoint latestCheckPoint_C;

        public PlayerPlacement(TrackPoint firstCheckPoint_A, TrackPoint firstCheckPoint_B, TrackPoint firstCheckPoint_C)
        {
            lap = 1;
            latestPointPath_A = firstCheckPoint_A == null ? -1 : firstCheckPoint_A.num_in_seq;
            latestPointPath_B = firstCheckPoint_B == null ? -1 : firstCheckPoint_B.num_in_seq;
            latestPointPath_C = firstCheckPoint_C == null ? -1 : firstCheckPoint_C.num_in_seq;
            latestCheckPoint_A = firstCheckPoint_A;
            latestCheckPoint_B = firstCheckPoint_B;
            latestCheckPoint_C = firstCheckPoint_C;
        }
    }

}
