using System;
using Google.Apis.Fitness.v1;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GPlusQuickstartCsharp
{
    public class StepQuery : FitnessQuery
    {
        private const string DataSource = "derived:com.google.step_count.delta:com.google.android.gms:estimated_steps";
        private const string DataType = "com.google.step_count.delta";

        /// Initializes a new instance of the StepQuery class.
        public StepQuery(FitnessService service) :
            base(service, DataSource, DataType)
        {
        }

        /// Queries the step.
        public IList<StepDataPoint> QueryStep(DateTime start, DateTime end)
        {
            var request = CreateRequest(start, end);
            var response = ExecuteRequest(request);

            return response
                .Bucket
                .SelectMany(b => b.Dataset)
                .Where(d => d.Point != null)
                .SelectMany(d => d.Point)
                .Where(p => p.Value != null)
                .SelectMany(p =>
                {
                    return p.Value.Select(v =>
                        new StepDataPoint
                        {
                            Step = v.FpVal.GetValueOrDefault(),
                            Stamp = GoogleTime.FromNanoseconds(p.StartTimeNanos).ToDateTime()
                        });
                })
                .ToList();
        }

        /// Queries the weight.
        public IList<StepDataPoint> QueryStepPerDay(DateTime start, DateTime end)
        {
            return QueryStep(start, end)
                .OrderBy(w => w.Stamp)
                .GroupBy(w => w.Stamp.Date)
                .Select(g => new StepDataPoint
                {
                    Stamp = g.Key,
                    MaxStep = g.Max(w => w.Step),
                    MinStep = g.Min(w => w.Step),
                })
                .ToList();
        }
    }

}
