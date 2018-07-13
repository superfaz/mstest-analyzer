// Copyright (c) Francois Karman. All rights reserved.
// Licensed under the MIT license.

using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Linq;

namespace MSTest.Analyzer.Helpers
{
    /// <summary>
    /// Represents an expected diagnostic.
    /// </summary>
    public class DiagnosticResult
    {
        private readonly ICollection<DiagnosticResultLocation> locations;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiagnosticResult"/> class.
        /// </summary>
        public DiagnosticResult()
        {
            this.locations = new LinkedList<DiagnosticResultLocation>();
        }

        /// <summary>
        /// Gets or sets the locations associated to the diagnostic result.
        /// </summary>
        public IEnumerable<DiagnosticResultLocation> Locations
        {
            get
            {
                return this.locations;
            }

            set
            {
                this.locations.Clear();
                foreach (var v in value)
                {
                    this.locations.Add(v);
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique location of the diagnostic result.
        /// </summary>
        public DiagnosticResultLocation Location
        {
            get
            {
                return this.locations.FirstOrDefault();
            }

            set
            {
                this.locations.Clear();
                this.locations.Add(value);
            }
        }

        /// <summary>
        /// Gets or sets the severity of the result.
        /// </summary>
        public DiagnosticSeverity Severity { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the diagnostic.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the message of the diagnostic result.
        /// </summary>
        public string Message { get; set; }
    }
}
