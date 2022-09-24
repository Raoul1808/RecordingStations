module RecordingStations

using ..Ahorn, Maple

@mapdef Entity "RecordingStations/RecordingStation" RecordingStation(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Recording Station (Recording Stations)" => Ahorn.EntityPlacement(
        RecordingStation
    )
)

function Ahorn.selection(entity::RecordingStation)
   x, y = Ahorn.position(entity)
   width = 32
   height = 16

   return Ahorn.Rectangle(x - 16, y, width, height)
end

# TODO: change sprite
sprite = "objects/pico8Console"
Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::RecordingStation, room::Maple.Room) = Ahorn.drawSprite(ctx, sprite, 0, 0)

end