namespace OSS.Twtr.Common.Core;

public record Either<TLeft, TRight>
{
    protected readonly TRight _right;
    protected readonly TLeft _left;
    protected readonly bool _isLeft;

    public Either(TLeft left)
    {
        _right = default;
        _left = left;
        _isLeft = true;
    }

    public Either(TRight right)
    {
        _left = default;
        _right = right;
        _isLeft = false;
    }

    public Task<TResult> On<TResult>(Func<TLeft, Task<TResult>> left, Func<TRight, Task<TResult>> right)
    {
        if (_isLeft)
            return left(_left);

        return right(_right);
    }

    public Task On(Func<TLeft, Task> left, Func<TRight, Task> right)
    {
        if (_isLeft)
            return left(_left);

        return right(_right);
    }

    public Task On(Func<Task> left, Func<Task> right)
    {
        if (_isLeft)
            return left();

        return right();
    }

    public T On<T>(Func<TLeft, T> left, Func<TRight, T> right)
    {
        if (_isLeft)
            return left(_left);

        return right(_right);
    }
}