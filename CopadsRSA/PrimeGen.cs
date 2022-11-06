/// Author: Benjamin Piro
/// Email: brp8396@rit.edu
/// File Description: Generates a user-specified amount of large prime numbers.

using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;

namespace PrimeGen
{
    /// <summary>
    ///     Main class for performing large prime number generation.
    /// </summary>
    class PrimeGen
    {

        /// <summary>
        ///     Generates and displays prime numbers based on validated user input,
        ///     and displays the time taken to do so using multithreading.
        /// </summary>
        /// <param name="numBits">Size of each prime</param>
        /// <param name="count">Number of primes to generate</param>
        /// <exception cref="PrimeGenException">
        ///     Thrown if numBits < 32 or not a multiple of 8, or if count < 0.
        /// </exception>
        public void GeneratePrimes(int numBits, int count) 
        {
            // Validate number of bits
            if (numBits < 32 || numBits % 8 != 0)
            {
                throw new PrimeGenException("Invalid number of bits");
            }

            // Validate number of primes to generate
            if (count < 0)
            {
                throw new PrimeGenException("Invalid amount of primes to generate");
            }

            var numPrimesFound = 0;
            var sw = new Stopwatch();
            sw.Start();
            // in the event count number of values are not found,
            // keep iterating until all requested values are found
            // this while loop is technically redundant, but serves as a theoretical failsafe
            while (numPrimesFound < count) 
            {
                var prime = GeneratePrime(numBits);
                if (numPrimesFound >= count)
                {
                    sw.Stop();
                }
                Console.WriteLine($"{++numPrimesFound}: {prime}");
                if (numPrimesFound >= count)
                {
                    // display generation time
                    Console.WriteLine($"Time to Generate: {sw.Elapsed}");
                }
                else
                {
                    // print a newline if prime generation is not done
                    Console.WriteLine();
                }
            }
        }

        /// <summary>
        ///     Loops forever until a prime number is generated
        /// </summary>
        /// <param name="numBits">Size of the prime</param>
        /// <returns>A large prime number</returns>
        /// <exception cref="PrimeGenException">
        ///     Thrown if numBits < 32 or not a multiple of 8.
        /// </exception>
        public BigInteger GeneratePrime(int numBits)
        {
            // Validate number of bits
            if (numBits < 32 || numBits % 8 != 0)
            {
                throw new PrimeGenException("Invalid number of bits");
            }

            BigInteger prime = -1;
            while (prime < 0)
            {
                var tp = Parallel.For(0, long.MaxValue, (i, state) =>
                {
                    if (prime < 0)
                    {
                        // Generate a random BigInteger
                        byte[] bytes = RandomNumberGenerator.GetBytes(numBits / 8);
                        var bi = new BigInteger(bytes, isUnsigned: true);
                        // Evaluate primeness
                        if (bi.IsProbablyPrime())
                        {
                            prime = bi;
                            state.Stop();
                        }
                    }
                    else
                    {
                        state.Stop();
                    }
                });
            }
            return prime;
        }
    }

