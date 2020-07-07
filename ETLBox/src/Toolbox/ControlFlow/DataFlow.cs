﻿namespace ETLBox.DataFlow
{
    /// <summary>
    /// Contains static information which affects all Dataflow tasks in ETLBox.
    /// Here you can set the threshold value when information about processed records should appear.
    /// </summary>
    public static class DataFlow
    {
        public static int? LoggingThresholdRows { get; set; } = 1000;
        public static int MaxDegreeOfParallelism { get; set; } = -1;
        public static int MaxBufferSize { get; set; } = -1;
        public static bool HasLoggingThresholdRows => LoggingThresholdRows != null && LoggingThresholdRows > 0;
        /// <summary>
        /// Set all settings back to default (which is null or false)
        /// </summary>
        public static void ClearSettings()
        {
            LoggingThresholdRows = 1000;
            MaxDegreeOfParallelism = -1;
            MaxBufferSize = -1;
        }
    }
}
