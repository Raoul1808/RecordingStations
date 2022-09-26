using System;
using Celeste;
using Monocle;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace RecordingStations.Entities
{
    public class RecordedPlayer : Actor
    {
        public bool IsDone;
        public List<Player.ChaserState> Timeline;

        private PlayerSprite _playerSprite;
        private PlayerHair _playerHair;
        
        private int _frameIndex;
        private float _currentTime;
        private float _endTime;
        
        private int FrameCount => Timeline.Count;

        public RecordedPlayer(Vector2 position, List<Player.ChaserState> timeline) : base(position)
        {
            Depth = 9008;
            Timeline = timeline;
            Position = position;
            _frameIndex = 0;
            _currentTime = timeline[0].TimeStamp;
            _endTime = timeline[FrameCount - 1].TimeStamp;
            _playerSprite = new PlayerSprite(PlayerSpriteMode.Playback);
            Add(_playerHair = new PlayerHair(_playerSprite));
            Add(_playerSprite);
            for (int index = 0; index < 10; ++index)
                _playerHair.AfterUpdate();
            Collider = new Hitbox(8f, 11f, -4f, -11f);
            Add(new PlayerCollider(OnBouncePlayer));
            
            PlaySound("appear");
        }

        private void OnBouncePlayer(Player player)
        {
            if (player.StateMachine.State == Player.StNormal && player.Speed.Y > 0f && player.Bottom <= Top + 3f)
            {
                Dust.Burst(player.BottomCenter, -1.57079637f, 8);
                ((Level) Scene).DirectionalShake(Vector2.UnitY, 0.05f);
                Input.Rumble(RumbleStrength.Light, RumbleLength.Medium);
                player.Bounce(Top + 2f);
                player.Play("event:/game/general/thing_booped");
            }
        }

        public override void Removed(Scene scene)
        {
            PlaySound("disappear");
            base.Removed(scene);
        }

        public override void Update()
        {
            // Note: Most (if not all) the code present in this function
            //       was taken from the PlayerPlayback class.
            //       I tried inheriting from this class directly, without any success,
            //       thus completely remaking it in the mod.
            
            if (Scene == null)
                return;
            
            _currentTime += Engine.DeltaTime;
            base.Update();
            if (_frameIndex >= FrameCount - 1 || _currentTime >= _endTime)
            {
                RemoveSelf();
            }
            
            var currentState = Timeline[_frameIndex];
            bool wasOnGround = CollideCheck<Solid>(Position - Vector2.UnitY);
            Position = currentState.Position;
            _playerSprite.Scale = currentState.Scale;
            string previousAnimationId = _playerSprite.CurrentAnimationID;
            if (currentState.Animation != _playerSprite.CurrentAnimationID && currentState.Animation != null && _playerSprite.Has(currentState.Animation))
                _playerSprite.Play(currentState.Animation, true);
            string currentAnimationId = _playerSprite.CurrentAnimationID;
            if (_playerSprite.Scale.X != 0.0f)
                _playerHair.Facing = (Facings) Math.Sign(_playerSprite.Scale.X);
            _playerSprite.Color = _playerHair.Color = currentState.HairColor;
            
            if (!wasOnGround && CollideCheck<Solid>(Position - Vector2.UnitY))
                PlaySound("land");

            if (previousAnimationId != currentAnimationId)
            {
                // Handle sounds
                int frame = _playerSprite.CurrentAnimationFrame;
                switch (currentAnimationId)
                {
                    // Jump
                    case "jumpFast":
                    case "jumpSlow":
                        PlaySound("jump");
                        break;
                    
                    // Dreamblock
                    case "dreamDashIn":
                        PlaySound("dreamblock_sequence");
                        break;
                    
                    // Dash
                    // TODO: second dash?
                    case "dash":
                        PlaySound(currentState.Scale.X > 0.0f ? "dash_red_right" : "dash_red_left");
                        break;
                    
                    // Climb
                    case "climbUp":
                    case "climbDown":
                    case "wallslide":
                        PlaySound("grab");
                        break;
                    
                    // Normal walk cases
                    case "runSlow_carry":
                    case "runFast":
                    case "runSlow":
                    case "walk":
                    case "runWind": 
                    case "carryTheoWalk":
                        if (frame == 0 || frame == 6)
                            PlaySound("footstep");
                        break;
                    
                    // Special walk cases
                    case "flip" when frame == 4:
                    case "runStumble" when frame == 6:
                    case "push" when frame == 8 || frame == 15:
                    case "idleC" when _playerSprite.Mode == PlayerSpriteMode.MadelineNoBackpack &&
                            (frame == 3 || frame == 6 || frame == 8 || frame == 11):
                        PlaySound("footstep");
                        break;
                }
            }
            
            while (_frameIndex < FrameCount - 1 && _currentTime >= Timeline[_frameIndex + 1].TimeStamp)
                _frameIndex++;

            if (!Scene.OnInterval(0.1f))
                return;
            TrailManager.Add(Position, _playerSprite, _playerHair, _playerSprite.Scale, _playerHair.Color, Depth + 1);
        }

        private void PlaySound(string id)
        {
            Audio.Play("event:/new_content/char/tutorial_ghost/" + id, Position);
        }
    }
}
