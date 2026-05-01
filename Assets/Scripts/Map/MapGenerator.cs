using System.Collections.Generic;
using UnityEngine;

public class MapGenerator
{
    private readonly int _totalRooms;
    private readonly int _maxBranch;

    public MapGenerator(int totalRooms = 10, int maxBranch = 4)
    {
        _totalRooms = totalRooms;
        _maxBranch = maxBranch;
    }

    public List<RoomNode> Generate()
    {
        var rooms = new Dictionary<Vector2Int, RoomNode>();
        var queue = new Queue<Vector2Int>();

        var start = Vector2Int.zero;
        rooms[start] = new RoomNode(start, RoomType.Start);
        queue.Enqueue(start);

        while (rooms.Count < _totalRooms && queue.Count > 0)
        {
            var current = queue.Dequeue();
            var directions = GetShuffledDirections();
            int branched = 0;

            foreach (var dir in directions)
            {
                if (rooms.Count >= _totalRooms || branched >= _maxBranch)
                    break;

                var next = current + dir;
                if (rooms.ContainsKey(next))
                    continue;

                rooms[next] = new RoomNode(next, RoomType.Normal);
                queue.Enqueue(next);
                branched++;
            }
        }

        AssignSpecialRooms(rooms);
        AssignDoorFlags(rooms);

        return new List<RoomNode>(rooms.Values);
    }

    private void AssignSpecialRooms(Dictionary<Vector2Int, RoomNode> rooms)
    {
        // 시작방에서 가장 먼 방을 보스방으로
        RoomNode farthest = null;
        float maxDist = 0;
        foreach (var node in rooms.Values)
        {
            if (node.RoomType == RoomType.Start) continue;
            float dist = Vector2Int.Distance(Vector2Int.zero, node.GridPosition);
            if (dist > maxDist)
            {
                maxDist = dist;
                farthest = node;
            }
        }
        if (farthest != null) farthest.RoomType = RoomType.Boss;

        // 막힌 방(인접 1개) 중 랜덤으로 Shop, Treasure 배치
        var deadEnds = new List<RoomNode>();
        foreach (var node in rooms.Values)
        {
            if (node.RoomType != RoomType.Normal) continue;
            int neighbors = CountNeighbors(node.GridPosition, rooms);
            if (neighbors == 1) deadEnds.Add(node);
        }

        Shuffle(deadEnds);
        if (deadEnds.Count > 0) deadEnds[0].RoomType = RoomType.Shop;
        if (deadEnds.Count > 1) deadEnds[1].RoomType = RoomType.Treasure;
    }

    private void AssignDoorFlags(Dictionary<Vector2Int, RoomNode> rooms)
    {
        var offsets = new (Vector2Int offset, DoorFlags flag)[]
        {
            (Vector2Int.up,    DoorFlags.North),
            (Vector2Int.down,  DoorFlags.South),
            (Vector2Int.right, DoorFlags.East),
            (Vector2Int.left,  DoorFlags.West),
        };

        foreach (var node in rooms.Values)
        {
            node.DoorFlags = DoorFlags.None;
            foreach (var (offset, flag) in offsets)
            {
                if (rooms.ContainsKey(node.GridPosition + offset))
                    node.DoorFlags |= flag;
            }
        }
    }

    private int CountNeighbors(Vector2Int pos, Dictionary<Vector2Int, RoomNode> rooms)
    {
        int count = 0;
        if (rooms.ContainsKey(pos + Vector2Int.up))    count++;
        if (rooms.ContainsKey(pos + Vector2Int.down))  count++;
        if (rooms.ContainsKey(pos + Vector2Int.right)) count++;
        if (rooms.ContainsKey(pos + Vector2Int.left))  count++;
        return count;
    }

    private List<Vector2Int> GetShuffledDirections()
    {
        var dirs = new List<Vector2Int>
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left
        };
        Shuffle(dirs);
        return dirs;
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}

public class RoomNode
{
    public Vector2Int GridPosition;
    public RoomType RoomType;
    public DoorFlags DoorFlags;

    public RoomNode(Vector2Int gridPosition, RoomType roomType)
    {
        GridPosition = gridPosition;
        RoomType = roomType;
    }
}
