namespace OSS.Twtr.Core;

public record Result<TLeft> : Either<TLeft, IEnumerable<Error>>
{
    public Result(TLeft left) : base(left)
    {
    }

    public Result(IEnumerable<Error> errors) : base(errors)
    {
    }

    public Result(Error error) : base(new List<Error>{error})
    {
    }

    public Result(Exception e) : base(new List<Error>{new Error(e.Message)})
    {
    }
}

public record struct Unit();