public static class TimeLogic
{
    public const float StartHour = 6f;
    public const float EndHour = 22f;
    public const float DayDuration = EndHour - StartHour;

    public static float GetHourFromProgress(float dayProgress) =>
        StartHour + dayProgress * DayDuration;

    public static bool IsNight(float hour) =>
        hour >= EndHour || hour < StartHour;

    public static string GetTimeString(float hour)
    {
        int h = (int)hour;
        int m = (int)((hour - h) * 60f);
        return $"{h:D2}:{m:D2}";
    }
}
