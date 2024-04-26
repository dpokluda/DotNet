namespace MyTask;

[TestClass]
public class MyThreadPoolTests
{
    [TestMethod]
    public async Task QueueUserWorkItemsWithSimpleValue()
    {
        List<int> numbers = new List<int>(10);
        for (int i = 0; i < 10; i++)
        {
            var item = i;
            MyThreadPool.QueueUserWorkItem(delegate
            {
                numbers.Add(item);
            });
        }

        // let tasks to finish
        await MyTask.Delay(200);
        
        // assert
        Assert.AreEqual(10, numbers.Count);
        numbers.Sort();
        for (int i = 0; i < 10; i++)
        {
            Assert.AreEqual(i, numbers[i]);
        }
    }
    
    [TestMethod]
    public async Task QueueUserWorkItemsWithAsyncLocal()
    {
        List<int> numbers = new List<int>(10);
        AsyncLocal<int> myValue = new AsyncLocal<int>();
        for (int i = 0; i < 10; i++)
        {
            myValue.Value = i;
            MyThreadPool.QueueUserWorkItem(delegate
            {
                numbers.Add(myValue.Value);
            });
        }

        // let tasks to finish
        await MyTask.Delay(200);
        
        // assert
        Assert.AreEqual(10, numbers.Count);
        numbers.Sort();
        for (int i = 0; i < 10; i++)
        {
            Assert.AreEqual(i, numbers[i]);
        }
    }
}