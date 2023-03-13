using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TankIncedent
{
    internal class PlayerSprite : Sprite
    {
        float _maxSpeed = 500f;
        float _acceleration = 7f;
        float _rotSpeed = 6f;
        

        bool _isSpacePressed = false;

        public PlayerSprite(Texture2D spriteTxr, Vector2 spritePos, Vector2 spriteVel, float spriteAngle) : base(spriteTxr, spritePos, spriteVel, spriteAngle)
        {
            _collisionRadius /= 2f;

        }

        public override void Update(GameTime gameTime, List<Sprite> spriteList, Point ScreenSize)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.W))
            {
                _spriteVel += Forward() * _acceleration;
            }
            else
            {
                _spriteVel = Vector2.Zero;
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                _spriteAngle -= _rotSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
                _spriteAngle += _rotSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }





            Vector2 maxVelocity = _spriteVel;
            maxVelocity.Normalize();
            maxVelocity *= _maxSpeed;

            _spriteVel.X = Math.Clamp(_spriteVel.X, Math.Abs(maxVelocity.X) * -1, Math.Abs(maxVelocity.X));
            _spriteVel.Y = Math.Clamp(_spriteVel.Y, Math.Abs(maxVelocity.Y) * -1, Math.Abs(maxVelocity.Y));


            _spritePos += _spriteVel * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (_spritePos.X < 0 - _spriteTxr.Width / 4)
            {
                _spritePos.X = ScreenSize.X + _spriteTxr.Width / 4;
            }
            else if (_spritePos.X > ScreenSize.X + _spriteTxr.Width / 4)
            {
                _spritePos.X = 0 - _spriteTxr.Width / 4;
            }

            if (_spritePos.Y < 0 - _spriteTxr.Height / 4)
            {
                _spritePos.Y = ScreenSize.X + _spriteTxr.Height / 4;
            }
            else if (_spritePos.Y > ScreenSize.X + _spriteTxr.Height / 4)
            {
                _spritePos.Y = 0 - _spriteTxr.Height / 4;
            }
        }

        public Vector2 Forward()
        {
            return new Vector2((float)Math.Cos(_spriteAngle), (float)Math.Sin(_spriteAngle));
        }



    }
}
