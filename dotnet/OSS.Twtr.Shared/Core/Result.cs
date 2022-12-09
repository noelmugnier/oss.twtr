namespace OSS.Twtr.Core;

public sealed record Result<TLeft> : Either<TLeft, IEnumerable<Error>>
{
    public TLeft Data => _left;
    public IEnumerable<Error> Errors => _right;
    public bool Success => _isLeft;
    
    public Result(TLeft left) : base(left)
    {
    }

    public Result(IEnumerable<Error> errors) : base(errors)
    {
    }

    public Result(Error error) : base(new List<Error>{error})
    {
    }

    public Result(Exception e) : base(new List<Error>{new (e.Message)})
    {
    }
}