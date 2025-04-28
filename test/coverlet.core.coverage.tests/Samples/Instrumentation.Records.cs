using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coverlet.Core.CoverageSamples.Tests
{
    public record RecordWithPropertyInit
    {
        private int _myRecordVal = 0;
        public RecordWithPropertyInit()
        {
            _myRecordVal = new Random().Next();
        }
        public string RecordAutoPropsNonInit { get; set; }
        public string RecordAutoPropsInit { get; set; } = string.Empty;
    }

    public class ClassWithRecordsAutoProperties
    {
        record RecordWithPrimaryConstructor(string Prop1, string Prop2);

        public ClassWithRecordsAutoProperties()
        {
            var record = new RecordWithPrimaryConstructor(string.Empty, string.Empty);
        }
    }

    public class ClassWithInheritingRecordsAndAutoProperties
    {
        record BaseRecord(int A);

        record InheritedRecord(int A) : BaseRecord(A);

        public ClassWithInheritingRecordsAndAutoProperties()
        {
            var record = new InheritedRecord(1);
        }
    }

    public class ClassWithRecordsEmptyPrimaryConstructor
	{
        internal record First
        {
            public string Bar() => "baz";
        }

        internal record Second()
        {
            public string Bar() => "baz";
        }
	}

    public class ClassWithAbstractRecords
    {
        public abstract record AuditData()
        {
            public abstract string GetAuditType();
        }

        public abstract record AuditData
        {
            private protected AuditData()
            {

            }

            public abstract string GetAuditType();
        }
	}
}
