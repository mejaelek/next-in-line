using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class TakingTurnsQueue_Tests
{
    // =============================================================
    // TEST: TestAddPerson_LimitedTurns
    // Description: Verifies that a person with a limited number of
    // turns is removed from the queue once their turns run out.
    //
    // Expected Result: PASS after fix.
    //
    // Error Found (before fix): The original code did NOT decrement
    // the Turns counter, so a person with 1 turn was re-enqueued
    // forever — creating an infinite loop.
    // Fix: Decrement person.Turns before checking whether to re-enqueue.
    // =============================================================
    [TestMethod]
    public void TestAddPerson_LimitedTurns()
    {
        var queue = new TakingTurnsQueue();
        queue.AddPerson("Alice", 2);
        queue.AddPerson("Bob", 1);

        // Bob has 1 turn, Alice has 2 turns.
        // Order: Alice(2), Bob(1)
        var p1 = queue.GetNextPerson(); // Alice (now 1 turn left), re-enqueued
        Assert.AreEqual("Alice", p1.Name);

        var p2 = queue.GetNextPerson(); // Bob (now 0 turns left), NOT re-enqueued
        Assert.AreEqual("Bob", p2.Name);

        var p3 = queue.GetNextPerson(); // Alice (now 0 turns left), NOT re-enqueued
        Assert.AreEqual("Alice", p3.Name);

        // Queue should now be empty
        Assert.AreEqual(0, queue.Length);
    }

    // =============================================================
    // TEST: TestAddPerson_InfiniteTurns
    // Description: Verifies that a person with 0 turns (infinite)
    // is always re-enqueued after being dequeued.
    //
    // Expected Result: PASS after fix.
    //
    // Error Found (before fix): The original code treated 0 turns as
    // a valid limited turn count, so infinite-turn people were
    // eventually removed from the queue.
    // Fix: Check if turns <= 0 BEFORE decrementing; if so, re-enqueue
    // without modifying turns.
    // =============================================================
    [TestMethod]
    public void TestAddPerson_InfiniteTurns()
    {
        var queue = new TakingTurnsQueue();
        queue.AddPerson("Infinite", 0); // 0 = infinite

        // Should always come back
        for (int i = 0; i < 10; i++)
        {
            var person = queue.GetNextPerson();
            Assert.AreEqual("Infinite", person.Name);
            Assert.AreEqual(1, queue.Length); // always re-enqueued
        }
    }

    // =============================================================
    // TEST: TestAddPerson_NegativeTurns_IsInfinite
    // Description: Verifies that a person with a negative turns value
    // is treated as having infinite turns (same as 0).
    //
    // Expected Result: PASS after fix.
    //
    // Error Found (before fix): Same root cause as infinite turns —
    // negative values were decremented and eventually went very
    // negative rather than being treated as infinite.
    // Fix: The <= 0 check covers negative values correctly.
    // =============================================================
    [TestMethod]
    public void TestAddPerson_NegativeTurns_IsInfinite()
    {
        var queue = new TakingTurnsQueue();
        queue.AddPerson("Eternal", -5);

        for (int i = 0; i < 5; i++)
        {
            var person = queue.GetNextPerson();
            Assert.AreEqual("Eternal", person.Name);
        }
    }

    // =============================================================
    // TEST: TestGetNextPerson_EmptyQueue_ThrowsException
    // Description: Verifies that calling GetNextPerson on an empty
    // queue throws an InvalidOperationException.
    //
    // Expected Result: PASS after fix.
    //
    // Error Found (before fix): The original code had no null/empty
    // check — it would crash with a NullReferenceException instead
    // of the required InvalidOperationException.
    // Fix: Added explicit empty check that throws InvalidOperationException.
    // =============================================================
    [TestMethod]
    [ExpectedException(typeof(InvalidOperationException))]
    public void TestGetNextPerson_EmptyQueue_ThrowsException()
    {
        var queue = new TakingTurnsQueue();
        queue.GetNextPerson(); // Should throw
    }

    // =============================================================
    // TEST: TestAddPerson_MultipleInfinite_RoundRobin
    // Description: Verifies that multiple infinite-turn people cycle
    // through the queue in FIFO order repeatedly (round-robin).
    //
    // Expected Result: PASS after fix.
    //
    // Error Found (before fix): None specific to this case, but
    // validates that re-enqueueing preserves round-robin order.
    // =============================================================
    [TestMethod]
    public void TestAddPerson_MultipleInfinite_RoundRobin()
    {
        var queue = new TakingTurnsQueue();
        queue.AddPerson("A", 0);
        queue.AddPerson("B", 0);
        queue.AddPerson("C", 0);

        string[] expected = { "A", "B", "C", "A", "B", "C" };
        for (int i = 0; i < expected.Length; i++)
        {
            var person = queue.GetNextPerson();
            Assert.AreEqual(expected[i], person.Name, $"Step {i}: expected {expected[i]}");
        }
    }

    // =============================================================
    // TEST: TestAddPerson_MixedTurns
    // Description: Verifies correct ordering when mixing limited and
    // infinite turn people in the same queue.
    //
    // Expected Result: PASS after fix.
    //
    // Error Found (before fix): Both bugs above combined would cause
    // incorrect turn ordering with mixed queues.
    // =============================================================
    [TestMethod]
    public void TestAddPerson_MixedTurns()
    {
        var queue = new TakingTurnsQueue();
        queue.AddPerson("Limited", 1); // only 1 turn
        queue.AddPerson("Infinite", 0); // infinite

        var p1 = queue.GetNextPerson(); // Limited (0 turns left — removed)
        Assert.AreEqual("Limited", p1.Name);

        var p2 = queue.GetNextPerson(); // Infinite (re-enqueued)
        Assert.AreEqual("Infinite", p2.Name);

        var p3 = queue.GetNextPerson(); // Infinite again
        Assert.AreEqual("Infinite", p3.Name);

        // Limited should be gone
        Assert.AreEqual(1, queue.Length);
    }
}