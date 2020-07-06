using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Luger.Utilities
{
    public class REPL
    {
        public abstract class StateBase
        {
            public abstract string Prompt { get; }

            public abstract Task<(string output, StateBase nextState)> Eval(string input);
        }

        public abstract class StateBase<TInput> : StateBase where TInput : struct
        {
            public abstract TInput ParseInput(string input);

            public abstract Task<(string output, StateBase nextState)> Eval(TInput input);

            public sealed override Task<(string output, StateBase nextState)> Eval(string input) =>
                Eval(ParseInput(input));
        }

        public abstract class CharCommandStateBase : StateBase<char>
        {
            private readonly char[] _commands;
            private readonly char? _defaultCommand;
            private readonly bool _ignoreCase;

            protected CharCommandStateBase(IEnumerable<char> commands, char? defaultCommand, bool ignoreCase)
            {
                if (commands == null)
                    throw new ArgumentNullException(nameof(commands));

                if (ignoreCase)
                {
                    commands = commands.Select(char.ToLowerInvariant);

                    if (defaultCommand.HasValue)
                        defaultCommand = char.ToLowerInvariant(defaultCommand.Value);
                }

                _commands = commands.Distinct().ToArray();

                if (_commands.Length == 0)
                    throw new ArgumentException();

                if (defaultCommand.HasValue && !_commands.Contains(defaultCommand.Value))
                    throw new ArgumentException();

                _defaultCommand = defaultCommand;
                _ignoreCase = ignoreCase;
            }

            public override string Prompt => $"({string.Join("|", _commands)}) " + (_defaultCommand.HasValue ? $"[{_defaultCommand}] > " : "> ");

            public override char ParseInput(string input)
            {
                if (input == null)
                    throw new ArgumentNullException(nameof(input));

                switch (input.Length)
                {
                    case 0:
                        return _defaultCommand ?? throw new ArgumentOutOfRangeException(nameof(input));
                    case 1:
                        return _ignoreCase ? char.ToLowerInvariant(input[0]) : input[0];
                    default:
                        throw new ArgumentOutOfRangeException(nameof(input));
                }
            }
        }

        private class StartState : CharCommandStateBase
        {
            public StartState() : base(new[] { 'p', 'q', 'r' }, 'r', true) { }

            public override Task<(string output, StateBase nextState)> Eval(char input)
            {
                throw new NotImplementedException();
            }
        }

        private readonly TextReader _reader;
        private readonly TextWriter _writer;

        public REPL(TextReader reader = null, TextWriter writer = null)
        {
            _reader = reader ?? Console.In;
            _writer = writer ?? Console.Out;
        }

        public async Task Run()
        {
            StateBase state = new StartState();

            do
            {
                await _writer.WriteAsync(state.Prompt);

                var input = await _reader.ReadLineAsync();

                string output;
                (output, state) = await state.Eval(input);

                await _writer.WriteLineAsync(output);
            }
            while (state != null);
        }
    }
}
