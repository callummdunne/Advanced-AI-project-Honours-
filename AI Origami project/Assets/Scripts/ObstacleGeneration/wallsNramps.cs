﻿using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;

/// <summary>
/// place ramp in front of player when left mouse is clicked
/// </summary>

public class wallsNramps : MonoBehaviour
{

    //vars
    public GameObject ramp;
    public GameObject wall;

    private Transform myTransform;
    private Transform cameraHeadTransform;

    private float placeRate = 1;
    private float nextPlace = 0;

    //ramp position
    private Vector3 rampPos = new Vector3();
    private Vector3 wallPos = new Vector3();
    private ArrayList selfSet = new ArrayList();
    private Dictionary<System.String, System.Int32> detectors = new Dictionary<System.String, System.Int32>();
    private int detectorRange = 10;

    // Use this for initialization
    void Start()
    {
        myTransform = transform;
        cameraHeadTransform = myTransform.Find("PlayerCamera");
        ReadString();
        getDetectors();
        Debug.Log(selfSet.Count);
        foreach (string detector in selfSet)
        {
            Debug.Log(detector.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("placeRamp") && Time.time > nextPlace)
        {
            nextPlace = Time.time + placeRate;
            rampPos = cameraHeadTransform.TransformPoint(0, 0, 10);
            rampPos.y = 2;
            Instantiate(ramp, rampPos, Quaternion.Euler(270, 270, 0));
            ramp.transform.localScale = new Vector3(200f, 200f, 200f);
        }
        if (Input.GetButton("placeWall") && Time.time > nextPlace)
        {
            nextPlace = Time.time + placeRate;
            wallPos = cameraHeadTransform.TransformPoint(0, 0, 10);
            wallPos.y = 1;
            Instantiate(wall, wallPos, Quaternion.Euler(270, 0, 0));
            wall.transform.localScale = new Vector3(100f, 50f, 100f);
        }
    }

    void getDetectors()
    {
        bool stillMerging = true;
        int current = 0;

        while (stillMerging)
        {
            //find closest pair for current detector
            int minDistance = 100;
            int bestMatch = -1;
            string tempDetector = "";
            for (int next = current + 1; next < selfSet.Count; next++)
            {
                string d1 = (string)selfSet[current];
                string d2 = (string)selfSet[next];
                int tempDist = calcDistance(d1, d2);
                if ((tempDist < minDistance) && (tempDist <= detectorRange))
                {
                    bestMatch = next;
                    minDistance = tempDist;
                    tempDetector = mergeDetectors(d1, d2);
                }
                
            }
            if (bestMatch == -1)
            {
                current += 1;
                //check if this was last detector
                if (current >= selfSet.Count - 1)
                {
                    stillMerging = false;
                }
            }
            else
            {
                //add new detector in current's spot
                selfSet[current] = tempDetector;
                //remove merged detector
                selfSet.RemoveAt(bestMatch);
            }

        }
    }
    //read from file containing self set
    void ReadString()
    {
        string path = "Assets/Scripts/Data/selfSet.txt";

        //Read the text from directly from the test.txt file
        StreamReader sr = new StreamReader(path);
        string line;
        do
        {
            line = sr.ReadLine();
            //randomly read with 25% chance of saving string as known self
            int chance = UnityEngine.Random.Range(0, 5);
            if (chance == 1)
            {
                selfSet.Add(line);
                detectors.Add(line, 1);
            }
        } while (!sr.EndOfStream);
        Debug.Log(selfSet.Count);
    }

    int calcDistance(string s1, string s2)
    {
        int distance = 0;
        //the first char has increased weight
        char char1 = s1[0];
        char char2 = s2[0];
        if (char1 != char2)
        {
            distance += 5;
        }

        //get dist from left substring
        try
        {
            int left1 = int.Parse(s1.Substring(1, 2));
            int left2 = int.Parse(s2.Substring(1, 2));
            int result = left1-left2;
            distance += Math.Abs(result);
            //get first last two integers and calc distance
            int xSize1 = int.Parse(s1.Substring(3,1));
            int xSize2 = int.Parse(s2.Substring(3,1));
            int ySize1 = int.Parse(s1.Substring(4,1));
            int ySize2 = int.Parse(s2.Substring(4,1));
            distance += Math.Abs(xSize1 - xSize2);
            distance += Math.Abs(ySize1 - ySize2);
        }
        catch (Exception e)
        {
            Debug.Log("NaN");
        }

        return distance;
    }

    string mergeDetectors(string d1, string d2)
    {
        //if first char not the same, chose first one
        string returnString = "";
        string type = d1.Substring(0,1);
        returnString += type;
        int val1 = int.Parse(d1.Substring(1,1));
        int val2 = int.Parse(d2.Substring(1, 1));
        double tempAve = (val1+val2) / 2;
        int ave = (int)tempAve;
        //getting second val
        val1 = int.Parse(d1.Substring(2, 1));
        val2 = int.Parse(d2.Substring(2, 1));
        tempAve = (val1 + val2) / 2;
        int ave2 = (int)tempAve;
        //add to return string
        string strVal = ave.ToString();
        returnString += strVal;
        strVal = ave2.ToString();
        returnString += strVal;
        //get last two ints
        val1 = int.Parse(d1[3].ToString());
        val2 = int.Parse(d2[3].ToString());
        tempAve = (val1 + val2) / 2;
        ave = (int)tempAve;
        //getting third val
        val1 = int.Parse(d1[4].ToString());
        val2 = int.Parse(d2[4].ToString());
        tempAve = (val1 + val2) / 2;
        ave2 = (int)tempAve;
        //add to return string
        returnString += ave.ToString();
        returnString += ave2.ToString();
        return returnString;
    }
}