using System;
using System.Collections.Generic;

namespace Clinic.Application.DTOs.Operations
{
    public class PatientLedgerDto
    {
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public decimal TotalDebit { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal CurrentBalance => TotalDebit - TotalCredit;
        
        public List<PatientLedgerTransactionDto> Transactions { get; set; } = new List<PatientLedgerTransactionDto>();
    }

    public class PatientLedgerTransactionDto
    {
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal RunningBalance { get; set; }
        
        // Secondary sorting properties
        public int SortOrder { get; set; } // e.g., Invoice=1, Payment=2
        public Guid SourceId { get; set; }
    }
}
