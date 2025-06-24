using System;
using UnityEngine.UIElements;

namespace Noo.DevToolkit
{
    public class NuiPropertyDrawerInt : NuiPropertyDrawerNumericGeneric<int, IntegerField> { }
    public class NuiPropertyDrawerLong : NuiPropertyDrawerNumericGeneric<long, LongField> { }

    public class NuiPropertyDrawerUnsignedInt : NuiPropertyDrawerNumericGeneric<uint, UnsignedIntegerField> { }
    public class NuiPropertyDrawerUnsignedLong : NuiPropertyDrawerNumericGeneric<ulong, UnsignedLongField> { }

    public class NuiPropertyDrawerFloat : NuiPropertyDrawerNumericGeneric<float, FloatField> { }
    public class NuiPropertyDrawerDouble : NuiPropertyDrawerNumericGeneric<double, DoubleField> { }

    public class NuiPropertyDrawerByte : NuiPropertyDrawerNumeric<byte>
    {
        protected override byte FromInt64(long value) => (byte)Math.Clamp(value, byte.MinValue, byte.MaxValue);
        protected override long ToInt64(byte value) => value;
    }

    public class NuiPropertyDrawerSbyte : NuiPropertyDrawerNumeric<sbyte>
    {
        protected override sbyte FromInt64(long value) => (sbyte)Math.Clamp(value, sbyte.MinValue, sbyte.MaxValue);
        protected override long ToInt64(sbyte value) => value;
    }

    public class NuiPropertyDrawerShort : NuiPropertyDrawerNumeric<short>
    {
        protected override short FromInt64(long value) => (short)Math.Clamp(value, short.MinValue, short.MaxValue);
        protected override long ToInt64(short value) => value;
    }

    public class NuiPropertyDrawerUshort : NuiPropertyDrawerNumeric<ushort>
    {
        protected override ushort FromInt64(long value) => (ushort)Math.Clamp(value, ushort.MinValue, ushort.MaxValue);
        protected override long ToInt64(ushort value) => value;
    }
}
