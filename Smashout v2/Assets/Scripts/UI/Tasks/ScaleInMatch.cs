using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScaleInMatch : Task {
    private List<GameObject> circles;
    private RectTransform matchCount;
    // Use this for initialization
    protected override void Init()
    {
        matchCount = Services.UIManager.matchCount.GetComponent<RectTransform>();
        matchCount.gameObject.SetActive(true);
    }

    // Update is called once per frame
    internal override void Update()
    {
        if (Services.GameManager.blueTrack == 1)
        {
            matchCount.GetChild(0).GetComponent<Image>().color = new Color(0f, 153.0f, 153.0f);
        }
        else if (Services.GameManager.blueTrack == 2)
        {
            matchCount.GetChild(1).GetComponent<Image>().color = new Color(0f, 153.0f, 153.0f);
        }
        else if (Services.GameManager.blueTrack == 3)
        {
            matchCount.GetChild(2).GetComponent<Image>().color = new Color(0f, 153.0f, 153.0f);
        }

        if (Services.GameManager.greenTrack == 1)
        {
            matchCount.GetChild(4).GetComponent<Image>().color = new Color(0f, 204.0f, 0f);
        }
        else if (Services.GameManager.greenTrack == 2)
        {
            matchCount.GetChild(3).GetComponent<Image>().color = new Color(0f, 204.0f, 0f);
        }
        else if (Services.GameManager.greenTrack == 3)
        {
            matchCount.GetChild(2).GetComponent<Image>().color = new Color(0f, 204.0f, 0f);
        }

        if (Services.GameManager.blueTrack == 0 && Services.GameManager.greenTrack == 0)
        {
            matchCount.GetChild(0).GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f);
            matchCount.GetChild(1).GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f);
            matchCount.GetChild(2).GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f);
            matchCount.GetChild(3).GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f);
            matchCount.GetChild(4).GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f);
        }
    }
}
