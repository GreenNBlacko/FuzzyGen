/*                                                            */
/*     Text file generator for the fuzzy search algorithm     */
/*                                                            */

using System.Diagnostics.Metrics;
using System.Text;

var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

var rand = new Random((int)DateTime.Now.Ticks);

Console.Write("Provide x: "); var x = int.Parse(Console.ReadLine()); // x - min phrase length [x; inf)
Console.Write("Provide y: "); var y = int.Parse(Console.ReadLine()); // y - min freq [y; inf)

Console.Write("Provide n: "); var n = int.Parse(Console.ReadLine()); // n - phrase count
Console.Write("Provide m: "); var m = int.Parse(Console.ReadLine()); // m - final element count

Console.WriteLine();

var outStr = "";

List<string> phrases = new();

for (int i = 0; i < n; i++) {                                        // Generating the phrases
	var phrase = "";

	for (int o = 0; o < x + rand.Next(0, (x + 3) % m); o++) {
		phrase += chars[rand.Next(chars.Length)];
	}

	phrases.Add(phrase);

	Console.WriteLine("Phrase '" + phrase + "' added to the list");
}

StringBuilder stringBuilder = new StringBuilder(m);

for (int i = 0; i < m; i++) {                                        // Generating the output string
	stringBuilder.Append(chars[rand.Next(chars.Length)]);
}

List<int> keys = new();
var counts = new Dictionary<string, int>();

for (int i = 0; i < n; i++) {                                        // adding the phrases into the text
	var rep = rand.Next(y, y + 10);                                   // how much will a certain phrase repeat itself

	for (int o = 0; o < rep; o++) {                                  // placing the phrase with the frequency of (y; y+2] 
		var pos = rand.Next(0, m - 1 - x);                           // the index to place the current phrase

		var isKey = keys.Count > 0;                                  // does a phrase already exist in this position?

		while (isKey) {                                              // making sure the phrase does not intercept another
			foreach (var key in keys) {
				isKey = pos + x >= key && pos <= key + x;            // checking if index of phrase matches

				if (isKey) break;                                    // tossing the index if it's in the range of a pre-existing key
			}

			if (!isKey)
				break;

			pos = rand.Next(0, m - 1 - x);
		}

		stringBuilder.Remove(pos, phrases[i].Length);
		stringBuilder.Insert(pos, phrases[i]);

		keys.Add(pos);
	}

	counts[phrases[i]] = rep;
}

outStr = stringBuilder.ToString();

keys.Sort();

Console.WriteLine("Input string:\n" + outStr + "\nExpected output(inaccurate):");

foreach (var (name, count) in counts) {
	Console.WriteLine(string.Format("x{0} {1}", count, name));
}

/*									END OF TEST STRING GENERATION									*/

Console.WriteLine();

/* 										START OF RESULT SEARCH										*/

Dictionary<string, int> matches = new();
List<string> valuesToKeep = new();

for (int size = outStr.Length - 1; size > x; size--) {
	StringBuilder bufferBuilder = new(size);

	int tempCount = matches.Count;

	for (int i = 0; i < outStr.Length - size; i++) {
		bufferBuilder.Clear().Append(outStr, i, x);
		var buffer = bufferBuilder.ToString();

		if (!matches.TryGetValue(buffer, out var count))
			count = 0;

		matches[buffer] = count + 1;

		if (matches[buffer] >= y && !valuesToKeep.Contains(buffer)) {
			bool write = true;

			for(int o = 0; o < valuesToKeep.Count && write; o++) {
				if (valuesToKeep[o].Contains(buffer))
					write = false;
			}

			if(write)
				valuesToKeep.Add(buffer);
		}
			
	}

	if (tempCount == matches.Count && valuesToKeep.Count != 0)
		break;
}

valuesToKeep.Sort();

foreach (var match in valuesToKeep) {
	Console.WriteLine("x" + matches[match] + " " + match);
}