using Celeste;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace RecordingStations.Entities
{
    /// <summary>
    /// A Recording Station the player can interact with.
    /// </summary>
    [CustomEntity("RecordingStations/RecordingStation")]
    public class RecordingStation : Entity
    {
        public TalkComponent InteractComponent;
        
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

        public void Interact(Player player)
        {
            RecordingSystem.AdvanceNext();
        }
    }
}
