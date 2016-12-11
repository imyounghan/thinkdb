
namespace ThinkDb.Expressions
{
    public enum ColumnExpressionType
    {
        None = 0,

        IsNull = 100,

        Count = 1,
        Min = 2,
        Max = 3,
        Sum = 4,
        Avg = 5,
        ToUpper = 12,
        ToLower = 13,
        Trim = 15,
        LTrim = 16,
        RTrim = 17,

        Year = 30,
        Month = 31,
        Day = 32,

        Abs = 50,
        Exp = 51,
        Floor = 52,
        Ln = 53,
        Log = 54,
        Pow = 55,
        Round = 56,
        Sign = 57,
        Sqrt = 58
    }
}
