using Celeste.Mod;
using RecordingStations.Entities;

namespace RecordingStations
{
    /// <summary>
    /// Base class for the mod
    /// </summary>
    public class RecordingStationsModule : EverestModule
    {
        public static RecordingStationsModule Instance;

        public RecordingStationsModule()
        {
            Instance = this;
        }
        
        public override void Load()
        {
            On.Celeste.Player.Update += RecordingStation.PlayerRecord;
        }

        public override void Unload()
        {
            On.Celeste.Player.Update -= RecordingStation.PlayerRecord;
        }
    }
}
