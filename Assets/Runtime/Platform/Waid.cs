using System;

namespace CMIYC.Platform
{
    public readonly struct Waid
    {
        public readonly int x;
        public readonly int y;
        public readonly Cardinal cardinal;

        public Waid(int x, int y, Cardinal cardinal)
        {
            this.x = x;
            this.y = y;
            this.cardinal = cardinal;
        }

        public override bool Equals(object? obj)
        {
            return obj is Waid other && Equals(other);
        }

        private bool Equals(Waid other)
        {
            if (x == other.x && y == other.y && cardinal == other.cardinal)
                return true;

            if (Math.Abs(x - other.x) > 1 || Math.Abs(y - other.y) > 1 || other.cardinal != Adjacant(cardinal))
                return false;

            switch (cardinal)
            {
                case Cardinal.North or Cardinal.South when x != other.x:
                case Cardinal.East or Cardinal.West when y != other.y:
                    return false;
                case Cardinal.North:
                    return y + 1 == other.y;
                case Cardinal.East:
                    return x + 1 == other.x;
                case Cardinal.South:
                    return y - 1 == other.y;
                case Cardinal.West:
                    return x - 1 == other.x;
                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y, (int)cardinal);
        }

        private static Cardinal Adjacant(Cardinal cardinal)
        {
            return cardinal switch
            {
                Cardinal.North => Cardinal.South,
                Cardinal.East => Cardinal.West,
                Cardinal.South => Cardinal.North,
                Cardinal.West => Cardinal.East,
                _ => throw new ArgumentOutOfRangeException(nameof(cardinal), cardinal, null)
            };
        }

        public static bool operator ==(Waid a, Waid b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Waid a, Waid b)
        {
            return !(a == b);
        }
    }
}
