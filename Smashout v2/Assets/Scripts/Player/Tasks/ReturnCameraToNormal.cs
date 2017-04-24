using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ReturnCameraToNormal : Task
{
    private float timeElapsed;
    private float duration;
    private Vector3 targetLocation;
    private Vector3 initialCameraLocation;
    private float initalCameraSize;
    private float targetSize;

    public ReturnCameraToNormal(float dur)
    {
        duration = dur;
    }

    protected override void Init()
    {
        timeElapsed = 0;
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        initalCameraSize = Camera.main.orthographicSize;
        initialCameraLocation = Camera.main.transform.position;
        targetSize = cameraController.CalculateAppropriateSize();
        targetLocation = cameraController.CalculateAppropriateLocation(targetSize);
        cameraController.viewAdjustEnabled = false;

        Services.EventManager.Register<BumpHit>(OnBumpHit);
        Services.EventManager.Register<GameOver>(OnGameOver);
    }

    internal override void Update()
    {
        timeElapsed += Time.deltaTime;

        Camera.main.orthographicSize = Mathf.Lerp(initalCameraSize, targetSize, Easing.QuadEaseOut(timeElapsed / duration));
        Camera.main.transform.position = Vector3.Lerp(initialCameraLocation, targetLocation, Easing.QuadEaseOut(timeElapsed / duration));

        if (timeElapsed >= duration)
        {
            SetStatus(TaskStatus.Success);
        }
    }

    void OnBumpHit(BumpHit e)
    {
        SetStatus(TaskStatus.Aborted);
    }

    void OnGameOver(GameOver e)
    {
        SetStatus(TaskStatus.Aborted);
    }

    protected override void CleanUp()
    {
        Camera.main.GetComponent<CameraController>().viewAdjustEnabled = true;
        Services.EventManager.Unregister<BumpHit>(OnBumpHit);
        Services.EventManager.Unregister<GameOver>(OnGameOver);
    }
}
