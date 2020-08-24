using System;
using System.Text.Json.Serialization;
using Utils;

namespace fincalc
{
    public class Return {
        [JsonConverter(typeof(JsonDateTimeConverter))]
        public DateTime EffectiveDate { get; set; }
        [JsonConverter(typeof(JsonDecimalConverter))]
        public decimal Rate { get; set; }
    }

}
