namespace MyChannelTests;

[TestClass]
public class MyChannelTests
{
    [TestMethod]
    public async Task Run()
    {
        var ch = new MyChannel.MyChannel<int>();
        _ = Task.Run(async () =>
        {
            for(var i = 0; i < 10; i++)
            {
                ch.Write(i);
                await Task.Delay(10);
            }
        });

        List<int> results = new();
        while (true)
        {
            results.Add(await ch.ReadAsync());
            if (results.Count == 10)
            {
                break;
            }
        }
        
        Assert.AreEqual(10, results.Count);
        for (var i = 0; i < 10; i++)
        {
            Assert.AreEqual(i, results[i]);
        }
    }
}