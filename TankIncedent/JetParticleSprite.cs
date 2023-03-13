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
    internal class JetParticleSprite : ParticleSprite
    {
        public JetParticleSprite(Texture2D spriteTxr,
           Vector2 spritePos,
           Vector2 spriteVel) : base(spriteTxr, spritePos, spriteVel, 0f)
        {
            _spriteTint = Color.LightBlue;
            _shrinkSpeed = 0.9f;
            _spriteScale = 1f;
        }
    }
}
