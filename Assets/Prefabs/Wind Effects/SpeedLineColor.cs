using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLineColor : MonoBehaviour
{
    public enum ColorF
    {
        white,
        green,
        blue,
        teal,
        cyan
    }
    public ParticleSystemRenderer Slot;
    public ColorF Col;
    public float durationTime = 2.0f;
    private float duration;
    public Material SlWhite;
    public Material SlGreen;
    public Material SlBlue;
    public Material SlTeal;
    public Material SlCyan;

    // Start is called before the first frame update
    void Start()
    {
        Slot = GetComponent<ParticleSystemRenderer>();
        Col = ColorF.white;
        duration = durationTime;
    }

    // Update is called once per frame
    void Update()
    {
        ColorSwap();
    }
    private void ColorSwap()
    {
        switch (Col)
        {
            case ColorF.white:
                duration -= Time.deltaTime;
                Slot.trailMaterial = SlWhite;
                SwapCheck();
                break;
            case ColorF.green:
                duration -= Time.deltaTime;
                Slot.trailMaterial = SlGreen;
                SwapCheck();
                break;
            case ColorF.blue:
                duration -= Time.deltaTime;
                Slot.trailMaterial = SlBlue;
                SwapCheck();
                break;
            case ColorF.teal:
                duration -= Time.deltaTime;
                Slot.trailMaterial = SlTeal;
                SwapCheck();
                break;
            case ColorF.cyan:
                duration -= Time.deltaTime;
                Slot.trailMaterial = SlCyan;
                SwapCheck();
                break;
            default:
                break;
        }
    }
    private void SwapCheck()
    {
        if (duration < 0)
        {
            duration = durationTime;
            Col = (ColorF)Random.Range(0, 4);
        }
    }
}
