using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private Rigidbody2D rb;
    private Camera cameraComp;
    public float highestPlayerOffset;
    private float baseSize;
    public float cameraSpeed;
    public float minSizeScale;

    // Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        cameraComp = GetComponent<Camera>();
        baseSize = cameraComp.orthographicSize;
	}
	
	// Update is called once per frame
	void Update () {
        if (Services.GameManager.gameStarted)
        {
            AdjustView();
        }
	}

    void AdjustView()
    {
        if (!Services.GameManager.gameStarted) return;
        float[] heights = new float[Services.GameManager.numPlayers];
        for (int i = 0; i < heights.Length; ++i) heights[i] = Services.GameManager.players[i].transform.position.y;

        float topOfView = Mathf.Max(heights) + highestPlayerOffset;
        float bottomOfView = Mathf.Min(heights) - highestPlayerOffset;
        float newSize = Mathf.Max(baseSize * minSizeScale, (topOfView + baseSize) / 2, (bottomOfView + baseSize) / 2);
        Vector3 newPosition = new Vector3(transform.position.x, -baseSize + newSize, transform.position.z);
        cameraComp.orthographicSize = newSize;
        transform.position = Vector3.MoveTowards(transform.position, newPosition, cameraSpeed * Time.deltaTime);

    }
}
