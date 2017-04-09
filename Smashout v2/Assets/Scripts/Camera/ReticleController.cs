using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticleController : MonoBehaviour {

    private Player player;
    private SpriteRenderer sr;
    private GameObject mask;
    public float xOffset;
    public float yOffset;
    private RectTransform rectTransform;
    private float xBound;
    private float yBound;

	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        Reposition();
	}

    public void InitializeReticle(Player pl, RenderTexture renderTexture)
    {
        player = pl;
        sr = player.gameObject.GetComponent<SpriteRenderer>();
        mask = transform.GetChild(0).gameObject;
        mask.SetActive(false);
        rectTransform = GetComponent<RectTransform>();
        mask.GetComponentInChildren<RawImage>().texture = renderTexture;
        mask.transform.GetChild(1).GetComponent<Image>().color = player.color;
        xBound = 800;
        yBound = 450;
        Services.EventManager.Register<GameOver>(OnGameOver);
    }

    void Reposition()
    {
        Vector3 playerPos = Camera.main.WorldToViewportPoint(player.transform.position);
        Vector3 playerTopRight = Camera.main.WorldToViewportPoint(sr.bounds.max);
        Vector3 playerBottomLeft = Camera.main.WorldToViewportPoint(sr.bounds.min);
        Debug.Log("player " + player.playerNum + " at screen pos " + playerPos.x + ", " + playerPos.y);
        float xPos = Mathf.Clamp(playerPos.x, xOffset, 1 - xOffset) * xBound*2 - xBound;
        float yPos = Mathf.Clamp(playerPos.y, yOffset, 1 - yOffset) * yBound*2 - yBound;
        rectTransform.anchoredPosition = new Vector2(xPos, yPos);

        if (playerBottomLeft.x > 1 || playerTopRight.x < 0 || playerBottomLeft.y > 1 || playerTopRight.y < 0)
        {
            mask.SetActive(true);
        }
        else
        {
            mask.SetActive(false);
        }
    }

    void OnGameOver(GameOver e)
    {
        Services.EventManager.Unregister<GameOver>(OnGameOver);
        gameObject.SetActive(false);
    }
}
