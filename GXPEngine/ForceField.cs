using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    internal class ForceField : EasyDraw
    {

        public Vector2 position;
        public Vector2 Dimensions;
        public Vector2 appliedForce;
        public float addedFriction;

        public ForceField(Vector2 position, Vector2 Dimensions, Vector2 appliedForce, float addedFriction) : base((int)Dimensions.x + 1, (int)Dimensions.y + 1)
        {
            this.position = position;
            this.Dimensions = Dimensions;
            this.appliedForce = appliedForce;
            this.addedFriction = addedFriction;

            SetOrigin(width / 2, height / 2);
            SetXY(position.x, position.y);

            Fill(0, 0);
            Stroke(80, 230, 255, 255);

            Clear(50, 200, 230, 100);

            Rect(Dimensions.x / 2,Dimensions.y / 2,Dimensions.x, Dimensions.y);
            Line(Dimensions.x / 2, Dimensions.y / 2, Dimensions.x / 2 + appliedForce.x * 500, Dimensions.y / 2 + appliedForce.y * 500);
        }
    }
}
