using System;
/// <summary>
/// Weight data point.
/// </summary>
namespace GPlusQuickstartCsharp
{
public class StepDataPoint
{
	     /// <summary>
        /// Gets or sets the step.
        /// </summary>
        /// <value>
        /// The step.
        /// </value>
        public integer? Step { get; set; }

        /// <summary>
        /// Gets or sets the stamp.
        /// </summary>
        /// <value>
        /// The stamp.
        /// </value>
        public DateTime Stamp { get; set; }

        /// Gets or sets the maximum Step.
        public double? MaxStep { get; set; }

        /// Gets or sets the minimum Step.
        public double? MinStep { get; set; }

        
    }
}
