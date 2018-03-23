
using UnityEngine;

public class ElectricalEffect : PausableBehaviour {

    private Animator[] childAnimatiors;

    protected override void _awake () {
        childAnimatiors = gameObject.GetComponentsInChildren<Animator>();
        foreach(var animator in childAnimatiors)
        {
            animator.gameObject.SetActive(false);
        }
    }

    public void Activate()
    {
        makeActive(childAnimatiors.Length);
        callAfterSeconds(0.7f, () => makeActive(6));
        callAfterSeconds(0.9f, () => makeActive(4));
        callAfterSeconds(1.2f, () => makeActive(2));
        callAfterSeconds(1.5f, () => makeActive(0));
    }

    private void makeActive(int amountActive)
    {
        //TODO: do check up here

        // Randomly determine which electric bolts should be on
        var rand = new System.Random();
        var length = childAnimatiors.Length;
        var cnt = 0;
        var list = new bool[length];
        while (cnt < amountActive)
        {
            var ind = rand.Next(0, length);
            if (list[ind])
            {
                list[ind] = true;
                cnt++;
            }
        }

        for (int i = 0; i < length; i++)
        {
            childAnimatiors[i].gameObject.SetActive(list[i]);
        }
    }
}
