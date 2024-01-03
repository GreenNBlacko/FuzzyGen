/*                                                            */
/*     Text file generator for the fuzzy search algorithm     */
/*                                                            */

var rand = new Random();

Console.Write("Provide x: "); var x = int.Parse(Console.ReadLine()); // x - phrase length
Console.Write("Provide y: "); var y = int.Parse(Console.ReadLine()); // y - min freq (y; inf)

var outStr = "";

List<string> phrases = new();

Console.Write("Provide n: "); var n = int.Parse(Console.ReadLine()); // n - phrase count
Console.Write("Provide m: "); var m = int.Parse(Console.ReadLine()); // m - final element count

Console.WriteLine();

for (int i = 0; i < n; i++) {                                        // Generating the phrases
    var phrase = "";

    for (int o = 0; o < x; o++) {
        phrase += (char)(rand.Next(0, 26) + 'A');
    }

    phrases.Add(phrase);

    Console.WriteLine("Phrase '" + phrase + "' added to the list");
}

for (int i = 0; i < m; i++) {                                        // Generating the output string
    outStr += (char)(rand.Next(0, 26) + 'A');
}

List<int> keys = new();

for (int i = 0; i < n; i++) {                                       // adding the phrases into the text
    var rep = rand.Next(y, y + 2);                                  // how much will a certain phrase repeat itself

    for(int o = 0; o < rep; o++) {                                  // placing the phrase with the frequency of (y; y+2] 
        var pos = rand.Next(0, n - 1);                                  // the index to place the current phrase

        var isKey = true;                                               // does a phrase already exist in this position?

        while (isKey) {                                                 // making sure the phrase does not intercept another
            foreach (var key in keys) {
                isKey = pos >= key && pos <= key + x;                   // checking if index of phrase matches

                if (isKey) break;
            }

            pos = rand.Next(0, n - 1);
        }

        outStr.Insert(pos, phrases[i]);

        keys.Add(pos);
    }
}