using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class PriorityQueue_Tests
{
    // =============================================================
    // TEST: TestEnqueue_SingleItem_DequeuesCorrectly
    // Requirement covered: Enqueue adds to back; Dequeue returns value.
    //
    // Expected Result: PASS after fix.
    //
    // Error Found: None for this case — basic enqueue/dequeue worked
    // in the original but returned the whole object instead of just
    // the string value.
    // Fix: Dequeue returns item.Value (string), not the PriorityItem object.
    // =============================================================
    [TestMethod]
    public void TestEnqueue_SingleItem_DequeuesCorrectly()
    {
        var pq = new PriorityQueue();
        pq.Enqueue("apple", 3);

        string result = pq.Dequeue();

        Assert.AreEqual("apple", result);
        Assert.AreEqual(0, pq.Length);
    }

    // =============================================================
    // TEST: TestDequeue_HighestPriorityRemovedFirst
    // Requirement covered: Dequeue removes the highest priority item.
    //
    // Expected Result: PASS after fix.
    //
    // Error Found: The original code used >= instead of > when
    // searching for the highest priority item. This caused the LAST
    // tied item to be removed instead of the first one, violating the
    // FIFO tiebreak rule AND sometimes removing wrong-priority items.
    // Fix: Changed >= to > so the first highest-priority item wins.
    // =============================================================
    [TestMethod]
    public void TestDequeue_HighestPriorityRemovedFirst()
    {
        var pq = new PriorityQueue();
        pq.Enqueue("low", 1);
        pq.Enqueue("high", 5);
        pq.Enqueue("medium", 3);

        string result = pq.Dequeue();

        Assert.AreEqual("high", result);
    }

    // =============================================================
    // TEST: TestDequeue_TiePriority_FIFOOrder
    // Requirement covered: When priorities are equal, the item closest
    // to the FRONT (first added) is removed first.
    //
    // Expected Result: PASS after fix.
    //
    // Error Found: The original code used >= when scanning for the
    // highest index, which meant it always picked the LAST item with
    // the highest priority, violating the FIFO requirement.
    // Fix: Changed >= to > — strict greater-than preserves FIFO on ties
    // because highestIndex is only updated when a strictly better
    // priority is found.
    // =============================================================
    [TestMethod]
    public void TestDequeue_TiePriority_FIFOOrder()
    {
        var pq = new PriorityQueue();
        pq.Enqueue("first", 5);
        pq.Enqueue("second", 5);
        pq.Enqueue("third", 5);

        // All same priority — should come out FIFO
        Assert.AreEqual("first", pq.Dequeue());
        Assert.AreEqual("second", pq.Dequeue());
        Assert.AreEqual("third", pq.Dequeue());
    }

    // =============================================================
    // TEST: TestDequeue_EmptyQueue_ThrowsInvalidOperationException
    // Requirement covered: Empty queue throws InvalidOperationException
    // with message "The queue is empty."
    //
    // Expected Result: PASS after fix.
    //
    // Error Found: The original code had no guard clause for an empty
    // queue — it would throw an IndexOutOfRangeException (wrong type)
    // or NullReferenceException instead of the required
    // InvalidOperationException with the exact required message.
    // Fix: Added empty check throwing InvalidOperationException with
    // message "The queue is empty."
    // =============================================================
    [TestMethod]
    public void TestDequeue_EmptyQueue_ThrowsInvalidOperationException()
    {
        var pq = new PriorityQueue();

        try
        {
            pq.Dequeue();
            Assert.Fail("Expected InvalidOperationException was not thrown.");
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual("The queue is empty.", ex.Message);
        }
    }

    // =============================================================
    // TEST: TestEnqueue_GoesToBack_OrderPreservedOnEqualPriority
    // Requirement covered: Enqueue adds to BACK of queue.
    //
    // Expected Result: PASS after fix.
    //
    // Error Found: No separate bug here, but this test validates that
    // Enqueue does not insert by priority (it goes to the back), and
    // that the FIFO logic in Dequeue correctly handles the order.
    // =============================================================
    [TestMethod]
    public void TestEnqueue_GoesToBack_OrderPreservedOnEqualPriority()
    {
        var pq = new PriorityQueue();
        pq.Enqueue("a", 2);
        pq.Enqueue("b", 2);
        pq.Enqueue("c", 2);

        // Since all equal priority, FIFO order applies
        Assert.AreEqual("a", pq.Dequeue());
        Assert.AreEqual("b", pq.Dequeue());
        Assert.AreEqual("c", pq.Dequeue());
    }

    // =============================================================
    // TEST: TestDequeue_MixedPriorities_AllRemovedInPriorityOrder
    // Requirement covered: Dequeue always removes highest priority;
    // subsequent calls continue to do so correctly.
    //
    // Expected Result: PASS after fix.
    //
    // Error Found: The >= bug meant that after removing the wrong
    // item in a tie scenario, subsequent dequeues could also be wrong.
    // Fix: Corrected comparison to strict >.
    // =============================================================
    [TestMethod]
    public void TestDequeue_MixedPriorities_AllRemovedInPriorityOrder()
    {
        var pq = new PriorityQueue();
        pq.Enqueue("c", 1);
        pq.Enqueue("a", 5);
        pq.Enqueue("b", 3);
        pq.Enqueue("d", 5); // tie with "a"

        // "a" and "d" both have priority 5 — "a" was added first
        Assert.AreEqual("a", pq.Dequeue()); // highest and first added
        Assert.AreEqual("d", pq.Dequeue()); // second highest (tied)
        Assert.AreEqual("b", pq.Dequeue()); // priority 3
        Assert.AreEqual("c", pq.Dequeue()); // priority 1
    }

    // =============================================================
    // TEST: TestDequeue_SingleItemThenEmpty_ThrowsOnSecondCall
    // Requirement covered: Exception thrown when queue becomes empty
    // after being used.
    //
    // Expected Result: PASS after fix.
    //
    // Error Found: Same empty-queue bug — original code didn't guard
    // against dequeuing from a queue that BECOMES empty after use.
    // Fix: The empty check runs every time Dequeue is called.
    // =============================================================
    [TestMethod]
    public void TestDequeue_SingleItemThenEmpty_ThrowsOnSecondCall()
    {
        var pq = new PriorityQueue();
        pq.Enqueue("only", 1);
        pq.Dequeue(); // removes the only item

        try
        {
            pq.Dequeue(); // should throw
            Assert.Fail("Expected InvalidOperationException was not thrown.");
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual("The queue is empty.", ex.Message);
        }
    }
}
