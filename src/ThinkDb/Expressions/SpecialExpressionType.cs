
namespace ThinkDb.Expressions
{
    public enum SpecialExpressionType
    {
        IsNull = 100,
        IsNotNull = 101,
        Concat = 10,
        Count = 1,
        Exists = 102,
        Like = 103,
        Min = 2,
        Max = 3,
        Sum = 4,
        Avg = 5,
        StringLength = 11,
        ToUpper = 12,
        ToLower = 13,
        In = 104,
        Substring = 14,
        Trim = 15,
        LTrim = 16,
        RTrim = 17,

        StringInsert = 18,
        Replace = 19,
        Remove = 20,
        IndexOf = 21,

        Year = 30,
        Month = 31,
        Day = 32,
        Hour = 33,
        Minute = 34,
        Second = 35,
        Millisecond = 36,
        Now = 37,
        Date = 38,
        DateDiffInMilliseconds =39,

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
