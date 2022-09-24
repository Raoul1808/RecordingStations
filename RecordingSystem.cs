using Celeste.Mod;

namespace RecordingStations
{
    /// <summary>
    /// This is the base class for the recording and playback system.
    /// </summary>
    public static class RecordingSystem
    {
        public enum RecordingMode
        {
            Idle,
            Recording,
            Playback,
        }

        public static RecordingMode CurrentMode { get; private set; } = RecordingMode.Idle;

        public static void AdvanceNext()
        {
            CurrentMode++;
            if (CurrentMode > RecordingMode.Playback)
                CurrentMode = RecordingMode.Idle;
            
            Logger.Log("RecStation", "Changing mode to " + CurrentMode);
        }
    }
}
