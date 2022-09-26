using Celeste;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using System.Collections.Generic;
using Celeste.Mod;

namespace RecordingStations.Entities
{
    /// <summary>
    /// A Recording Station the player can interact with.
    /// </summary>
    [CustomEntity("RecordingStations/RecordingStation")]
    public class RecordingStation : Entity
    {
        public enum RecordingMode
        {
            Idle,
            Recording,
            Playback,
        }
        
        public TalkComponent InteractComponent;

        public static RecordingMode CurrentMode { get; private set; } = RecordingMode.Idle;

        private static List<Player.ChaserState> _recordedStates = new List<Player.ChaserState>();
        private static RecordedPlayer _playerPlayback = null;
        
        public RecordingStation(EntityData data, Vector2 offset)
            : base (data.Position + offset)
        {
            Depth = 1000;
            Vector2 drawAt = new Vector2(data.Width / 2.0f, 0.0f);
            Add(InteractComponent = new TalkComponent(
                new Rectangle(-8, 0, 16, 16),
                drawAt,
                Interact));
            
            Image image = new Image(GFX.Game["objects/pico8Console"]);
            image.JustifyOrigin(0.5f, 0.5f);
            Add(image);
        }

        public static void PlayerRecord(On.Celeste.Player.orig_Update orig, Player player)
        {
            if (CurrentMode == RecordingMode.Recording)
            {
                _recordedStates.Add(new Player.ChaserState(player));
            }
            orig(player);
        }

        public void Interact(Player player)
        {
            NextRecordingMode();
        }

        public static void NextRecordingMode()
        {
            CurrentMode++;
            if (CurrentMode > RecordingMode.Playback)
                CurrentMode = RecordingMode.Idle;
            
            Logger.Log("RecStation", "Changed state to " + CurrentMode);

            switch (CurrentMode)
            {
                case RecordingMode.Idle:
                    Cleanup();
                    break;
                
                case RecordingMode.Playback:
                    PlayBack();
                    break;
            }
        }

        private static void Cleanup()
        {
            if (_playerPlayback != null)
                Engine.Scene.Remove(_playerPlayback);
            _recordedStates.Clear();
        }

        private static void PlayBack()
        {
            Logger.Log("RecPlayback", "Playing back " + _recordedStates.Count + " frames");
            Engine.Scene.Add(_playerPlayback = new RecordedPlayer(_recordedStates[0].Position, _recordedStates));
        }
    }
}
