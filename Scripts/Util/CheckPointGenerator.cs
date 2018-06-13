using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This class if for generating a valid chain of checkpoints
/// so that nextValidCheckPoints forms a nice circular linked list.
/// MAKE SURE THERE IS NO INITIAL TRACK POINT LOADED.
/// 
/// There will always be manual work needed to connect branches at both ends
/// 
/// Also be sure to disable this script once it has ran once to avoid losing all your hard work ;)
/// </summary>
public class CheckPointGenerator : MonoBehaviour {

    public int distanceBetweenPoints = 20;

	// Use this for initialization
	void Start () {
        var allTrackPoints = FindObjectsOfType<TrackPoint>().ToList();

        var listsByPathChoice = new List<TrackPoint>[]
        {
            allTrackPoints.Where(e => e.pathChoice == TrackPoint.PathChoice.PATH_A).OrderBy(e => e.num_in_seq).ToList(),
            allTrackPoints.Where(e => e.pathChoice == TrackPoint.PathChoice.PATH_B).OrderBy(e => e.num_in_seq).ToList(),
            allTrackPoints.Where(e => e.pathChoice == TrackPoint.PathChoice.PATH_C).OrderBy(e => e.num_in_seq).ToList()
        };

        // reset all track points
        foreach (var pathChain in listsByPathChoice)
        {
            foreach(var trackPoint in pathChain)
            {
                trackPoint.isCheckPoint = false;
                trackPoint.nextValidCheckPoints = null;

                // also setting the name here is handy since it lets us know the path it's on and 
                // lets us know this script has ran on it ;)
                trackPoint.name = trackPoint.name + "_" + trackPoint.pathChoice;
            }
        }

        // assign checkpoints
        foreach (var pathChain in listsByPathChoice)
        {
            if (pathChain.Count <= distanceBetweenPoints)
                continue;

            TrackPoint latestCheckPoint = null;
            for(int i = 0; i < pathChain.Count; i += distanceBetweenPoints)
            {
                pathChain[i].isCheckPoint = true;
                if (latestCheckPoint != null)
                {
                    pathChain[i].nextValidCheckPoints = new List<TrackPoint>() { latestCheckPoint };
                }
                latestCheckPoint = pathChain[i];
            }

            pathChain[0].nextValidCheckPoints = new List<TrackPoint>() { latestCheckPoint };
        }
	}
}
