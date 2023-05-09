using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GXPEngine;
using GXPEngine.Core;

internal class UnitTest
{

    //press F11 whilst the program is running to execute unit tests in console

    public static bool Approximate(float input, float target, float factor)
    {
        return (input < target + factor && input > target - factor);
    }

    public static bool Approximate(Vector2 input, Vector2 target, float factor)
    {
        return (input.x < target.x + factor && input.x > target.x - factor && input.y < target.y + factor && input.y > target.y - factor);
    }

    public static void DoTests()
    {
        float factor = 0.0001f;

        Vector2 u = new Vector2(13, 0);
        Vector2 v = u.Normalize();
        Console.WriteLine("{0} normalized = {1} (should be (1,0))", u, v);
        Console.WriteLine("Normalize correct? {0}", Approximate(u, new Vector2(1, 0), factor));

        Vector2 w = new Vector2(2, 1);
        Vector2 x = new Vector2(1, 2);
        float y = 4;
        Console.WriteLine("{0} + {1} = {2} (should be (3,3))", w, x, w + x);
        Console.WriteLine("addition correct? {0}", Approximate(w + x, new Vector2(3, 3), factor));

        Console.WriteLine("{0} - {1} = {2} (should be (1,-1))", w, x, w - x);
        Console.WriteLine("subtraction correct? {0}", Approximate(w - x, new Vector2(1, -1), factor));

        Console.WriteLine("{0} * {1} = {2} (should be (2,2))", w, x, w * x);
        Console.WriteLine("multiply between vectors correct? {0}", Approximate(w * x, new Vector2(2, 2), factor));

        Console.WriteLine("{0} * {1} = {2} (should be (8,4))", w, y, w * y);
        Console.WriteLine("multiply witn a float correct? {0}", Approximate(w * y, new Vector2(8, 4), factor));

        Console.WriteLine("{0} / {1} = {2} (should be (0.5,0.25))", w, y, w / y);
        Console.WriteLine("division correct? {0}", Approximate(w / y, new Vector2(0.5f, 0.25f), factor));

        Console.WriteLine("length of {0} = {2} (should be (2.2360...))", w, y, w.Length());
        Console.WriteLine("Length correct? {0}", Approximate(w.Length(), 2.2360f, factor));

        Console.WriteLine("distance between {0} and {1} = {2} (1.4142...)", w, x, w.DistanceTo(x));
        Console.WriteLine("Distance correct? {0}", Approximate(w.DistanceTo(x), 1.4142f, factor));

        float z = 45;
        float a = 1.5f * Mathf.PI;
        float b = x.Length();
        float c = 90;
        float d = Mathf.PI;
        Vector2 e = new Vector2(3, 1);
        Vector2 f = new Vector2(1, 1);
        Vector2 g = new Vector2(4, 3);
        Vector2 h = new Vector2(1, 1);
        Vector2 i = new Vector2(-1, 0);

        Console.WriteLine("{0} degrees = {1} radians (should be 3.141...)", 180, Vector2.Deg2Rad(180));
        Console.WriteLine("Degrees to radians correct? {0}", Approximate(Vector2.Deg2Rad(180), Mathf.PI, factor));

        Console.WriteLine("{0} radians = {1} degrees (should be 45...)", 0.785, Vector2.Rad2Deg(Mathf.PI * 0.25f));
        Console.WriteLine("Radians to degrees correct? {0}", Approximate(Vector2.Rad2Deg(Mathf.PI * 0.25f), 45, factor));

        Console.WriteLine("Unit vector of 45 degrees = {0} (should be (0.7071..., 0.7071...))", Vector2.GetUnitVectorDeg(45));
        Console.WriteLine("Unit vector from degrees correct? {0}", Approximate(Vector2.GetUnitVectorDeg(45), new Vector2(0.7071f, 0.7071f), factor));

        Console.WriteLine("Unit vector of 2 PI radians = {0} (should be (1,0))", Vector2.GetUnitVectorRad(2f * Mathf.PI));
        Console.WriteLine("Unit vector from radians correct? {0}", Approximate(Vector2.GetUnitVectorRad(2f * Mathf.PI), new Vector2(1, 0), factor));

        Vector2 randomUnitVector = Vector2.RandomUnitVector();
        Console.WriteLine("random unit vector = {0}, length = {1}, (should be 1)", randomUnitVector, randomUnitVector.Length());
        Console.WriteLine("random unit vector correct? {0}", Approximate(randomUnitVector.Length(), 1, factor));

        Console.WriteLine("Set angle of {0} to {1} = {2} (should be (1.581..., 1.581...)", x, z, x.SetAngleDegrees(z));
        Console.WriteLine("Angle set correct? {0}", Approximate(x.SetAngleDegrees(z), new Vector2(1.5811f, 1.5811f), factor));

        Console.WriteLine("Angle of {0} = {1} (should be 45)", x, x.GetAngleDegrees());
        Console.WriteLine("Angle get correct? {0}", Approximate(x.GetAngleDegrees(), 45, factor));

        Console.WriteLine("Set radians of {0} to {1} = {2} (should be (0, {3})", x, a, x.SetAngleRadians(a), -b);
        Console.WriteLine("Radians set correct? {0}", Approximate(x.SetAngleRadians(a), new Vector2(0, -b), factor));

        Console.WriteLine("Radians of {0} = {1} (should be {2})", x, x.GetAngleRadians(), -a + Mathf.PI);
        Console.WriteLine("Radians get correct? {0}", Approximate(x.GetAngleRadians(), -a + Mathf.PI, factor));

        Vector2 outcome = e.RotateDegrees(c);
        Console.WriteLine("Rotating {0} by {1} degrees results in {2} (should be (-1, 3))", e, c, outcome);
        Console.WriteLine("Rotate degrees correct? {0}", Approximate(outcome, new Vector2(-1, 3), factor));

        outcome = e.RotateRadians(d);
        Console.WriteLine("Rotating {0} by {1} radians results in {2} (should be (1, -3))", e, d, outcome);
        Console.WriteLine("Rotate radians correct? {0}", Approximate(outcome, new Vector2(1, -3), factor));

        outcome = e.RotateAroundDegrees(f, c);
        Console.WriteLine("Rotating {0} by {1} degrees around {2} results in {3} (should be (5, 1))", e, c, f, outcome);
        Console.WriteLine("Rotate around point degrees correct? {0}", Approximate(outcome, new Vector2(5, 1), factor));

        outcome = e.RotateAroundRadians(f, d);
        Console.WriteLine("Rotating {0} by {1} radians around {2} results in {3} (should be (-3, 1))", e, d, f, outcome);
        Console.WriteLine("Rotate around point radians correct? {0}", Approximate(outcome, new Vector2(-3, 1), factor));

        Console.WriteLine("The normal of {0} results in {1} (should be (-1, 2))", w, w.Normal());
        Console.WriteLine("Rotate around point radians correct? {0}", Approximate(w.Normal(), new Vector2(-1, 2), factor));

        Console.WriteLine("The unit normal of {0} results in {1} (should be (-0.6, 0.8))", g, g.UnitNormal());
        Console.WriteLine("Rotate around point radians correct? {0}", Approximate(g.UnitNormal(), new Vector2(-0.6f, 0.8f), factor));

        Console.WriteLine("The dot product of {0} and {1} results in {2} (should be -1)", h, i, h.Dot(i));
        Console.WriteLine("Dot product correct? {0}", Approximate(h.Dot(i), -1, factor));
    }
}

