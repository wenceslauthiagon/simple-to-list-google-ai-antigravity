namespace TodoList.Tests.Fakes;

public class TestTimeProvider : TimeProvider
{
    private DateTimeOffset _utcNow;

    public TestTimeProvider(DateTimeOffset initialTime)
    {
        _utcNow = initialTime;
    }

    public override DateTimeOffset GetUtcNow() => _utcNow;

    public void Advance(TimeSpan delta) => _utcNow = _utcNow.Add(delta);

    public void SetUtcNow(DateTimeOffset time) => _utcNow = time;
}
