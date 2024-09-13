using UnityEngine;

namespace Noo.Tools
{
    public static class Easing
    {
        public enum Type
        {
            Linear,

            QuadraticIn,
            QuadraticOut,
            QuadraticInOut,

            CubicIn,
            CubicOut,
            CubicInOut,

            QuarticIn,
            QuarticOut,
            QuarticInOut,

            QuinticIn,
            QuinticOut,
            QuinticInOut,

            SinusoidalIn,
            SinusoidalOut,
            SinusoidalInOut,

            ExponentialIn,
            ExponentialOut,
            ExponentialInOut,

            CircularIn,
            CircularOut,
            CircularInOut,

            ElasticIn,
            ElasticOut,
            ElasticInOut,

            BackIn,
            BackOut,
            BackInOut,

            BounceIn,
            BounceOut,
            BounceInOut,

            Constant,
            BinaryIn,
            BinaryOut,
        }

        public static float Evaluate(Type type, float k)
        {
            return type switch
            {
                Type.QuadraticIn => Quadratic.In(k),
                Type.QuadraticOut => Quadratic.Out(k),
                Type.QuadraticInOut => Quadratic.InOut(k),
                Type.CubicIn => Cubic.In(k),
                Type.CubicOut => Cubic.Out(k),
                Type.CubicInOut => Cubic.InOut(k),
                Type.QuarticIn => Quartic.In(k),
                Type.QuarticOut => Quartic.Out(k),
                Type.QuarticInOut => Quartic.InOut(k),
                Type.QuinticIn => Quintic.In(k),
                Type.QuinticOut => Quintic.Out(k),
                Type.QuinticInOut => Quintic.InOut(k),
                Type.SinusoidalIn => Sinusoidal.In(k),
                Type.SinusoidalOut => Sinusoidal.Out(k),
                Type.SinusoidalInOut => Sinusoidal.InOut(k),
                Type.ExponentialIn => Exponential.In(k),
                Type.ExponentialOut => Exponential.Out(k),
                Type.ExponentialInOut => Exponential.InOut(k),
                Type.CircularIn => Circular.In(k),
                Type.CircularOut => Circular.Out(k),
                Type.CircularInOut => Circular.InOut(k),
                Type.ElasticIn => Elastic.In(k),
                Type.ElasticOut => Elastic.Out(k),
                Type.ElasticInOut => Elastic.InOut(k),
                Type.BackIn => Back.In(k),
                Type.BackOut => Back.Out(k),
                Type.BackInOut => Back.InOut(k),
                Type.BounceIn => Bounce.In(k),
                Type.BounceOut => Bounce.Out(k),
                Type.BounceInOut => Bounce.InOut(k),
                Type.Constant => Constant(k),
                Type.BinaryIn => Binary.In(k),
                Type.BinaryOut => Binary.In(k),
                _ => k,
            };
        }

        public static float Linear(float k)
        {
            return k;
        }

        public static class Pow2
        {
            public static float In(float k)
            {
                return k * k;
            }

            public static float Out(float k)
            {
                return k * (2f - k);
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return 0.5f * k * k;
                return -0.5f * ((k -= 1f) * (k - 2f) - 1f);
            }
        }

        public static class Pow3
        {
            public static float In(float k)
            {
                return k * k * k;
            }

            public static float Out(float k)
            {
                return 1f + ((k -= 1f) * k * k);
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return 0.5f * k * k * k;
                return 0.5f * ((k -= 2f) * k * k + 2f);
            }
        };

        public static class Pow4
        {
            public static float In(float k)
            {
                return k * k * k * k;
            }

            public static float Out(float k)
            {
                return 1f - ((k -= 1f) * k * k * k);
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return 0.5f * k * k * k * k;
                return -0.5f * ((k -= 2f) * k * k * k - 2f);
            }
        };

        public static class Pow5
        {
            public static float In(float k)
            {
                return k * k * k * k * k;
            }

            public static float Out(float k)
            {
                return 1f + ((k -= 1f) * k * k * k * k);
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return 0.5f * k * k * k * k * k;
                return 0.5f * ((k -= 2f) * k * k * k * k + 2f);
            }
        };

        public static class Pow6
        {
            public static float In(float k)
            {
                return k * k * k * k * k * k;
            }

            public static float Out(float k)
            {
                return 1f + ((k -= 1f) * k * k * k * k * k);
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return 0.5f * k * k * k * k * k * k;
                return 0.5f * ((k -= 2f) * k * k * k * k * k + 2f);
            }
        };

        public static class Quadratic
        {
            public static float In(float k)
            {
                return k * k;
            }

            public static float Out(float k)
            {
                return k * (2f - k);
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return 0.5f * k * k;
                return -0.5f * ((k -= 1f) * (k - 2f) - 1f);
            }
        };

        public static class Cubic
        {
            public static float In(float k)
            {
                return k * k * k;
            }

            public static float Out(float k)
            {
                return 1f + ((k -= 1f) * k * k);
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return 0.5f * k * k * k;
                return 0.5f * ((k -= 2f) * k * k + 2f);
            }
        };

        public static class Quartic
        {
            public static float In(float k)
            {
                return k * k * k * k;
            }

