using System;

namespace fincalc
{
    public class Flow {
        public enum FlowType { Cash, Securities, InKind }
        public FlowType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime EffectiveDate { get; set; }
    }

}