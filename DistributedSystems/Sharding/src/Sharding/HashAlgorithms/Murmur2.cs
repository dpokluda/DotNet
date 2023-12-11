namespace Sharding.HashAlgorithms;

/// <summary>
/// Hashing function implementing Murmur2 hashing algorithm.
/// </summary>
/// <seealso cref="HashFunction"/>
public class Murmur2 : HashFunction
    {
        /// <summary>
        /// Compute hash value for the specified data.
        /// </summary>
        /// <param name="data">The data to compute hash for.</param>
        /// <returns>
        /// Hash value.
        /// </returns>
        /// <seealso cref="Sharding.HashFunction.ComputeHash(byte[])"/>
        public override int ComputeHash(byte[] data)
        {
            const uint seed = 89478583;
            return (int) MurmurHash2(seed, data, data.Length);
        }

        private static uint MurmurHash2(uint seed32, byte[] data, int length)
        {
            const int M = 0x5bd1e995;
            const int R = 24;

            int len = data.Length;
            long hash = (uint)(seed32 ^ length);

            int i = 0;
            while (len >= 4)
            {
                int k = data[i + 0] & 0xFF;
                k |= (data[i + 1] & 0xFF) << 8;
                k |= (data[i + 2] & 0xFF) << 16;
                k |= (data[i + 3] & 0xFF) << 24;

                k *= M;
                k ^= RightMove(k, R);
                k *= M;

                hash *= M;
                hash ^= k;

                i += 4;
                len -= 4;
            }

            switch (len)
            {
                // reverse the last 3 bytes and convert it to an uint
                // so cast the last to into an UInt16 and get the 3rd as a byte
                // ABC --> CBA; (UInt16)(AB) --> BA
                //h ^= (uint)(*ptrByte);
                //h ^= (uint)(ptrByte[1] << 8);
                case 3:
                    hash ^= (data[i + 2] & 0xFF) << 16;
                    hash ^= (data[i + 1] & 0xFF) << 8;
                    hash ^= (data[i + 0] & 0xFF);
                    hash *= M;
                    break;

                case 2:
                    hash ^= (data[i + 1] & 0xFF) << 8;
                    hash ^= (data[i + 0] & 0xFF);
                    hash *= M;
                    break;

                case 1:
                    hash ^= (data[i + 0] & 0xFF);
                    hash *= M;
                    break;
            }

            hash ^= RightMove(hash, 13);
            hash *= M;
            hash ^= RightMove(hash, 15);

            return (uint)hash;
        }
    }