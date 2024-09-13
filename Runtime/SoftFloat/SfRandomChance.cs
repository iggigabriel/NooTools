namespace Noo.Tools
{
    /// <summary>
    /// Binary chance generator with Pseudo Random Distribution
    /// based on https://gaming.stackexchange.com/a/178681
    /// </summary>
    public struct SfRandomChance
    {
        /// Lut generated from:
        /// 
        /// decimal CfromP( decimal p )
        /// {
        ///     decimal Cupper = p;
        ///     decimal Clower = 0m;
        ///     decimal Cmid;
        ///     decimal p1;
        ///     decimal p2 = 1m;
        ///     while(true)
        ///     {
        ///         Cmid = ( Cupper + Clower ) / 2m;
        ///         p1 = PfromC( Cmid );
        ///         if ( Math.Abs( p1 - p2 ) <= 0m ) break;
        ///         if ( p1 > p ) Cupper = Cmid;
        ///         else Clower = Cmid;
        ///         p2 = p1;
        ///     }
        /// 
        ///     return Cmid;
        /// }
        /// 
        /// decimal PfromC( decimal C )
        /// {
        ///     decimal pProcOnN = 0m;
        ///     decimal pProcByN = 0m;
        ///     decimal sumNpProcOnN = 0m;
        /// 
        ///     int maxFails = (int)Math.Ceiling( 1m / C );
        ///     for (int N = 1; N <= maxFails; ++N)
        ///     {
        ///         pProcOnN = Math.Min( 1m, N * C ) * (1m - pProcByN);
        ///         pProcByN += pProcOnN;
        ///         sumNpProcOnN += N * pProcOnN;
        ///     }
        /// 
        ///     return ( 1m / sumNpProcOnN );
        /// }
        static readonly int[] cLut = new int[]
        {
            0x0000,0x00F9,0x03C6,0x083F,0x0E42,
            0x15B1,0x1E73,0x2871,0x3398,0x3FD2,
            0x4D56,0x5C43,0x7B2B,0x9249,0xAAAA,
            0xC000,0xD2D2,0xE38E,0xF286,0x10000,
        };

        SfRandom rng;
        Sfloat chance;
        int k;

        public SfRandomChance(uint seed, Sfloat chance)
        {
            if (chance <= Sfloat.Zero || chance >= Sfloat.One)
            {
                this.chance = Sfloat.Clamp01(chance);
            }
            else
            {
                var lutChance = chance * Sfloat.FromInt(cLut.Length);
                var lutIndex = Sfloat.FloorToInt(lutChance);
                this.chance = Sfloat.Lerp(new Sfloat(cLut[lutIndex]), new Sfloat(cLut[lutIndex + 1]), Sfloat.Fract(lutChance));
            }

            k = 1;
            rng = new SfRandom(seed);
        }

        public bool Check()
        {
            var check = rng.NextBool(chance * k);
            if (check) k = 1; else k++;
            return check;
        }
    }
}
