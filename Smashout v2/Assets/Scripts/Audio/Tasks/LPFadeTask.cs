using UnityEngine;

class LPFadeTask : Task
{
    private AudioLowPassFilter filter;
    private float fl, fh, rl, rh, dura, elapsed;
    private Easing.Function ease;

    public LPFadeTask(AudioLowPassFilter lpf, float freqLow, float freqHigh, float resLow, float resHigh, float duration, Easing.Function easingFunc)
    {
        filter = lpf;
        fl = freqLow;
        fh = freqHigh;
        rl = resLow;
        rh = resHigh;
        dura = duration;
        ease = easingFunc;
    }

    protected override void Init()
    {
        filter.lowpassResonanceQ = rl;
        filter.cutoffFrequency = fl;
        elapsed = 0;
    }

    internal override void Update()
    {
        elapsed += Time.deltaTime;
        filter.lowpassResonanceQ = Mathf.Lerp(rl, rh, ease(elapsed / dura));
        filter.cutoffFrequency = Mathf.Lerp(fl, fh, ease(elapsed / dura));
        if (elapsed > dura) SetStatus(TaskStatus.Success);
    }
}