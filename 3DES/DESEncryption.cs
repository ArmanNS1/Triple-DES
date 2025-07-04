using System.Collections;

namespace DES
{
    public class DESEncryption
    {
        // Initial permutation table
        private static readonly int[] IP = new int[]
        {
                58,50,42,34,26,18,10,2,
                60,52,44,36,28,20,12,4,
                62,54,46,38,30,22,14,6,
                64,56,48,40,32,24,16,8,
                57,49,41,33,25,17,9,1,
                59,51,43,35,27,19,11,3,
                61,53,45,37,29,21,13,5,
                63,55,47,39,31,23,15,7
        };

        // Final permutation table (inverse of IP)
        private static readonly int[] FP = new int[]
        {
                40,8,48,16,56,24,64,32,
                39,7,47,15,55,23,63,31,
                38,6,46,14,54,22,62,30,
                37,5,45,13,53,21,61,29,
                36,4,44,12,52,20,60,28,
                35,3,43,11,51,19,59,27,
                34,2,42,10,50,18,58,26,
                33,1,41,9,49,17,57,25
        };

        // Expansion box (E-box) - expands 32-bit block to 48 bits
        private static readonly int[] EBox = new int[]
        {
            32, 1, 2, 3, 4, 5,
            4, 5, 6, 7, 8, 9,
            8, 9,10,11,12,13,
            12,13,14,15,16,17,
            16,17,18,19,20,21,
            20,21,22,23,24,25,
            24,25,26,27,28,29,
            28,29,30,31,32, 1
        };

        // P-box permutation
        private static readonly int[] PBox = new int[]
        {
            16, 7, 20, 21,
            29, 12, 28, 17,
            1, 15, 23, 26,
            5, 18, 31, 10,
            2, 8, 24, 14,
            32, 27, 3, 9,
            19, 13, 30, 6,
            22, 11, 4, 25
        };

        // S-boxes
        private static readonly int[,] SBox1 =
        {
            { 14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7 },
            { 0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8 },
            { 4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0 },
            { 15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13 }
        };

        private static readonly int[,] SBox2 =
        {
            { 15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10 },
            { 3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5 },
            { 0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15 },
            { 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9 }
        };

        private static readonly int[,] SBox3 =
        {
            { 10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8 },
            { 13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1 },
            { 13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7 },
            { 1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12 }
        };

        private static readonly int[,] SBox4 =
        {
            { 7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15 },
            { 13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9 },
            { 10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4 },
            { 3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14 }
        };

        private static readonly int[,] SBox5 =
        {
            { 2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9 },
            { 14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6 },
            { 4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14 },
            { 11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3 }
        };

        private static readonly int[,] SBox6 =
        {
            { 12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11 },
            { 10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8 },
            { 9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6 },
            { 4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13 }
        };

        private static readonly int[,] SBox7 =
        {
            { 4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1 },
            { 13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6 },
            { 1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2 },
            { 6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12 }
        };

        private static readonly int[,] SBox8 =
        {
            { 13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7 },
            { 1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2 },
            { 7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8 },
            { 2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11 }
        };

        private static readonly int[][,] SBoxes =
        {
            SBox1,
            SBox2,
            SBox3,
            SBox4,
            SBox5,
            SBox6,
            SBox7,
            SBox8
        };

        public static BitArray EncryptBlock(BitArray plainBlock64, List<BitArray> roundKeys)
        {
            // Step 1: Initial Permutation
            BitArray permuted = Permute(plainBlock64, IP);

            // Step 2: Split into L and R (32 bits each)
            BitArray L = new(32);
            BitArray R = new(32);
            for (int i = 0; i < 32; i++)
            {
                L[i] = permuted[i];
                R[i] = permuted[i + 32];
            }

            // Step 3: 16 Feistel rounds
            for (int round = 0; round < 16; round++)
            {
                BitArray previousR = R;
                R = L.Xor(F(R, roundKeys[round]));
                L = previousR;
            }

            // Step 4: Combine R and L (note the swap!)
            BitArray preOutput = new(64);
            for (int i = 0; i < 32; i++)
            {
                preOutput[i] = R[i];
                preOutput[i + 32] = L[i];
            }

            // Step 5: Final Permutation
            return Permute(preOutput, FP);
        }

        public static BitArray DecryptBlock(BitArray cipherBlock64, List<BitArray> roundKeys)
        {
            // Decryption is the same as encryption but with reversed round keys
            var reversedKeys = new List<BitArray>(roundKeys);
            reversedKeys.Reverse();
            return EncryptBlock(cipherBlock64, reversedKeys);
        }

        private static BitArray F(BitArray right32, BitArray roundKey48)
        {
            // Step 1: Expand 32-bit R to 48 bits
            BitArray expandedRight = Permute(right32, EBox);

            // Step 2: XOR with round key
            expandedRight.Xor(roundKey48);

            // Step 3: S-box substitution (48 → 32 bits)
            BitArray sBoxOutput = SBoxSubstitution(expandedRight);

            // Step 4: P-box permutation
            return Permute(sBoxOutput, PBox);
        }

        private static BitArray SBoxSubstitution(BitArray input48)
        {
            BitArray output32 = new(32);
            int inputIndex = 0;
            int outputIndex = 0;

            // Process each 6-bit group through corresponding S-box
            for (int sboxIndex = 0; sboxIndex < 8; sboxIndex++)
            {
                // Extract 6 bits for current S-box
                int row = (input48[inputIndex] ? 2 : 0) | (input48[inputIndex + 5] ? 1 : 0);
                int col =
                    (input48[inputIndex + 1] ? 8 : 0)
                    | (input48[inputIndex + 2] ? 4 : 0)
                    | (input48[inputIndex + 3] ? 2 : 0)
                    | (input48[inputIndex + 4] ? 1 : 0);

                // Get 4-bit output from S-box
                int value = SBoxes[sboxIndex][row, col];

                // Convert to bits and add to output
                for (int bit = 3; bit >= 0; bit--)
                    output32[outputIndex + bit] = (value & (1 << bit)) != 0;

                inputIndex += 6;
                outputIndex += 4;
            }

            return output32;
        }

        private static BitArray Permute(BitArray input, int[] table)
        {
            BitArray output = new(table.Length);
            for (int i = 0; i < table.Length; i++)
                output[i] = input[table[i] - 1]; // DES tables are 1-indexed
            return output;
        }
    }
}
