using System;
using UnityEngine;

namespace Noo.Tools
{
    [Serializable]
    public struct SfloatSODCurve
    {
        public static Sfloat DeltaTime => Sfloat.Ratio100(1);

        public static readonly SfloatSODCurve Default = new(Sfloat.One, Sfloat.One, Sfloat.One);
        public static readonly SfloatSODCurve Damped = new(Sfloat.One * 3, Sfloat.One / 2, Sfloat.One * -2);
        public static readonly SfloatSODCurve Elastic = new(Sfloat.One * 4, Sfloat.One / 3, Sfloat.Zero);
        public static readonly SfloatSODCurve Undershoot = new(Sfloat.One * 4, Sfloat.One, Sfloat.Half * -3);

        [SerializeField, Tooltip("Speed at which the system will respond to changes in the input, measured in cycles per second")]
        private Sfloat frequency;

        [SerializeField, Tooltip("0 = never stops vibrating, 0-1 = vibration strenght, >1 = never vibrates")]
        private Sfloat damping;

        [SerializeField, Tooltip("<0 = anticipate, 0-1 = response strenght, >1 = overshoot")]
        private Sfloat response;

        [SerializeField]
        private Sfloat3 kValues;

        /// <summary>
        /// Speed at which the system will respond to changes in the input, measured in cycles per second.
        /// </summary>
        public Sfloat Frequency
        {
            readonly get => frequency;
            set
            {
                frequency = value;
                RecalculateKValues();
            }
        }

        /// <summary>
        /// 0 = never stops vibrating, 0-1 = vibration strenght, >1 = never vibrates
        /// </summary>
        public Sfloat Damping
        {
            readonly get => damping;
            set
            {
                damping = value;
                RecalculateKValues();
            }
        }

        /// <summary>
        /// <0 = anticipate, 0-1 = response strenght, >1 = overshoot
        /// </summary>
        public Sfloat Response
        {
            readonly get => response;
            set
            {
                response = value;
                RecalculateKValues();
            }
        }

        public readonly Sfloat3 KValues => kValues;

        public SfloatSODCurve(Sfloat frequency = default, Sfloat damping = default, Sfloat response = default)
        {
            this.frequency = frequency;
            this.damping = damping;
            this.response = response;

            kValues = default;

            RecalculateKValues();
        }

        public void Set(Sfloat? frequency = default, Sfloat? damping = default, Sfloat? response = default)
        {
            if (frequency.HasValue) this.frequency = frequency.Value;
            if (damping.HasValue) this.damping = damping.Value;
            if (response.HasValue) this.response = response.Value;

            RecalculateKValues();
        }

        public static SfloatSODCurve Lerp(SfloatSODCurve a, SfloatSODCurve b, Sfloat t)
        {
            var c = default(SfloatSODCurve);

            c.damping = Sfloat.Lerp(a.damping, b.damping, t);
            c.frequency = Sfloat.Lerp(a.frequency, b.frequency, t);
            c.response = Sfloat.Lerp(a.response, b.response, t);
            c.kValues = Sfloat3.Lerp(a.kValues, b.kValues, t);

            return c;
        }

        private void RecalculateKValues()
        {
            Sfloat f = Sfloat.Max(Sfloat.Ratio1000(1), Sfloat.Pi * frequency);
            Sfloat fdbl = Sfloat.Two * f;

            kValues.x = damping / f;

            kValues.y = Sfloat.One / (fdbl * fdbl);
            kValues.y = Sfloat.Max(kValues.y, DeltaTime * DeltaTime / Sfloat.Two + DeltaTime * kValues.x / Sfloat.Two);
            kValues.y = Sfloat.Max(kValues.y / DeltaTime, kValues.x);

            kValues.z = response * damping / fdbl / DeltaTime;
        }
    }
}