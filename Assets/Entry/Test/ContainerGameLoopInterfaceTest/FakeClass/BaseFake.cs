public class BaseFake
{
    public string Result { get; private set; }

    protected void SetResult(string result) => Result = result;

    protected void SetResult() => SetResult("EXE");
}