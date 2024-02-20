/*                                                            */
/*     Text file generator for the fuzzy search algorithm     */
/*                                                            */

using System.Text;
using System.Text.RegularExpressions;

var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.,;:<>/?\\=+";

Console.Write("Provide x: ");	var x = int.Parse(Console.ReadLine() ?? "0");			// x - min phrase length [x; inf)
Console.Write("Provide x1: ");	var x1 = int.Parse(Console.ReadLine() ?? "0");			// x1 - max search phrase length [x; x1)
Console.Write("Provide y: ");	var y = int.Parse(Console.ReadLine() ?? "0");			// y - min freq [y; inf)

Console.Write("Provide r: ");	var r = Console.ReadLine() ?? "";						// r - phrase regex pattern;
Console.Write("Provide s: ");	var s = int.Parse(Console.ReadLine() ?? "0");           // s - step
Console.Write("Prefer size over count?: "); var preferSizeOverCount = (Console.ReadLine() ?? "0") == "1";

var outStr = "";

Console.Write("Skip generation? "); 
if(int.Parse(Console.ReadLine() ?? "0") == 1) {
    Console.Write("Enter path to data file: "); var f = Console.ReadLine();

	while(!File.Exists(f)) {
        Console.Write("Path does not exist! Try again.\nEnter path to data file: "); f = Console.ReadLine();
    }

	outStr = File.ReadAllText(f);

	goto search;
}

Console.Write("Provide n: "); var n = int.Parse(Console.ReadLine() ?? "0"); // n - phrase count
Console.Write("Provide m: "); var m = int.Parse(Console.ReadLine() ?? "0"); // m - final element count

Console.WriteLine();

List<string> phrases = new();

for (int i = 0; i < n; i++) {
    var rand = new Random((int)DateTime.Now.Ticks);
    // Generating the phrases
    var phrase = "";

	for (int o = 0; o < x + rand.Next(0, (x + 3) % m); o++) {
		phrase += chars[rand.Next(chars.Length)];
	}

	phrases.Add(phrase);

	Console.WriteLine("Phrase '" + phrase + "' added to the list");
}

StringBuilder stringBuilder = new StringBuilder(m);

for (int i = 0; i < m; i++) {                                        // Generating the output string
    var rand = new Random((int)DateTime.Now.Ticks);

    stringBuilder.Append(chars[rand.Next(chars.Length)]);
}

List<int> keys = new();
var counts = new Dictionary<string, int>();

for (int i = 0; i < n; i++) {
    var rand = new Random((int)DateTime.Now.Ticks);

    // adding the phrases into the text
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

search:
Console.WriteLine();

/* 										START OF RESULT SEARCH										*/

if (x1 == -1)
	x1 = outStr.Length - x;

Dictionary<string, int> matches = new();

for(int size = x; size <= x1 && size < outStr.Length - x; size += s) {
	HashSet<string> processedSubstrings = new();

	Console.WriteLine("Step: " + size);

	Parallel.For(0, outStr.Length - size, i => {
		string buffer = outStr.Substring(i, size);

		if (processedSubstrings.Contains(buffer))
			return;

		if (!string.IsNullOrEmpty(r) && !Regex.IsMatch(buffer, r))
			return;

		int count = GetCount(outStr, buffer);

		if (count < y)
			return;

		var overlappingEntries = matches.Keys.Where(e => e != buffer && (e.Contains(buffer) || buffer.Contains(e))).ToList();
		foreach (var overlappingEntry in overlappingEntries) {
			if (preferSizeOverCount) { // Prefer size over count
				if (buffer.Length > overlappingEntry.Length) {
					// Replace smaller entry with larger entry
					matches.Remove(overlappingEntry);
					processedSubstrings.Remove(overlappingEntry);
				} else if (buffer.Length == overlappingEntry.Length && count > matches[overlappingEntry]) {
					// Replace equal size entry if current entry has more matches
					matches.Remove(overlappingEntry);
					processedSubstrings.Remove(overlappingEntry);
				} else {
					// Skip current entry as it is smaller or equal to existing entry
					continue;
				}
			} else { // Prefer count over size
				if (count > matches[overlappingEntry]) {
					// Replace existing entry if current entry has more matches
					matches.Remove(overlappingEntry);
					processedSubstrings.Remove(overlappingEntry);
				} else {
					// Skip current entry as it has fewer matches than existing entry
					continue;
				}
			}
		}

		// Add the current substring to matches dictionary
		matches[buffer] = count;
		processedSubstrings.Add(buffer);
	});
}

foreach (var match in matches) {
	Console.WriteLine($"Match: {match.Key}, Count: {match.Value}");
}

int GetCount(string text, string subStr) {
	int count = 0;
	int index = 0;

	index = text.IndexOf(subStr, index);

	while (index != -1) {
		count++;
		index = text.IndexOf(subStr, index + 1);
	}
		

	return count;
}