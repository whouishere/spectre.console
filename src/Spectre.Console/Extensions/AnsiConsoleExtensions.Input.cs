namespace Spectre.Console;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Contains extension methods for <see cref="IAnsiConsole"/>.
/// </summary>
public static partial class AnsiConsoleExtensions
{
    internal static async Task<string> ReadLine(this IAnsiConsole console, Style? style, bool secret, char? mask, IEnumerable<string>? items = null, CancellationToken cancellationToken = default)
    {
        if (console is null)
        {
            throw new ArgumentNullException(nameof(console));
        }

        style ??= Style.Plain;
        var text = new List<char>();
        var position = 0;

        var autocomplete = new List<string>(items ?? Enumerable.Empty<string>());
        var isOsxPlatform = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        var isWinOrLinuxPlatform = !isOsxPlatform;

        int Advance(int direction)
        {
            var steps = 0;
            var reachedGreedyChar = false;

            bool IsNextGreedyChar() => direction == -1
                ? !char.IsWhiteSpace(text[position + steps + direction])
                : char.IsWhiteSpace(text[position + steps]);

            while (position + steps + direction >= 0 && position + steps + direction <= text.Count)
            {
                steps += direction;
                if (position + steps == 0 || position + steps == text.Count)
                {
                    break;
                }

                if (reachedGreedyChar && !IsNextGreedyChar())
                {
                    break;
                }

                if (IsNextGreedyChar())
                {
                    reachedGreedyChar = true;
                }
            }

            return Math.Abs(steps);
        }

        void Backspace(int steps = 1)
        {
            console.Cursor.MoveLeft(steps);
            console.Write(new string(text.Skip(position).Concat(Enumerable.Repeat(' ', steps)).ToArray()));
            console.Cursor.MoveLeft(steps + (text.Count - position));
            text.RemoveRange(position - steps, steps);
            position -= steps;
        }

        void Delete(int steps = 1)
        {
            console.Write(new string(text.Skip(position + steps).Concat(Enumerable.Repeat(' ', steps)).ToArray()));
            console.Cursor.MoveLeft(steps + (text.Count - position) - 1);
            text.RemoveRange(position, steps);
        }

        while (true)
        {
            var rawKey = await console.Input.ReadKeyAsync(true, cancellationToken).ConfigureAwait(false);
            if (rawKey == null)
            {
                continue;
            }

            var key = rawKey.Value;

            // Enter
            if (key.Key == ConsoleKey.Enter)
            {
                return new string(text.ToArray());
            }

            // Completion
            if (key.Key == ConsoleKey.Tab && autocomplete.Count > 0)
            {
                var replace = AutoComplete(autocomplete, new string(text.ToArray()));
                if (!string.IsNullOrEmpty(replace))
                {
                    // Render the suggestion
                    console.Write("\b \b".Repeat(text.Count), style);
                    console.Write(replace);
                    text = replace.ToList();
                    continue;
                }
            }

            var isControlPressed = key.Modifiers.HasFlag(ConsoleModifiers.Control);
            var isAltPressed = key.Modifiers.HasFlag(ConsoleModifiers.Alt);
            switch (key.Key)
            {
                // Backspace
                case ConsoleKey.Backspace:

                    if ((isWinOrLinuxPlatform && isControlPressed) ||
                        (isOsxPlatform && isAltPressed))
                    {
                        Backspace(secret ? position : Advance(-1));
                    }
                    else if (position > 0)
                    {
                        Backspace();
                    }

                    continue;

                // Delete
                case ConsoleKey.Delete:

                    if ((isWinOrLinuxPlatform && isControlPressed) ||
                        (isOsxPlatform && isAltPressed))
                    {
                        Delete(secret ? position : Advance(1));
                    }
                    else if (position < text.Count)
                    {
                        Delete();
                    }

                    continue;

                // Left Arrow
                case ConsoleKey.LeftArrow when isOsxPlatform || !isControlPressed:

                    if (position > 0)
                    {
                        position--;
                        console.Cursor.MoveLeft();
                    }

                    continue;

                // Right Arrow
                case ConsoleKey.RightArrow when isOsxPlatform || !isControlPressed:

                    if (position < text.Count)
                    {
                        position++;
                        console.Cursor.MoveRight();
                    }

                    continue;

                // Home
                case ConsoleKey.Home:

                    console.Cursor.MoveLeft(position);
                    position = 0;
                    continue;

                // End
                case ConsoleKey.End:

                    console.Cursor.MoveRight(text.Count - position);
                    position = text.Count;
                    continue;

                // Ctrl + Left Arrow
                case ConsoleKey.LeftArrow when isWinOrLinuxPlatform && isControlPressed:
                case ConsoleKey.B when isOsxPlatform && isAltPressed:

                    var leftMoveChars = secret ? position : Advance(-1);
                    position -= leftMoveChars;
                    console.Cursor.MoveLeft(leftMoveChars);

                    continue;

                // Ctrl + Right Arrow
                case ConsoleKey.F when isOsxPlatform && isAltPressed:
                case ConsoleKey.RightArrow when isWinOrLinuxPlatform && isControlPressed:

                    var rightMoveChars = secret ? text.Count - position : Advance(1);
                    position += rightMoveChars;
                    console.Cursor.MoveRight(rightMoveChars);

                    continue;
            }

            // Normal Input
            if (!char.IsControl(key.KeyChar))
            {
                console.Write(secret ? "*" : new string(new[] { key.KeyChar }.Concat(text.Skip(position)).ToArray()), style);
                console.Cursor.MoveLeft(text.Count - position);
                text.Insert(position, key.KeyChar);
                position++;
            }
        }

        style ??= Style.Plain;
        var text = string.Empty;

        var autocomplete = new List<string>(items ?? Enumerable.Empty<string>());

        while (true)
        {
            var rawKey = await console.Input.ReadKeyAsync(true, cancellationToken).ConfigureAwait(false);
            if (rawKey == null)
            {
                continue;
            }

            var key = rawKey.Value;
            if (key.Key == ConsoleKey.Enter)
            {
                return text;
            }

            if (key.Key == ConsoleKey.Tab && autocomplete.Count > 0)
            {
                var autoCompleteDirection = key.Modifiers.HasFlag(ConsoleModifiers.Shift)
                    ? AutoCompleteDirection.Backward
                    : AutoCompleteDirection.Forward;
                var replace = AutoComplete(autocomplete, text, autoCompleteDirection);
                if (!string.IsNullOrEmpty(replace))
                {
                    // Render the suggestion
                    console.Write("\b \b".Repeat(text.Length), style);
                    console.Write(replace);
                    text = replace;
                    continue;
                }
            }

            if (key.Key == ConsoleKey.Backspace)
            {
                if (text.Length > 0)
                {
                    text = text.Substring(0, text.Length - 1);
                    console.Write("\b \b");
                }

                continue;
            }

            if (!char.IsControl(key.KeyChar))
            {
                text += key.KeyChar.ToString();
                var output = key.KeyChar.ToString();
                console.Write(secret ? output.Mask(mask) : output, style);
            }
        }
    }