    /// <summary>
    ///     Static class containing extension methods.
    /// </summary>
    public static class Extensions
    {
        // static list of the first 500 prime numbers used to check if a number is trivially prime
        private static readonly int[] basicPrimes = new int[] { 
            2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 
            79, 83, 89, 97, 101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 
            167, 173, 179, 181, 191, 193, 197, 199, 211, 223, 227, 229, 233, 239, 241, 251, 
            257, 263, 269, 271, 277, 281, 283, 293, 307, 311, 313, 317, 331, 337, 347, 349, 
            353, 359, 367, 373, 379, 383, 389, 397, 401, 409, 419, 421, 431, 433, 439, 443, 
            449, 457, 461, 463, 467, 479, 487, 491, 499, 503, 509, 521, 523, 541, 547, 557, 
            563, 569, 571, 577, 587, 593, 599, 601, 607, 613, 617, 619, 631, 641, 643, 647, 
            653, 659, 661, 673, 677, 683, 691, 701, 709, 719, 727, 733, 739, 743, 751, 757, 
            761, 769, 773, 787, 797, 809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 
            877, 881, 883, 887, 907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 
            991, 997, 1009, 1013, 1019, 1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069, 
            1087, 1091, 1093, 1097, 1103, 1109, 1117, 1123, 1129, 1151, 1153, 1163, 1171, 1181, 
            1187, 1193, 1201, 1213, 1217, 1223, 1229, 1231, 1237, 1249, 1259, 1277, 1279, 1283, 
            1289, 1291, 1297, 1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373, 1381, 1399, 
            1409, 1423, 1427, 1429, 1433, 1439, 1447, 1451, 1453, 1459, 1471, 1481, 1483, 1487, 
            1489, 1493, 1499, 1511, 1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583, 
            1597, 1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657, 1663, 1667, 1669, 1693, 
            1697, 1699, 1709, 1721, 1723, 1733, 1741, 1747, 1753, 1759, 1777, 1783, 1787, 1789, 
            1801, 1811, 1823, 1831, 1847, 1861, 1867, 1871, 1873, 1877, 1879, 1889, 1901, 1907, 
            1913, 1931, 1933, 1949, 1951, 1973, 1979, 1987, 1993, 1997, 1999, 2003, 2011, 2017, 
            2027, 2029, 2039, 2053, 2063, 2069, 2081, 2083, 2087, 2089, 2099, 2111, 2113, 2129, 
            2131, 2137, 2141, 2143, 2153, 2161, 2179, 2203, 2207, 2213, 2221, 2237, 2239, 2243, 
            2251, 2267, 2269, 2273, 2281, 2287, 2293, 2297, 2309, 2311, 2333, 2339, 2341, 2347, 
            2351, 2357, 2371, 2377, 2381, 2383, 2389, 2393, 2399, 2411, 2417, 2423, 2437, 2441, 
            2447, 2459, 2467, 2473, 2477, 2503, 2521, 2531, 2539, 2543, 2549, 2551, 2557, 2579, 
            2591, 2593, 2609, 2617, 2621, 2633, 2647, 2657, 2659, 2663, 2671, 2677, 2683, 2687, 
            2689, 2693, 2699, 2707, 2711, 2713, 2719, 2729, 2731, 2741, 2749, 2753, 2767, 2777, 
            2789, 2791, 2797, 2801, 2803, 2819, 2833, 2837, 2843, 2851, 2857, 2861, 2879, 2887, 
            2897, 2903, 2909, 2917, 2927, 2939, 2953, 2957, 2963, 2969, 2971, 2999, 3001, 3011, 
            3019, 3023, 3037, 3041, 3049, 3061, 3067, 3079, 3083, 3089, 3109, 3119, 3121, 3137, 
            3163, 3167, 3169, 3181, 3187, 3191, 3203, 3209, 3217, 3221, 3229, 3251, 3253, 3257, 
            3259, 3271, 3299, 3301, 3307, 3313, 3319, 3323, 3329, 3331, 3343, 3347, 3359, 3361, 
            3371, 3373, 3389, 3391, 3407, 3413, 3433, 3449, 3457, 3461, 3463, 3467, 3469, 3491, 
            3499, 3511, 3517, 3527, 3529, 3533, 3539, 3541, 3547, 3557, 3559, 3571 };

        /// <summary>
        ///     Tests whether a given BigInteger is composite or likely prime.
        /// </summary>
        /// <param name="value">
        ///     The BigInteger being evaluated, also referred to as "n".
        ///     Value is limited to being greater than or equal to 0.
        /// </param>
        /// <param name="k">
        ///     Maximum number of test iterations before returning true.
        /// </param>
        /// <returns>
        ///     True if value is likely prime, false if value is composite.
        /// </returns>
        public static bool IsProbablyPrime(this BigInteger value, int k = 10)
        {
            // check value is non-trivial
            if (value < 4)
            {
                // 1, 2, 3 are prime numbers
                if (value >= 1 && value <= 3)
                {
                    return true;
                }
                // even numbers (except 2) are not prime
                return false;
            }

            // check value is not a multiple of the first 500 primes
            foreach (var prime in basicPrimes)
            {
                if (value % prime == 0)
                {
                    return false;
                }
            }
            // calculate r and d in "n = (2^r) * d"
            BigInteger d = value - 1;
            int r = 0;
            // factor out powers of 2 from n - 1
            /* this loop will always iterate at least once since
             * n - 1 is guaranteed to be an even number, therefore
             * r can never be 0
             */
            while (d % 2 == 0)
            {
                d /= 2;
                r++;
            }
            int i = 0;
            for (; i < k; i++) // witness loop
            {
                // pick rand int a in [2, n-2]
                /*done by finding random BigInteger, modding by(n-3) which
                * is guaranteed to be positive since n must be > 3, then adding 2
                * in order to find a value in the desired range*/
                byte[] randBytes = RandomNumberGenerator.GetBytes((int)value.GetBitLength());
                BigInteger x = new BigInteger(randBytes, isUnsigned: true);

                // x = (a^d) % n
                x = BigInteger.ModPow((x % (value - 3)) + 2, d, value);

                // (x^2) % n = n - 1 or 1 is inconclusive, so continue witness loop
                if (x == 1 || x == value - 1)
                {
                    continue;
                }
                int j = 0;
                // flag for checking whether or not to continue outer loop
                bool continueWitness = false;
                for (; j < r - 1; j++)
                {
                    // x = (x^2) % n
                    x = BigInteger.ModPow(x, 2, value);
                    // (x^2) % n = n - 1 is inconclusive, so continue witness loop
                    if (x == value - 1)
                    {
                        // break this loop and continue the outer (witness) loop
                        continueWitness = true;
                        break;
                    }
                }
                // if (x^2) % n is not inconclusive, then n is composite
                if (!continueWitness)
                {
                    return false;
                }
            }
            return true;
        }
    }

    /// <summary>
    ///     Custom exception for handling invalid PrimeGen arguments.
    /// </summary>
    class PrimeGenException : Exception
    {
        // Base constructor for PrimeGenException
        public PrimeGenException() { }
        // Constructore for PrimeGenException that takes a message argument
        public PrimeGenException(string msg) : base(msg) { }
    }
}