namespace MyTask;

[TestClass]
public class MyTaskTests
{
    [TestMethod]
    public async Task QueueUserWorkItemsWithSimpleValue()
    {
        List<int> numbers = new List<int>(10);
        List<MyTask> tasks = new List<MyTask>(10);
        for (int i = 0; i < 10; i++)
        {
            var item = i;
            tasks.Add(MyTask.Run(() =>
            {
                numbers.Add(item);
            }));
        }

        // let tasks to finish
        await MyTask.WhenAll(tasks);
        
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
        List<MyTask> tasks = new List<MyTask>(10);
        for (int i = 0; i < 10; i++)
        {
            myValue.Value = i;
            tasks.Add(MyTask.Run(() =>
            {
                numbers.Add(myValue.Value);
            }));
        }

        // let tasks to finish
        await MyTask.WhenAll(tasks);
        
        // assert
        Assert.AreEqual(10, numbers.Count);
        numbers.Sort();
        for (int i = 0; i < 10; i++)
        {
            Assert.AreEqual(i, numbers[i]);
        }
    }
}