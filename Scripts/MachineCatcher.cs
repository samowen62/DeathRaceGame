﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineCatcher : PausableBehaviour {

    private GameObject catcher;
    private GameObject innerRing;
    private GameObject outerRing;
    private float topPos = 35f;
    private float bottomPos = 8.5f;
    private float moveSpeed = 100;
    private bool descending = false;
    private bool leaving = false;


    // Use this for initialization
    protected override void _awake () {
        catcher = transform.Find("Catcher").gameObject;
        innerRing = transform.Find("InnerRing").gameObject;
        outerRing = transform.Find("OuterRing").gameObject;
    }

    // Update is called once per frame
    protected override void _update()
    {
        if (descending)
        {
            float newPos = transform.localPosition.y - moveSpeed * Time.deltaTime;
            if(newPos <= bottomPos)
            {
                newPos = bottomPos;
                descending = false;
            }
            transform.localPosition = new Vector3(0, newPos, 0);
        }
        else if (leaving)
        {
            float newPos = transform.localPosition.y + moveSpeed * Time.deltaTime;
            if (newPos >= topPos)
            {
                newPos = topPos;
                leaving = false;
                setVisible(false);
            }
            transform.localPosition = new Vector3(0, newPos, 0);
            
        }
        catcher.transform.RotateAround(catcher.transform.position, catcher.transform.forward, 18 * Time.deltaTime);
        innerRing.transform.RotateAround(innerRing.transform.position, innerRing.transform.forward, -160 * Time.deltaTime);
        outerRing.transform.RotateAround(outerRing.transform.position, outerRing.transform.forward, 160 * Time.deltaTime);
    }

    public void Enter()
    {
        descending = true;
        setVisible(true);
    }

    private void setVisible(bool visible)
    {
        catcher.gameObject.SetActive(visible);
        innerRing.gameObject.SetActive(visible);
        outerRing.gameObject.SetActive(visible);
    }

    public void Leave()
    {
        leaving = true;
    }

    public void SetInitialPosition()
    {
        var rotation = new Quaternion();
        rotation.Set(0, 0, 0, 1);
        transform.localRotation = rotation;
        transform.localScale = new Vector3(4, 4, 4);
        transform.localPosition = new Vector3(0, topPos, 0);

        // because unity is too dumb to just call _awake on initialization
        if (catcher == null)
        {
            catcher = transform.Find("Catcher").gameObject;
            innerRing = transform.Find("InnerRing").gameObject;
            outerRing = transform.Find("OuterRing").gameObject;
        }

        setVisible(false);
    }
}
