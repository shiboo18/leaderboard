using System;
/// <summary>
/// Weight data point.
/// </summary>
namespace GPlusQuickstartCsharp
{
    public class stepDataPoint
    {
        /// <summary>
        /// Gets or sets the step.
        /// </summary>
        /// <value>
        /// The step.
        /// </value>
        public int? Step { get; set; }

        /// <summary>
        /// Gets or sets the stamp.
        /// </summary>
        /// <value>
        /// The stamp.
        /// </value>
        public DateTime Stamp { get; set; }

        /// Gets or sets the maximum Step.
        public int? MaxStep { get; set; }

        /// Gets or sets the minimum Step.
        public int? MinStep { get; set; }


    }
}
