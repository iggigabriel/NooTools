using System.Globalization;

namespace Nootools.Deterministic
{
    public partial struct F32
    {
        private int raw;

        public F32(int rawValue)
        {
            raw = rawValue;
        }

        public static implicit operator decimal(F32 a)
        {
            return (decimal)a.raw / Raw.One;
        }

        public override string ToString()
        {
            return ((decimal)this).ToString(CultureInfo.InvariantCulture);
        }
    }
}