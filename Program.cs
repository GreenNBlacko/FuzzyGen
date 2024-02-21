using ClickableTransparentOverlay;
using ImGuiNET;
using System.Collections.Concurrent;
using System.Drawing;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace Fuzzy;

public class FuzzySearch : Overlay {
	int x = 10;
	int y = 10;

	int x1 = -1;
	int x2 = -1;
	int s = 1;
	bool preferSizeOverCount = false;
	string r = string.Empty;

	bool skipGen = false;
	int n = 3;
	int m = 100000;
	string filePath = string.Empty;

	private bool f_Limits = true;
	private bool f_Settings = true;
	private bool f_Data = true;

	private bool searching = false;
	private string currentlyProcessing = string.Empty;
	private int currentStep = 0;
	private List<Tuple<string, int>> Results = new List<Tuple<string, int>>();
	private bool resultFound = false;

	public static void Main (string[] args) {
		Console.WriteLine("program is starting...");

		var gui = new FuzzySearch();

		gui.Start().Wait();
	}

	protected override Task PostInitialized () {
		var colors = ImGui.GetStyle().Colors;

		colors[(int)ImGuiCol.WindowBg] = new Vector4(0.1f, 0.1f, 0.13f, 1.0f);
		colors[(int)ImGuiCol.MenuBarBg] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);

		// Border
		colors[(int)ImGuiCol.Border] = new Vector4(0.44f, 0.37f, 0.61f, 0.29f);
		colors[(int)ImGuiCol.BorderShadow] = new Vector4(0.0f, 0.0f, 0.0f, 0.24f);

