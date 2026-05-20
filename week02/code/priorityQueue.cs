public class PriorityItem
{
    public string Value { get; set; }
    public int Priority { get; set; }

    public PriorityItem(string value, int priority)
    {
        Value = value;
        Priority = priority;
    }

    public override string ToString()
    {
        return $"{Value} (priority {Priority})";
    }
}

public class PriorityQueue
{
    private readonly List<PriorityItem> _queue = new();

    public int Length => _queue.Count;

    /// <summary>
    /// Add an item with a priority to the BACK of the queue.
    /// Higher numbers = higher priority.
    /// </summary>
    public void Enqueue(string value, int priority)
    {
        _queue.Add(new PriorityItem(value, priority));
    }

    /// <summary>
    /// Remove and return the VALUE of the highest priority item.
    /// If there are ties, the one closest to the FRONT (earliest added) wins.
    /// Throws InvalidOperationException if empty.
    /// </summary>
    public string Dequeue()
    {
        if (_queue.Count == 0)
            throw new InvalidOperationException("The queue is empty.");

        // Find the index of the highest priority item
        // In case of ties, we want the FIRST (lowest index) one — so use strict >
        int highestIndex = 0;
        for (int i = 1; i < _queue.Count; i++)
        {
            // IMPORTANT: strictly greater than (>) preserves FIFO on ties
            // Using >= would incorrectly prefer the LAST tied item
            if (_queue[i].Priority > _queue[highestIndex].Priority)
            {
                highestIndex = i;
            }
        }

        string value = _queue[highestIndex].Value;
        _queue.RemoveAt(highestIndex);
        return value;
    }
}
