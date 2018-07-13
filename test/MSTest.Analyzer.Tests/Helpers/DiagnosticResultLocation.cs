// Copyright (c) Francois Karman. All rights reserved.
// Licensed under the MIT license.

using System;

namespace MSTest.Analyzer.Helpers
{
    /// <summary>
    /// Represents a ocation where the diagnostic appears, as determined by path, line number, and column number.
    /// </summary>
    public class DiagnosticResultLocation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticResultLocation"/> class.
        /// </summary>
        /// <param name="path">The path of the file associated with the diagnostic result.</param>
        /// <param name="line">The line associated with the diagnostic result.</param>
        /// <param name="column">The column associated with the diagnostic result.</param>
        public DiagnosticResultLocation(string path, int line, int column)
        {
            if (line < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(line), "line must be >= -1");
            }

            if (column < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(column), "column must be >= -1");
            }

            this.Path = path;
            this.Line = line;
            this.Column = column;
        }

        /// <summary>
        /// Gets the path of the file associated with the diagnostic result.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the line associated with the diagnostic result.
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// Gets the column associated with the diagnostic result.
        /// </summary>
        public int Column { get; }
    }
}
