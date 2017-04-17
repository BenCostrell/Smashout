﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleInMatch : MonoBehaviour {
    private List<GameObject> circles;
    private RectTransform matchCount;
    private GameObject centerCount;
    private GameObject[] roundCircles;
    // Use this for initialization
    void Start()
    {
        matchCount = Services.UIManager.matchCount.GetComponent<RectTransform>();
        centerCount = matchCount.GetChild(0).gameObject;
        roundCircles = new GameObject[Services.GameManager.matchSet];
        float x = -10f - Services.GameManager.matchSet/2*40;
        for(int i = 0; i < roundCircles.Length; i++)
        {
            roundCircles[i] = Instantiate(centerCount);
            roundCircles[i].transform.SetParent(matchCount.gameObject.transform);
            roundCircles[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(x, 0);
            x += 40f;
            if(i == Services.GameManager.matchSet / 2)
            {
                roundCircles[i].gameObject.GetComponent<RectTransform>().localScale = new Vector3(0.35f, 0.35f, 1.0f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < Services.GameManager.blueTrack; i++)
        {
            roundCircles[i].GetComponent<Image>().color = new Color(0f, 153.0f, 153.0f);
        }
        for(int i = roundCircles.Length-1; i > roundCircles.Length - 1 - Services.GameManager.greenTrack; i--)
        {
            roundCircles[i].GetComponent<Image>().color = new Color(0f, 204.0f, 0f);
        }

        if (Services.GameManager.blueTrack == 0 && Services.GameManager.greenTrack == 0)
        {
            for(int i =0; i < roundCircles.Length; i++)
            {
                roundCircles[i].GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f);
            }
        }
    }
}
