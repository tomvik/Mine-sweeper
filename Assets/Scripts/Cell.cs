using UnityEngine;

public struct Cell
{
    public enum Type
    {
        Empty,
        Mine,
        Number,
    }

    public Type type;

    public enum Status
    {
        Hidden,
        Revealed,
        Flagged,
        Exploded
    }

    public Status status;

    public int number;
    public Vector3Int position;
}
