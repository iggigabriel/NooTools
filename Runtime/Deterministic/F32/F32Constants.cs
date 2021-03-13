namespace Nootools.Deterministic
{
    public partial struct F32
    {
        public const int INT_BITS = 16;
        public const int FRAC_BITS = 31 - INT_BITS; // 1 bit is for sign

        public static class Raw
        {
            public const int MaxValue = int.MaxValue;
            public const int MinValue = int.MinValue;

            public const int One = 1 << INT_BITS;
            public const int MinusOne = -(1 << INT_BITS);
            public const int Zero = 0;
        }

        public static readonly F32 MaxValue = new F32(Raw.MaxValue);
        public static readonly F32 MinValue = new F32(Raw.MinValue);

        public static readonly F32 One = new F32(Raw.One);
        public static readonly F32 MinusOne = new F32(Raw.MinusOne);
        public static readonly F32 Zero = new F32(Raw.Zero);
    }
}