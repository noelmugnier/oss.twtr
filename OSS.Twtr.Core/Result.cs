namespace OSS.Twtr.Core;

public record Result<TLeft> : Either<TLeft, IEnumerable<Error>>
{
    public Result(TLeft left) : base(left){}
    public Result(IEnumerable<Error> errors) : base(errors){}
}

public record struct Unit(){}