            public static float Out(float k)
            {
                return 1f - ((k -= 1f) * k * k * k);
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return 0.5f * k * k * k * k;
                return -0.5f * ((k -= 2f) * k * k * k - 2f);
            }
        };

        public static class Quintic
        {
            public static float In(float k)
            {
                return k * k * k * k * k;
            }

            public static float Out(float k)
            {
                return 1f + ((k -= 1f) * k * k * k * k);
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return 0.5f * k * k * k * k * k;
                return 0.5f * ((k -= 2f) * k * k * k * k + 2f);
            }
        };

        public static class Sinusoidal
        {
            public static float In(float k)
            {
                return 1f - Mathf.Cos(k * Mathf.PI / 2f);
            }

            public static float Out(float k)
            {
                return Mathf.Sin(k * Mathf.PI / 2f);
            }

            public static float InOut(float k)
            {
                return 0.5f * (1f - Mathf.Cos(Mathf.PI * k));
            }
        };

        public static class Exponential
        {
            public static float In(float k)
            {
                return k == 0f ? 0f : Mathf.Pow(1024f, k - 1f);
            }

            public static float Out(float k)
            {
                return k == 1f ? 1f : 1f - Mathf.Pow(2f, -10f * k);
            }

            public static float InOut(float k)
            {
                if (k == 0f) return 0f;
                if (k == 1f) return 1f;
                if ((k *= 2f) < 1f) return 0.5f * Mathf.Pow(1024f, k - 1f);
                return 0.5f * (-Mathf.Pow(2f, -10f * (k - 1f)) + 2f);
            }
        };

        public static class Circular
        {
            public static float In(float k)
            {
                return 1f - Mathf.Sqrt(1f - k * k);
            }

            public static float Out(float k)
            {
                return Mathf.Sqrt(1f - ((k -= 1f) * k));
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return -0.5f * (Mathf.Sqrt(1f - k * k) - 1);
                return 0.5f * (Mathf.Sqrt(1f - (k -= 2f) * k) + 1f);
            }
        };

        public static class Elastic
        {
            public static float In(float k)
            {
                if (k == 0) return 0;
                if (k == 1) return 1;
                return -Mathf.Pow(2f, 10f * (k -= 1f)) * Mathf.Sin((k - 0.1f) * (2f * Mathf.PI) / 0.4f);
            }

            public static float Out(float k)
            {
                if (k == 0) return 0;
                if (k == 1) return 1;
                return Mathf.Pow(2f, -10f * k) * Mathf.Sin((k - 0.1f) * (2f * Mathf.PI) / 0.4f) + 1f;
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return -0.5f * Mathf.Pow(2f, 10f * (k -= 1f)) * Mathf.Sin((k - 0.1f) * (2f * Mathf.PI) / 0.4f);
                return Mathf.Pow(2f, -10f * (k -= 1f)) * Mathf.Sin((k - 0.1f) * (2f * Mathf.PI) / 0.4f) * 0.5f + 1f;
            }
        };

        public static class Back
        {
            const float s = 1.70158f;
            const float s2 = 2.5949095f;

            public static float In(float k)
            {
                return k * k * ((s + 1f) * k - s);
            }

            public static float Out(float k)
            {
                return (k -= 1f) * k * ((s + 1f) * k + s) + 1f;
            }

            public static float InOut(float k)
            {
                if ((k *= 2f) < 1f) return 0.5f * (k * k * ((s2 + 1f) * k - s2));
                return 0.5f * ((k -= 2f) * k * ((s2 + 1f) * k + s2) + 2f);
            }
        };

        public static class Bounce
        {
            public static float In(float k)
            {
                return 1f - Out(1f - k);
            }

            public static float Out(float k)
            {
                if (k < (1f / 2.75f))
                {
                    return 7.5625f * k * k;
                }
                else if (k < (2f / 2.75f))
                {
                    return 7.5625f * (k -= (1.5f / 2.75f)) * k + 0.75f;
                }
                else if (k < (2.5f / 2.75f))
                {
                    return 7.5625f * (k -= (2.25f / 2.75f)) * k + 0.9375f;
                }
                else
                {
                    return 7.5625f * (k -= (2.625f / 2.75f)) * k + 0.984375f;
                }
            }

            public static float InOut(float k)
            {
                if (k < 0.5f) return In(k * 2f) * 0.5f;
                return Out(k * 2f - 1f) * 0.5f + 0.5f;
            }
        };

        public static float Constant(float k)
        {
            return 1f;
        }

        public static class Binary
        {
            public static float In(float k)
            {
                return k > 0.5f ? 1f : 0f;
            }

            public static float Out(float k)
            {
                return k > 0.5f ? 0f : 1f;
            }
        }

        public static float ElasticVelocity(float from, float to, float currentVelocity, float elasticity = .8f, float strength = .2f)
        {
            return (currentVelocity * elasticity) + ((to - from) * strength);
        }

        public static Vector2 ElasticVelocity(Vector2 from, Vector2 to, Vector2 currentVelocity, float elasticity = .8f, float strength = .2f)
        {
            return (currentVelocity * elasticity) + ((to - from) * strength);
        }
        public static Vector3 ElasticVelocity(Vector3 from, Vector3 to, Vector3 currentVelocity, float elasticity = .8f, float strength = .2f)
        {
            return (currentVelocity * elasticity) + ((to - from) * strength);
        }
    }
}
