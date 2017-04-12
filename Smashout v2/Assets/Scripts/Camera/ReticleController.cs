using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReticleController : MonoBehaviour {

    private Player player;
    private SpriteRenderer sr;
    private GameObject reticleImage;
    private GameObject borderObj;
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
        reticleImage = transform.GetChild(0).gameObject;
        reticleImage.SetActive(false);
        rectTransform = GetComponent<RectTransform>();
        borderObj = reticleImage.transform.GetChild(1).gameObject;
        borderObj.GetComponent<Image>().color = player.color;
        reticleImage.GetComponentInChildren<RawImage>().texture = renderTexture;
        xBound = 800;
        yBound = xBound/Camera.main.aspect;
        Services.EventManager.Register<GameOver>(OnGameOver);
    }

    void Reposition()
    {
        Vector3 playerPos = Camera.main.WorldToViewportPoint(player.transform.position);
        Vector3 playerTopRight = Camera.main.WorldToViewportPoint(sr.bounds.max);
        Vector3 playerBottomLeft = Camera.main.WorldToViewportPoint(sr.bounds.min);
        float xPos = Mathf.Clamp(playerPos.x, xOffset, 1 - xOffset) * xBound*2 - xBound;
        float yPos = Mathf.Clamp(playerPos.y, yOffset, 1 - yOffset) * yBound*2 - yBound;
        rectTransform.anchoredPosition = new Vector2(xPos, yPos);

        reticleImage.SetActive(true);
        if (playerBottomLeft.x > 1) {
            borderObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        } else if (playerTopRight.x < 0) {
            borderObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
        else if (playerBottomLeft.y > 1) {
            borderObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }
        else if (playerTopRight.y < 0) {
            borderObj.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
        }
        else
        {
            reticleImage.SetActive(false);
        }
    }

    void OnGameOver(GameOver e)
    {
        Services.EventManager.Unregister<GameOver>(OnGameOver);
        gameObject.SetActive(false);
    }
}
