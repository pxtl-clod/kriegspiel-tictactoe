namespace KriegspielTicTacToe;

using OneOf;
using OneOf.Types;

/// <summary>
/// Reads command keys from keyboard.
/// </summary>
internal static class InputUtility {
    public static void PauseAndPressAnyKey(string? prompt = null) {
        if(prompt != null && !prompt.EndsWith(' ')) {
            prompt = $"{prompt} ";
        }
        if (Console.IsInputRedirected) {
            Console.Out.WriteLine($"{prompt}Input any line to continue...");
            Console.ReadLine();
        } else {
            Console.Out.WriteLine($"{prompt}Press any key to continue...");
            Console.ReadKey(intercept: true);
        }
    }
    public static OneOf<char, int, Unknown> ReadCommandKeys(string prompt, int codeLength)
        => ReadCommandKeys(prompt, codeLength, new[]{'r'});

    public static OneOf<char, int, Unknown> ReadCommandKeys(string prompt, int codeLength, char[] validCommandChars) {
        if (validCommandChars == null) {
            validCommandChars = new[]{'r'};
        }
        Console.Out.WriteLine(prompt);
               
        if (Console.IsInputRedirected) {
            // if we lack access to ReadKey
            while (true) {
                var inputStr = Console.ReadLine()!;
                var isNumeric = int.TryParse(inputStr, out var numericCode);

                if (isNumeric) {
                    if (inputStr.Length > codeLength) {
                        Console.Out.WriteLine("Too long.");
                        continue;
                    } else {
                        Console.Out.WriteLine();
                        return numericCode;
                    }
                } else if (inputStr.Length != 1) {
                    Console.Out.WriteLine("Command must be one character long.");
                } else if(validCommandChars.Contains(inputStr.Single())){
                    Console.Out.WriteLine();
                    return inputStr.Single();
                } else {
                    Console.Out.WriteLine();
                    Console.Out.WriteLine("Invalid command.");
                    return new Unknown();
                }
            }
        } else {
            // if we have access to ReadKey
            var isNumeric = false;
            var numericCode = -1;
            var sb = new System.Text.StringBuilder();
            do {
                var key = Console.ReadKey();
                if(key.KeyChar >= '0' && key.KeyChar <= '9') {
                    sb.Append(key.KeyChar);
                } else if(validCommandChars.Contains(key.KeyChar)){
                    Console.Out.WriteLine();
                    return key.KeyChar;
                } else {
                    Console.Out.WriteLine();
                    Console.Out.WriteLine("Invalid command.");
                    return new Unknown();
                }
                isNumeric = int.TryParse(sb.ToString(), out numericCode);
            } while (sb.Length < codeLength && isNumeric);
            Console.Out.WriteLine();
            return numericCode;
        }
        
    }
}