    private static string AutoComplete(List<string> autocomplete, string text, AutoCompleteDirection autoCompleteDirection)
    {
        var found = autocomplete.Find(i => i == text);
        var replace = string.Empty;

        if (found == null)
        {
            // Get the closest match
            var next = autocomplete.Find(i => i.StartsWith(text, true, CultureInfo.InvariantCulture));
            if (next != null)
            {
                replace = next;
            }
            else if (string.IsNullOrEmpty(text))
            {
                // Use the first item
                replace = autocomplete[0];
            }
        }
        else
        {
            // Get the next match
            replace = GetAutocompleteValue(autoCompleteDirection, autocomplete, found);
        }

        return replace;
    }

    private static string GetAutocompleteValue(AutoCompleteDirection autoCompleteDirection, IList<string> autocomplete, string found)
    {
        var foundAutocompleteIndex = autocomplete.IndexOf(found);
        var index = autoCompleteDirection switch
        {
            AutoCompleteDirection.Forward => foundAutocompleteIndex + 1,
            AutoCompleteDirection.Backward => foundAutocompleteIndex - 1,
            _ => throw new ArgumentOutOfRangeException(nameof(autoCompleteDirection), autoCompleteDirection, null),
        };

        if (index >= autocomplete.Count)
        {
            index = 0;
        }

        if (index < 0)
        {
            index = autocomplete.Count - 1;
        }

        return autocomplete[index];
    }

    private enum AutoCompleteDirection
    {
        Forward,
        Backward,
    }
}