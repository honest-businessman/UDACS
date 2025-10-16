using UnityEngine;
public enum DayLength { None, TenMinutes, TwentyMinutes, Overdrive };
public class DayNight : MonoBehaviour
{
    public DayLength dayLength;
    private float Time;
    int tick;
    void Start()
    {
        gameObject.transform.eulerAngles = new Vector3(Time * 15 - 90, 10, 0);
        tick = 0;
        Time = 6;
    }
    void FixedUpdate()
    {
        tick++;
        if (tick >= 2)
        {
            if (dayLength == DayLength.TenMinutes)
            {
                Time += 0.0016f;
            }
            else if (dayLength == DayLength.TwentyMinutes)
            {
                Time += 0.0008f;
            }
            else if (dayLength == DayLength.Overdrive)
            {
                Time += 0.008f;
            }
            gameObject.transform.eulerAngles = new Vector3(Time * 15, 10, 0);
            tick = 0;
        }
    }
}