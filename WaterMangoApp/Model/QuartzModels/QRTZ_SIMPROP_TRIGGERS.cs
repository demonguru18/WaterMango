namespace WaterMangoApp.Model.QuartzModels
{
    public class QRTZ_SIMPROP_TRIGGERS
    {
        public string SCHED_NAME { get; set; }
        public string TRIGGER_NAME { get; set; }
        public string TRIGGER_GROUP { get; set; }
        public string STR_PROP_1 { get; set; }
        public string STR_PROP_2 { get; set; }
        public string STR_PROP_3 { get; set; }
        public int? INT_PROP_1 { get; set; }
        public int? INT_PROP_2 { get; set; }
        public long? LONG_PROP_1 { get; set; }
        public long? LONG_PROP_2 { get; set; }
        public decimal? DEC_PROP_1 { get; set; }
        public decimal? DEC_PROP_2 { get; set; }
        public bool? BOOL_PROP_1 { get; set; }
        public bool? BOOL_PROP_2 { get; set; }
        public string TIME_ZONE_ID { get; set; }

        public virtual QRTZ_TRIGGERS QRTZ_TRIGGERS { get; set; }
    }
}