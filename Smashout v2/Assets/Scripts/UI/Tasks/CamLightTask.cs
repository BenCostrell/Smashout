using UnityEngine;

class CamLightTask : Task
{
    private Light light;
    private float intensityLow, intensityHigh, duration, timeElapsed;

    public CamLightTask(Light light, float intensityLow, float intensityHigh, float duration)
    {
        this.light = light;
        this.intensityHigh = intensityHigh;
        this.intensityLow = intensityLow;
        this.duration = duration;
    }

    protected override void Init()
    {
        light.intensity = intensityLow;
        light.enabled = true;
        timeElapsed = 0;
    }

    internal override void Update()
    {
        light.intensity = Mathf.Lerp(intensityLow, intensityHigh, Easing.QuadEaseIn(timeElapsed / duration));
        timeElapsed += Time.deltaTime;
        if (timeElapsed > duration) SetStatus(TaskStatus.Success);
    }

    protected override void OnSuccess()
    {
    }
}