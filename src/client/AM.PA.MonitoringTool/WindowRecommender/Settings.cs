﻿namespace WindowRecommender
{
    internal static class Settings
    {
        internal const string EventTable = "window_recommender_events";

        internal const string EnabledSettingDatabaseKey = "WindowRecommenderEnabled";
        internal const bool EnabledDefault = true;
        internal static bool Enabled = EnabledDefault;

        internal const string NumberOfWindowsSettingDatabaseKey = "WindowRecommenderNumberOfWindows";
        internal const int NumberOfWindowsDefault = 3;
        internal static int NumberOfWindows = NumberOfWindowsDefault;

        internal const string TreatmentModeSettingDatabaseKey = "WindowRecommenderTreatmentMode";
        internal const bool TreatmentModeDefault = false;
        internal static bool TreatmentMode = TreatmentModeDefault;
        
        internal const int OverlayAlpha = 64;
        internal const int FramesPerSecond = 10;

        internal const int DurationIntervalSeconds = 10;
        internal const int DurationTimeframeMinutes = 10;

        internal const int FrequencyIntervalSeconds = 10;
        internal const int FrequencyTimeframeMinutes = 10;
    }
}
