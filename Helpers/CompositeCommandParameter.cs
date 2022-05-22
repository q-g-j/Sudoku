// Helper class for AdvancedInvokeCommandAction.
//
// Snippet taken from:
// https://stackoverflow.com/questions/66465149/pass-extra-argument-to-command-with-invokecommandaction



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku.Helpers
{    public class CompositeCommandParameter
    {
        public CompositeCommandParameter(EventArgs eventArgs, string parameter)
        {
            EventArgs = eventArgs;
            Parameter = parameter;
        }

        public EventArgs EventArgs { get; }

        public string Parameter { get; }
    }
}
