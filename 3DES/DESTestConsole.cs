using System.Text;
using DES.ModeHandlers;

namespace DES
{
    public class DESTestConsole
    {
        public static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            Console.WriteLine("DES Algorithm Test Program");
            Console.WriteLine("========================\n");

            // دریافت متن از کاربر
            Console.WriteLine("Please enter the text to encrypt:");
            string plaintext = Console.ReadLine() ?? "Default text for testing";

            // دریافت کلید از کاربر
            Console.WriteLine("\nPlease enter the encryption key one (8 characters):");
            string keyInput1 = Console.ReadLine() ?? "TestKey1";
            byte[] key1 = Encoding.UTF8.GetBytes(keyInput1.PadRight(8)[..8]);

            Console.WriteLine("\nPlease enter the encryption key two (8 characters):");
            string keyInput2 = Console.ReadLine() ?? "TestKey1";
            byte[] key2 = Encoding.UTF8.GetBytes(keyInput2.PadRight(8)[..8]);

            Console.WriteLine("\nPlease enter the encryption key three (8 characters):");
            string keyInput3 = Console.ReadLine() ?? "TestKey1";
            byte[] key3 = Encoding.UTF8.GetBytes(keyInput3.PadRight(8)[..8]);

            // دریافت بردار اولیه
            Console.WriteLine(
                "\nPlease enter the initial vector (8 characters) or leave it blank for default value:"
            );

            string ivInput = Console.ReadLine() ?? "12345678";

            byte[] iv = Encoding.UTF8.GetBytes(ivInput.PadRight(8)[..8]);

            // انتخاب حالت رمزنگاری
            Console.WriteLine("\nPlease select the encryption mode:");
            Console.WriteLine("1. ECB (Electronic Codebook)");
            Console.WriteLine("2. CBC (Cipher Block Chaining)");
            Console.WriteLine("3. CFB (Cipher Feedback)");
            Console.WriteLine("4. OFB (Output Feedback)");
            Console.WriteLine("5. CTR (Counter)");

            string modeInput = Console.ReadLine() ?? "1";
            if (!int.TryParse(modeInput, out int mode) || mode < 1 || mode > 5)
            {
                mode = 1; // ECB به عنوان پیش‌فرض
                Console.WriteLine("Invalid mode. Using ECB mode.");
            }

            // تبدیل متن به بایت
            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);

            // اضافه کردن پدینگ برای اطمینان از اینکه طول داده مضربی از 8 است
            int paddingLength = 8 - (plaintextBytes.Length % 8);
            if (paddingLength < 8)
            {
                byte[] paddedPlaintext = new byte[plaintextBytes.Length + paddingLength];
                Array.Copy(plaintextBytes, paddedPlaintext, plaintextBytes.Length);
                for (int i = plaintextBytes.Length; i < paddedPlaintext.Length; i++)
                {
                    paddedPlaintext[i] = (byte)paddingLength;
                }
                plaintextBytes = paddedPlaintext;
            }

            // ایجاد هندلر مناسب بر اساس حالت انتخاب شده
            OperationModeHandler handler1;
            OperationModeHandler handler2;
            OperationModeHandler handler3;

            switch (mode)
            {
                case 2:
                    handler1 = new CBCModeHandler(key1, iv);
                    handler2 = new CBCModeHandler(key2, iv);
                    handler3 = new CBCModeHandler(key3, iv);
                    Console.WriteLine("\nUsing CBC mode");
                    break;
                case 3:
                    handler1 = new CFBModeHandler(key1, iv);
                    handler2 = new CFBModeHandler(key2, iv);
                    handler3 = new CFBModeHandler(key3, iv);
                    Console.WriteLine("\nUsing CFB mode");
                    break;
                case 4:
                    handler1 = new OFBModeHandler(key1, iv);
                    handler2 = new OFBModeHandler(key2, iv);
                    handler3 = new OFBModeHandler(key3, iv);
                    Console.WriteLine("\nUsing OFB mode");
                    break;
                case 5:
                    handler1 = new CTRModeHandler(key1, iv);
                    handler2 = new CTRModeHandler(key2, iv);
                    handler3 = new CTRModeHandler(key3, iv);
                    Console.WriteLine("\nUsing CTR mode");
                    break;
                default:
                    handler1 = new ECBModeHandler(key1);
                    handler2 = new ECBModeHandler(key2);
                    handler3 = new ECBModeHandler(key3);
                    Console.WriteLine("\nUsing ECB mode");
                    break;
            }

            try
            {
                // رمزنگاری                           
                Console.WriteLine("\nEncrypting...");
                var  ciphertext = TDESEncryption(handler1, handler2, handler3, plaintextBytes);
                string base64Ciphertext = Encoding.UTF8.GetString(ciphertext);
                Console.WriteLine($"Encrypted text (Base64): {base64Ciphertext}");

                // رمزگشایی
                Console.WriteLine("\nDecrypting...");

                byte[] decryptedBytes = TDESDecryption(handler1, handler2, handler3, ciphertext);

                // حذف پدینگ
                int lastByte = decryptedBytes[^1];
                if (lastByte > 0 && lastByte <= 8)
                {
                    bool validPadding = true;
                    for (int i = decryptedBytes.Length - lastByte; i < decryptedBytes.Length; i++)
                    {
                        if (decryptedBytes[i] != lastByte)
                        {
                            validPadding = false;
                            break;
                        }
                    }

                    if (validPadding)
                    {
                        byte[] unpaddedBytes = new byte[decryptedBytes.Length - lastByte];
                        Array.Copy(decryptedBytes, unpaddedBytes, unpaddedBytes.Length);
                        decryptedBytes = unpaddedBytes;
                    }
                }

                string decryptedText = Encoding.UTF8.GetString(decryptedBytes);
                Console.WriteLine($"Decrypted text: {decryptedText}");

                // بررسی صحت رمزگشایی
                if (decryptedText == plaintext)
                {
                    Console.WriteLine("\n✓ Encryption and decryption completed successfully!");
                }
                else
                {
                    Console.WriteLine(
                        "\n✗ خطا در رمزگشایی! متن رمزگشایی شده با متن اصلی مطابقت ندارد."
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        public static byte[] TDESEncryption(OperationModeHandler handler1,
        OperationModeHandler handler2,
        OperationModeHandler handler3,
        byte[] plaintextBytes)
        {
            Console.WriteLine("\nEncrypting...");
            byte[] ciphertext1 = handler1.Encrypt(plaintextBytes);
            byte[] ciphertext2 = handler2.Decrypt(ciphertext1);
            byte[] ciphertext3 = handler3.Encrypt(ciphertext2);
            return ciphertext3;
        }

        public static byte[] TDESDecryption(OperationModeHandler handler1,
        OperationModeHandler handler2,
        OperationModeHandler handler3,
        byte[] ciphertext)
        {
            Console.WriteLine("\nEncrypting...");
            byte[] ciphertext2 = handler3.Decrypt(ciphertext);
            byte[] ciphertext1 = handler2.Encrypt(ciphertext2);
            byte[] plaintextBytes = handler1.Decrypt(ciphertext1);
            return plaintextBytes;
        }
    }
}
