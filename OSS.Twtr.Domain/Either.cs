namespace OSS.Twtr.Core;

public record Either<TLeft, TRight>
{
    private readonly TRight _right;
    private readonly TLeft _left;
    private readonly bool _isLeft;

    public Either(TLeft success)
    {
        _right = default;
        _left = success;
        _isLeft = true;
    }

    public Either(TRight error)
    {
        _left = default;
        _right = error;
        _isLeft = false;
    }

    public Task<TResult> On<TResult>(Func<TLeft, Task<TResult>> success, Func<TRight, Task<TResult>> error)
    {
        if (_isLeft)
            return success(_left);

        return error(_right);
    }

    public Task On(Func<TLeft, Task> success, Func<TRight, Task> error)
    {
        if (_isLeft)
            return success(_left);

        return error(_right);
    }

    public Task On(Func<Task> success, Func<Task> error)
    {
        if (_isLeft)
            return success();

        return error();
    }

    public T On<T>(Func<TLeft, T> success, Func<TRight, T> error)
    {
        if (_isLeft)
            return success(_left);

        return error(_right);
    }
}