		// Text
		colors[(int)ImGuiCol.Text] = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
		colors[(int)ImGuiCol.TextDisabled] = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);

		// Headers
		colors[(int)ImGuiCol.Header] = new Vector4(0.13f, 0.13f, 0.17f, 1.0f);
		colors[(int)ImGuiCol.HeaderHovered] = new Vector4(0.19f, 0.2f, 0.25f, 1.0f);
		colors[(int)ImGuiCol.HeaderActive] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);

		// Buttons
		colors[(int)ImGuiCol.Button] = new Vector4(0.13f, 0.13f, 0.17f, 1.0f);
		colors[(int)ImGuiCol.ButtonHovered] = new Vector4(0.19f, 0.2f, 0.25f, 1.0f);
		colors[(int)ImGuiCol.ButtonActive] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);
		colors[(int)ImGuiCol.CheckMark] = new Vector4(0.74f, 0.58f, 0.98f, 1.0f);

		// Popups
		colors[(int)ImGuiCol.PopupBg] = new Vector4(0.1f, 0.1f, 0.13f, 0.92f);

		// Slider
		colors[(int)ImGuiCol.SliderGrab] = new Vector4(0.44f, 0.37f, 0.61f, 0.54f);
		colors[(int)ImGuiCol.SliderGrabActive] = new Vector4(0.74f, 0.58f, 0.98f, 0.54f);

		// Frame BG
		colors[(int)ImGuiCol.FrameBg] = new Vector4(0.13f, 0.13f, 0.17f, 1.0f);
		colors[(int)ImGuiCol.FrameBgHovered] = new Vector4(0.19f, 0.2f, 0.25f, 1.0f);
		colors[(int)ImGuiCol.FrameBgActive] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);
		// Tabs
		colors[(int)ImGuiCol.Tab] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);
		colors[(int)ImGuiCol.TabHovered] = new Vector4(0.24f, 0.24f, 0.32f, 1.0f);
		colors[(int)ImGuiCol.TabActive] = new Vector4(0.2f, 0.22f, 0.27f, 1.0f);
		colors[(int)ImGuiCol.TabUnfocused] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);
		colors[(int)ImGuiCol.TabUnfocusedActive] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);

		// Title
		colors[(int)ImGuiCol.TitleBg] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);
		colors[(int)ImGuiCol.TitleBgActive] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);
		colors[(int)ImGuiCol.TitleBgCollapsed] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);

		// Scrollbar
		colors[(int)ImGuiCol.ScrollbarBg] = new Vector4(0.1f, 0.1f, 0.13f, 1.0f);
		colors[(int)ImGuiCol.ScrollbarGrab] = new Vector4(0.16f, 0.16f, 0.21f, 1.0f);
		colors[(int)ImGuiCol.ScrollbarGrabHovered] = new Vector4(0.19f, 0.2f, 0.25f, 1.0f);
		colors[(int)ImGuiCol.ScrollbarGrabActive] = new Vector4(0.24f, 0.24f, 0.32f, 1.0f);
		// Seperator
		colors[(int)ImGuiCol.Separator] = new Vector4(0.44f, 0.37f, 0.61f, 1.0f);
		colors[(int)ImGuiCol.SeparatorHovered] = new Vector4(0.74f, 0.58f, 0.98f, 1.0f);
		colors[(int)ImGuiCol.SeparatorActive] = new Vector4(0.84f, 0.58f, 1.0f, 1.0f);

		// Resize Grip
		colors[(int)ImGuiCol.ResizeGrip] = new Vector4(0.44f, 0.37f, 0.61f, 0.29f);
		colors[(int)ImGuiCol.ResizeGripHovered] = new Vector4(0.74f, 0.58f, 0.98f, 0.29f);
		colors[(int)ImGuiCol.ResizeGripActive] = new Vector4(0.84f, 0.58f, 1.0f, 0.29f);

		// Docking
		//colors[(int)ImGuiCol.DockingPreview] = new Vector4{ 0.44f, 0.37f, 0.61f, 1.0f };

		var style = ImGui.GetStyle();
		style.TabRounding = 4;
		style.ScrollbarRounding = 9;
		style.WindowRounding = 7;
		style.GrabRounding = 3;
		style.FrameRounding = 3;
		style.PopupRounding = 4;
		style.ChildRounding = 4;

		return base.PostInitialized();
	}

	protected override void Render () {
		if (searching)
			goto search;

		ImGui.Begin("Fuzzy Search");

		ImGui.SetWindowFontScale(1.3f);

		ImGui.BeginChild("Limits Window", GetFoldoutSize(2, f_Limits));

		FoldoutHeader("Limits", ref f_Limits, delegate {
			ImGui.InputInt("Min phrase length", ref x);
			ImGui.InputInt("Min frequency", ref y);
		});

		ImGui.EndChild();

		ImGui.BeginChild("Settings Window", GetFoldoutSize(5, f_Settings));

		FoldoutHeader("Settings", ref f_Settings, delegate {
			ImGui.InputInt("Size interval start", ref x1);
			ImGui.InputInt("Size interval end", ref x2);
			ImGui.InputInt("Step size", ref s);
			ImGui.Checkbox("Prefer size over count", ref preferSizeOverCount);
			ImGui.InputText("Match pattern", ref r, 1000);
		});

		ImGui.EndChild();

		ImGui.BeginChild("Data Window", GetFoldoutSize(1 + (skipGen ? 1 : 2), f_Data));

		FoldoutHeader("Data", ref f_Data, delegate {
			ImGui.Checkbox("Use a file instead of generating data", ref skipGen);

			if (skipGen) {
				ImGui.InputText("File path", ref filePath, 256);
			} else {
				ImGui.InputInt("Phrase count", ref n);
				ImGui.InputInt("Total element count", ref m);
			}
		});

		ImGui.EndChild();

		if (skipGen && !File.Exists(filePath))
			ImGui.TextColored(new Vector4(Color.Red.R, Color.Red.G, Color.Red.B, Color.Red.A), "Please select a valid path");

		if ((skipGen && File.Exists(filePath) || !skipGen) && ImGui.Button("Start search", GetFoldoutSize(0, false))) {
			searching = true;

			currentStep = 0;
			Results.Clear();
			resultFound = false;

			Task.Run(() => GetResults());
		}

		if (ImGui.Button("Exit", GetFoldoutSize(0, false)))
			Close();

		ImGui.End();

		if (!searching)
			return;

		search:

		ImGui.Begin("Search Results");

		ImGui.SetWindowFontScale(1.3f);

		ImGui.Dummy(new Vector2(ImGui.GetWindowWidth() - 15, 0));

		if (!resultFound) {
			ImGui.Text("Step: " + currentStep);
			ImGui.Text("Currently processing: " + currentlyProcessing);
		} else {
			if (ImGui.BeginTable("Results", 2)) {
				ImGui.TableNextRow();

				ImGui.TableSetColumnIndex(0);
				ImGui.Text("Match");

				ImGui.TableSetColumnIndex(1);
				ImGui.Text("Count");

				for (int row = 0; row < Results.Count; row++) {
					ImGui.TableNextRow();

					ImGui.TableSetColumnIndex(0);
					ImGui.Text(Results[row].Item1);

					ImGui.TableSetColumnIndex(1);
					ImGui.Text(Results[row].Item2.ToString());
				}

			}

			ImGui.EndTable();

			if (ImGui.Button("Close", new Vector2(ImGui.GetWindowWidth() - 15, 23))) {
				searching = false;
				currentStep = 0;
				Results.Clear();
				resultFound = false;
			}
		}

		ImGui.End();
	}

	private Vector2 GetFoldoutSize (int ChildCount, bool foldout) {
		return new Vector2(ImGui.GetWindowWidth() - 15, foldout ? 23 + 26f * ChildCount : 23);
	}

	private void FoldoutHeader (string label, ref bool foldout, Action content) {
		ImGui.SetNextItemOpen(foldout);
		foldout = ImGui.CollapsingHeader(label);

		if (foldout) {
			ImGui.Indent();
			content.Invoke();
			ImGui.Unindent();
		}
	}

	public void GetResults () {
		var outStr = "";

		if (skipGen)
			outStr = File.ReadAllText(filePath);
		else
			outStr = generateTestString();

		Console.WriteLine();

		/* 										START OF RESULT SEARCH										*/


		ConcurrentDictionary<string, int> matches = new();

		for (int size = Math.Max(x, (x1 != -1 ? x1 : x)); size <= Math.Min(outStr.Length - x, (x2 != -1 ? x2 : outStr.Length - x)); size += s) {
			ConcurrentDictionary<string, byte> processedSubstrings = new();

			Console.WriteLine("Step: " + size);

			currentStep = size;

			Parallel.For(0, outStr.Length - size, i => {
				string buffer = outStr.Substring(i, size);

				currentlyProcessing = buffer;

				if (processedSubstrings.Keys.Contains(buffer))
					return;

				if (!string.IsNullOrEmpty(r) && !Regex.IsMatch(buffer, r))
					return;

				int count = GetCount(outStr, buffer);

				if (count < y)
					return;

				var currentEntries = matches;

				var overlappingEntries = new List<KeyValuePair<string, int>>();

				foreach (var e in currentEntries) {
					if (e.Key != null && e.Key != buffer && (e.Key.Contains(buffer) || buffer.Contains(e.Key)))
						overlappingEntries.Add(e);
				}

				foreach (var overlappingEntry in overlappingEntries) {
					if (preferSizeOverCount) { // Prefer size over count
						if (buffer.Length > overlappingEntry.Key.Length) {
							// Replace smaller entry with larger entry
							matches.TryRemove(overlappingEntry);
							processedSubstrings.Remove(overlappingEntry.Key, out byte _);
						} else if (buffer.Length == overlappingEntry.Key.Length && count > matches[overlappingEntry.Key]) {
							// Replace equal size entry if current entry has more matches
							matches.TryRemove(overlappingEntry);
							processedSubstrings.Remove(overlappingEntry.Key, out byte _);
						} else {
							// Skip current entry as it is smaller or equal to existing entry
							continue;
						}
					} else { // Prefer count over size
						if (count > overlappingEntry.Value || count >= overlappingEntry.Value && buffer.Length > overlappingEntry.Key.Length) {
							// Replace existing entry if current entry has more matches or is longer
							matches.TryRemove(overlappingEntry);
							processedSubstrings.Remove(overlappingEntry.Key, out byte _);
						} else {
							// Skip current entry as it has fewer matches than existing entry
							continue;
						}
					}
				}

				// Add the current substring to matches dictionary
				matches[buffer] = count;
				processedSubstrings[buffer] = 0;
			});
		}

		foreach (var match in matches) {
			Results.Add(new(match.Key, match.Value));

			Console.WriteLine($"Match: {match.Key}, Count: {match.Value}");
		}

		resultFound = true;
	}

	string generateTestString() {
		var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.,;:<>/?\\=+";

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
		}

		return stringBuilder.ToString();
	}

	int GetCount (string text, string subStr) {
		int count = 0;
		int index = 0;

		index = text.IndexOf(subStr, index);

		while (index != -1) {
			count++;
			index = text.IndexOf(subStr, index + 1);
		}


		return count;
	}
}


/*                                                            */
/*     Text file generator for the fuzzy search algorithm     */
/*                                                            */


/*


*/