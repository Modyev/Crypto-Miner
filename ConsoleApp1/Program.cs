using System;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

class BitcoinMiner
{
    static SHA256 sha256 = SHA256.Create();

    static string ComputeSHA256(string data)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        byte[] hash = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    static bool IsValidHash(string hash, BigInteger difficultyTarget)
    {
        BigInteger hashValue = BigInteger.Parse("0" + hash, NumberStyles.HexNumber);
        return hashValue <= difficultyTarget;
    }

    static void MineBlock(string previousHash, string merkleRoot, BigInteger difficultyTarget)
    {
        long nonce = 0;
        // getting the time before starting the mining process so we can keep track how much time it took to mine the block
        DateTime startTime = DateTime.Now;

        Console.ForegroundColor = ConsoleColor.White;
        while (true)
        {
            string blockHeader = previousHash + merkleRoot + DateTimeOffset.UtcNow.ToUnixTimeSeconds() + nonce;
            string hash = ComputeSHA256(ComputeSHA256(blockHeader));

            if (IsValidHash(hash, difficultyTarget))
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;

                Console.WriteLine("\n✅ Block mined!");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Hash: " + hash);
                Console.WriteLine("Nonce: " + nonce);
                Console.WriteLine($"After {nonce / 1000}k Tries");
                Console.WriteLine("Time Taken: " + (DateTime.Now - startTime));
                Console.ForegroundColor= ConsoleColor.White;
                break;
            }


            // Used to print progress, but commented it as it consumes a lot of resources and slows down the mining process

            //if (nonce % 1000000 == 0)
            //    Console.Write($"\rNonce: {nonce / 1000}k");

            nonce++;
        }
    }

    public static void Main()
    {
        //Changing Output Encoding so I can print emojis
        Console.OutputEncoding = Encoding.UTF8;

        string previousHash = "0000000000000000000a8c000000000000000000000000000000000000000000";
        // merkle root is like a hash of all the transactions ids, used by the network to verify that all the transactions are present in the block
        string merkleRoot = "4d5ebaa6c3dd8b7b71e2e7497d16794ff1ee2ea36fef64b75f92c4cbfad1a3c5";

        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write("How many leading zeros would you like in the difficulty? (the greater the harder): ");
        Console.ForegroundColor = ConsoleColor.White;
        int LeadingZeros = Convert.ToInt32(Console.ReadLine());

        BigInteger difficultyTarget = AdjustDifficulty(LeadingZeros);
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n⛏️ Starting CPU Mining...\n");
        MineBlock(previousHash, merkleRoot, difficultyTarget);
    }
    static BigInteger AdjustDifficulty(int leadingZeros)
    {
        if (leadingZeros > 64) leadingZeros = 64;

        string r = new string('0', leadingZeros);
        r = r.PadRight(64, 'F');

        return BigInteger.Parse("0" + r, NumberStyles.HexNumber);
    }
}
