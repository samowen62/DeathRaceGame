using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class PlacementManager : PausableBehaviour
{
    public GameContext gameContext;

    //TODO:restrict these to only be able to set to checkpoints
    //also these should have the largest num_in_seq. that number goes down to 0
    public TrackPoint firstCheckPoint_A;

    public TrackPoint firstCheckPoint_B;

    public TrackPoint firstCheckPoint_C;

    public int laps = 1;

    private TrackPoint.PathChoice[] validPaths;

    private const int skipTrackPoints = 10;

    private List<PlayerPlacement> placementList = new List<PlayerPlacement>();

    private Dictionary<RacePlayer, PlayerPlacement> listOfPlayers = new Dictionary<RacePlayer, PlayerPlacement>();

    private void Start()
    {
        var paths = new List<TrackPoint.PathChoice>();
        if (firstCheckPoint_A != null && firstCheckPoint_A.isCheckPoint)
        {
            paths.Add(TrackPoint.PathChoice.PATH_A);
        }
        if (firstCheckPoint_B != null && firstCheckPoint_B.isCheckPoint)
        {
            paths.Add(TrackPoint.PathChoice.PATH_B);
        }
        if (firstCheckPoint_C != null && firstCheckPoint_C.isCheckPoint)
        {
            paths.Add(TrackPoint.PathChoice.PATH_C);
        }
        validPaths = paths.ToArray();
    }

    /**
     * Adds the list of RacePlayers to the list of possible players
     */
    public void addPlayers(List<RacePlayer> players)
    {
        players.ForEach(player =>
        {
            PlayerPlacement playerPlacement = new PlayerPlacement(firstCheckPoint_A, firstCheckPoint_B, firstCheckPoint_C, laps);
            listOfPlayers.Add(player, playerPlacement);
            placementList.Add(playerPlacement);
        });
    }

    /**
     * Called when a player enters a track point. Used when determining player placement
     * returns true if the player entered a new checkpoint
     */
    public bool updateTrackPoint(RacePlayer player, TrackPoint trackPoint)
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
            case TrackPoint.PathChoice.PATH_C:
                if (trackPoint.isCheckPoint && playerPlacement.latestCheckPoint_C.isNextValidCheckPoint(trackPoint))
                {
                    playerPlacement.latestCheckPoint_C = trackPoint;
                    playerPlacement.latestPointPath_C = trackPoint.num_in_seq;
                    return true;
                }

                if (trackPoint.num_in_seq > playerPlacement.latestPointPath_C - skipTrackPoints)
                {
                    playerPlacement.latestPointPath_C = trackPoint.num_in_seq;
                }
                break;
        }

        return false;
    }

    /**
     * Called when a RacePlayer crosses the finish line
     */
    public void crossFinish(RacePlayer player)
    {
        //I think this is failing on AI finish
        if (validCross(player)) {

            listOfPlayers[player].finishLap(pauseInvariantTime);
            Debug.Log(player.name + " entered lap " + listOfPlayers[player].lap);

            //check if player finished
            if (listOfPlayers[player].finished)
            {
                gameContext.finishPlayer(player);
            }
        }
    }

    private bool validCross(RacePlayer player)
    {
        foreach (var path in validPaths)
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

            if (valid) return true;
        }
        return false;
    }

    /**
     * Returns the placement of the given RacePlayer
     */
    public int getPlacementOf(RacePlayer player)
    {
        placementList.Sort();
        return placementList.IndexOf(listOfPlayers[player]) + 1;
    }

    public List<RacePlayer> getUnfinishedPlayersOrdered()
    {
        placementList.Sort();
        return placementList
            .Where(player => !player.finished)
            .Select(playerPlacement => listOfPlayers
                .FirstOrDefault(x => x.Value == playerPlacement).Key
            )
            .ToList();
    }

    public float[] getLapTimesForPlayer(RacePlayer player)
    {
        return listOfPlayers[player].lapTimes;
    }

    public float getLastLapStart(RacePlayer player)
    {
        return listOfPlayers[player].getLastLapStart();
    }

    //fill lap times
    public void forcePlayerFinish(RacePlayer player)
    {
        listOfPlayers[player].forcePlayerFinish();
    }

    private class PlayerPlacement : IComparable
    {
        public int lap { get; private set; }
        public int laps { get; private set; }
        public float[] lapTimes { get; private set; }
        public float[] lapStart { get; private set; }

        public bool finished { get; private set; }

        public int latestPointPath_A;
        public int latestPointPath_B;
        public int latestPointPath_C;

        private int numPoints_A;
        private int numPoints_B;
        private int numPoints_C;
        private int totalPoints;

        public TrackPoint latestCheckPoint_A;
        public TrackPoint latestCheckPoint_B;
        public TrackPoint latestCheckPoint_C;

        public PlayerPlacement(TrackPoint firstCheckPoint_A, TrackPoint firstCheckPoint_B, TrackPoint firstCheckPoint_C, int _laps)
        {
            lap = 0;
            laps = _laps;
            lapTimes = new float[laps];
            lapStart = new float[laps];
            latestPointPath_A = 0;
            numPoints_A = firstCheckPoint_A == null ? 0 : firstCheckPoint_A.num_in_seq;
            latestPointPath_B = 0;
            numPoints_B = firstCheckPoint_B == null ? 0 : firstCheckPoint_B.num_in_seq;
            latestPointPath_C = 0;
            numPoints_C = firstCheckPoint_C == null ? 0 : firstCheckPoint_C.num_in_seq;
            latestCheckPoint_A = firstCheckPoint_A;
            latestCheckPoint_B = firstCheckPoint_B;
            latestCheckPoint_C = firstCheckPoint_C;
            totalPoints = numPoints_A + numPoints_B + numPoints_C;

            //set all of lapTimes to 0f
            for (int i = 0; i < lapTimes.Length; i++) lapTimes[i] = 0f;
        }

        public int CompareTo(object other)
        {
            PlayerPlacement that = other as PlayerPlacement;
            return totalPoints * (this.lap - that.lap) + 
                (this.latestPointPath_A - that.latestPointPath_A +
                this.latestPointPath_B - that.latestPointPath_B +
                this.latestPointPath_C - that.latestPointPath_C);
        }

        //finishes the lap and increments it
        public void finishLap(float time)
        {
            if (finished) return;
            //first time crossing the player starts lap 1
            //ASSUME all players must be behind the finish line to start
            if (lap == 0)
            {
                lapStart[0] = time;
            }
            else
            {
                lapTimes[lap - 1] = time - lapStart[lap - 1];
                if (lap == laps)
                {
                    finished = true;
                }
                else
                {
                    lapStart[lap] = time;
                }
            }

            latestPointPath_A = numPoints_A;
            latestPointPath_B = numPoints_B;
            latestPointPath_C = numPoints_C;
            lap++;
        }

        //TODO: make this method a little better. it's just a mess now
        public void forcePlayerFinish()
        {
            float lastLapTime = 0f;
            for (int i = lapTimes.Length - 1; i >= 0; i--)
            {
                if (lapTimes[i] > 0)
                {
                    lastLapTime = lapTimes[i];
                    break;
                }
            }

            //TODO may want to factor in distance from finish line instead of just adding 5 seconds [i.e. trackpoint num]
            float lapTime = lastLapTime + 5f;
            for (int i = lapTimes.Length - 1; i >= 0; i--)
            {
                if (lapTimes[i] > 0)
                {
                    break;
                }
                else
                {
                    lapTimes[i] = lapTime;
                }
            }
        }

        public float getLastLapStart()
        {
            if (lap == 0)
                return 0f;
            else if (lap >= laps)
                return lapStart[laps - 1];
            else
                return lapStart[lap - 1];
        }
    }

}
