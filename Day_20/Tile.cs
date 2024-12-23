// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace Day_20;

public struct Tile : IEquatable<Tile>
{
    public int X;
    public int Y;
    public bool HasWall;
   
    public bool Equals(Tile other)
    {
        return X == other.X && Y == other.Y;
    }
    public override bool Equals(object? obj)
    {
        return obj is Tile other && Equals(other);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
    public static bool operator ==(Tile left, Tile right)
    {
        return left.Equals(right);
    }
    public static bool operator !=(Tile left, Tile right)
    {
        return !left.Equals(right);
    }
}