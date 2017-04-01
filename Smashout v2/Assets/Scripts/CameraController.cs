using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private Rigidbody2D rb;
    private Camera camera;
    public float highestPlayerOffset;
    private float baseSize;
    public float cameraSpeed;
    public float minSizeScale;

    // Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        camera = GetComponent<Camera>();
        baseSize = camera.orthographicSize;
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
        float height_P1 = Services.GameManager.players[0].transform.position.y;
        float height_P2 = Services.GameManager.players[1].transform.position.y;

        float topOfView = Mathf.Max(height_P1, height_P2) + highestPlayerOffset;
        float newSize = Mathf.Max(baseSize * minSizeScale, (topOfView + baseSize) / 2);
        Vector3 newPosition = new Vector3(transform.position.x, -baseSize + newSize, transform.position.z);
        camera.orthographicSize = newSize;
        transform.position = Vector3.MoveTowards(transform.position, newPosition, cameraSpeed * Time.deltaTime);

    }
}
