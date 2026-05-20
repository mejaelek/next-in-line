public class Person
{
    public string Name { get; set; }
    public int Turns { get; set; }

    public Person(string name, int turns)
    {
        Name = name;
        Turns = turns;
    }

    public override string ToString()
    {
        return $"{Name} ({Turns} turns)";
    }
}

public class TakingTurnsQueue
{
    private readonly Queue<Person> _queue = new();

    public int Length => _queue.Count;

    /// <summary>
    /// Add a person to the queue with a number of turns.
    /// A turns value of 0 or less means infinite turns.
    /// </summary>
    public void AddPerson(string name, int turns)
    {
        var person = new Person(name, turns);
        _queue.Enqueue(person);
    }

    /// <summary>
    /// Dequeue the next person. If they still have turns (or infinite turns),
    /// re-enqueue them. Throws InvalidOperationException if queue is empty.
    /// </summary>
    public Person GetNextPerson()
    {
        if (_queue.Count == 0)
            throw new InvalidOperationException("No one is in the queue.");

        Person person = _queue.Dequeue();

        // Infinite turns: 0 or less means re-enqueue always
        if (person.Turns <= 0)
        {
            _queue.Enqueue(person);
        }
        else
        {
            // Decrement turns BEFORE deciding whether to re-enqueue
            person.Turns--;

            if (person.Turns > 0)
                _queue.Enqueue(person);
        }

        return person;
    }
}
