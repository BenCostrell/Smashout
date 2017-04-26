using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private Rigidbody2D rb;
    private Camera cameraComp;
    public float highestPlayerOffset;
    [HideInInspector]
    public float baseSize;
    public float cameraSpeed;
    public float sizeChangeSpeed;
    public float minSizeScale;
    public float maxSizeScale;
    public bool viewAdjustEnabled;
    public float onHitZoomSizeFactor;
    public float onHitZoomPositionFactor;
    [Space(10)]
    public float intensityLow;
    public float intensityHigh;
    public float duration;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        cameraComp = GetComponent<Camera>();
        baseSize = cameraComp.orthographicSize;
        viewAdjustEnabled = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (Services.GameManager.gameStarted && viewAdjustEnabled)
        {
            AdjustView();
        }
	}

    void AdjustView()
    {
        if (!Services.GameManager.gameStarted) return;
        float newSize = CalculateAppropriateSize();
        Vector3 newPosition = CalculateAppropriateLocation(newSize);
        cameraComp.orthographicSize = newSize;
        transform.position = Vector3.MoveTowards(transform.position, newPosition, cameraSpeed * Time.deltaTime);

    }

    public float CalculateAppropriateSize()
    {
        float[] heights = new float[Services.GameManager.numPlayers];
        for (int i = 0; i < heights.Length; ++i) heights[i] = Services.GameManager.players[i].transform.position.y;

        float topOfView = Mathf.Max(heights) + highestPlayerOffset;
        //float bottomOfView = Mathf.Min(heights) - highestPlayerOffset;
        float bottomOfView = -150f;
        float targetSize = Mathf.Max(baseSize * minSizeScale, (topOfView + baseSize) / 2, (bottomOfView + baseSize) / 2);
        targetSize = Mathf.Min(targetSize, baseSize * maxSizeScale);
        float sizeDiff = targetSize - cameraComp.orthographicSize;
        float newSize = cameraComp.orthographicSize + (sizeDiff * sizeChangeSpeed);
        return newSize;
    }

    public void SetLight(bool state)
    {
        Light l = GetComponentInChildren<Light>();
        if (!state) l.enabled = false;
        else Services.TaskManager.AddTask(new CamLightTask(l, intensityLow, intensityHigh, duration));
    }

    public Vector3 CalculateAppropriateLocation(float size)
    {
        return new Vector3(0, -baseSize + size, transform.position.z);
    }
}
