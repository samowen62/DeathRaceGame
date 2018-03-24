
using UnityEngine;
using System.Linq;

public class ElectricalEffect : PausableBehaviour {

    private Animator[] childAnimatiors;
    private bool[] activeBolts;
    private int numBolts;

    public float totalTime = 0.8f;

    protected override void _awake () {
        childAnimatiors = gameObject.GetComponentsInChildren<Animator>();
        numBolts = childAnimatiors.Length;
        activeBolts = new bool[numBolts];

        foreach (var animator in childAnimatiors)
        {
            animator.gameObject.SetActive(false);
        }
    }

    public void Activate()
    {
        float timeBetweenCalls = totalTime / numBolts;
        float baseTime = 0f;

        for (int i = 0; i < numBolts; i++)
        {
            childAnimatiors[i].gameObject.SetActive(true);
            activeBolts[i] = true;
            callAfterSeconds(baseTime, () => makeInactive(1));
            baseTime += timeBetweenCalls;
        }
    }

    private void makeInactive(int amountToInactivate)
    {
        int amountActive = activeBolts.Count(b => b);
        amountToInactivate = Mathf.Min(amountToInactivate, amountActive);

        if (amountToInactivate == 0) return;

        // Randomly determine which electric bolts should be on
        var rand = new System.Random();
        var cnt = 0;
        while (cnt < amountToInactivate)
        {
            var ind = rand.Next(0, numBolts);
            if (activeBolts[ind])
            {
                activeBolts[ind] = false;
                cnt++;
            }
        }

        for (int i = 0; i < numBolts; i++)
        {
            childAnimatiors[i].gameObject.SetActive(activeBolts[i]);
        }
    }
}
