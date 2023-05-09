using GXPEngine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GXPEngine
{
    internal class Turret : Polygon
    {

        EasyDraw barrel;

        Vector2 pos;
        Vector2 barrelVec;

        float targetingDistance = 200;
        float shellSpeed = 2;

        float cooldown = 100;
        float timer;

        List<Shell> shells;

        public Turret(Vector2 spawnPos) : base(20, 10, spawnPos, false)
        {
            pos = spawnPos;
            barrelVec = new Vector2(-20, 0);

            barrel = new EasyDraw(40, 40);
            barrel.SetOrigin(barrel.width/2, barrel.height/2);
            AddChild(barrel);

            barrel.SetXY(pos.x, pos.y);

            shells = new List<Shell>();
        }

        public void Aim(List<Polygon> targets)
        {

            bool shooting = false;

            List<Polygon> targetable = new List<Polygon>();

            for(int i = 0; i < targets.Count; i++)
            {
                if (targets[i].isActive) 
                {
                    if (pos.DistanceTo(targets[i].pos) < targetingDistance)
                    {
                        targetable.Add(targets[i]);
                        shooting = true;
                    }
                }
            }

            float closestTarget = float.PositiveInfinity;
            int closestI = 0;

            for (int i = 0; i < targetable.Count; i++)
            {
                if (pos.DistanceTo(targetable[i].pos) < closestTarget)
                {
                    closestTarget = pos.DistanceTo(targetable[i].pos);
                    closestI = i;

                    //1.5 added for compensation of acceleration
                    barrelVec.SetAngleRadians(pos.AngleToRadians(targetable[i].pos + (targetable[i].vel / (shellSpeed * shellSpeed)) * 1.5f * pos.DistanceTo(targetable[i].pos)) + Mathf.PI);
                }
            }

            barrel.ClearTransparent();
            barrel.Line(20, 20, 20 + barrelVec.x, 20 + barrelVec.y);

            if (shooting)
            {
                timer += Time.deltaTime;
            }

            if (timer >= cooldown && shooting == true)
            {
                timer -= cooldown;

                Shell shell = new Shell(barrelVec.Normalized() * shellSpeed, pos + barrelVec);
                AddChild(shell);
                shells.Add(shell);
            }

            for(int i = shells.Count - 1; i > -1; i--)
            {
                shells[i].Step(targets);
                if(shells[i].isGone == true)
                {
                    shells.Remove(shells[i]);
                }
            }
        }
    }
}





















////calculate time of impact based on current movement of target and bullet

//Polygon trg = targetable[closestI];

////first calculate distance from the muzzle to the target and other important variables
//Vector2 barrelEnd = pos + barrelVec;
//float dist = pos.DistanceTo(trg.pos);
//float targetSpeed = trg.vel.Length();


////float cosTheta = (barrelEnd - trg.pos).Normalized().Dot(trg.vel);
//float cosTheta = Mathf.Cos((trg.pos - pos).AngleToDegrees(trg.vel));

////now we put all of this into the ABC formula
//float a = ((shellSpeed * shellSpeed) - (targetSpeed * targetSpeed));
//float b = 2 * dist * targetSpeed * cosTheta;
//float c = -(dist * dist);

//float positiveTOI = (-b + Mathf.Sqrt((b * b) - 4 * a * c)) / (2 * a);
//float negativeTOI = (-b - Mathf.Sqrt((b * b) - 4 * a * c)) / (2 * a);

//if(positiveTOI > 0 && positiveTOI < negativeTOI || positiveTOI > 0 && negativeTOI < 0)
//{
//    //barrelVec.SetAngleRadians(pos.AngleToRadians(trg.pos) - Mathf.PI / 2 +
//    //(trg.vel * ((trg.pos - barrelEnd) / positiveTOI)).GetAngleRadians());

//    barrelVec.SetAngleRadians((trg.vel * ((trg.pos - barrelEnd) / positiveTOI)).GetAngleRadians());

//    //barrelVec.RotateDegrees(180);
//}
//else if(negativeTOI > 0)
//{
//    barrelVec.SetAngleRadians(pos.AngleToRadians(trg.pos) - Mathf.PI / 2 +
//    (trg.vel * ((trg.pos - barrelEnd) / negativeTOI)).GetAngleRadians());
//    //barrelVec.RotateDegrees(180);
//}
//else
//{
//    shooting = false;
//}

//Console.WriteLine(positiveTOI);
//Console.WriteLine(negativeTOI);
//Console.WriteLine("");