using Celeste.Mod;

namespace RecordingStations
{
    public class RecordingStationsModule : EverestModule
    {
        public static RecordingStationsModule Instance;

        public RecordingStationsModule()
        {
            Instance = this;
        }
        
        public override void Load()
        {
            Logger.Log("RecStation", "Hello from RecordingStationsModule!");
        }

        public override void Unload()
        {
            Logger.Log("RecStation", "Goodbye from RecordingStationsModule!");
        }
    }
}
