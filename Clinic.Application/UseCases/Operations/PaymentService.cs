using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Clinic.Application.Interfaces;
using Clinic.Application.Interfaces.Configuration;
using Clinic.Application.Interfaces.MasterData;
using Clinic.Application.Interfaces.Operations;
using Clinic.Domain.Entities.Operations;

namespace Clinic.Application.UseCases.Operations
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMasterReferenceService _masterReferenceService;
        private readonly INumberSequenceService _numberSequenceService;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IInvoiceRepository invoiceRepository,
            IMasterReferenceService masterReferenceService,
            INumberSequenceService numberSequenceService,
            IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _invoiceRepository = invoiceRepository;
            _masterReferenceService = masterReferenceService;
            _numberSequenceService = numberSequenceService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Payment> CreatePaymentAsync(Guid invoiceId, decimal amount, string paymentMethod, string? referenceNumber, string? notes, Guid userId, CancellationToken cancellationToken = default)
        {
            if (amount <= 0)
                throw new InvalidOperationException("Payment amount must be greater than zero.");

            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, cancellationToken);
            if (invoice == null)
                throw new InvalidOperationException("Invoice not found.");

            if (invoice.Status != "Finalized")
                throw new InvalidOperationException("Payment can only be made against finalized invoices.");

            // Verify payment method
            var methodRef = await _masterReferenceService.GetByCodeAsync("PaymentMethod", paymentMethod, cancellationToken);
            if (methodRef == null || !methodRef.IsActive)
                throw new InvalidOperationException($"Invalid or inactive payment method: {paymentMethod}");

            // Calculate outstanding
            var totalPaid = await _paymentRepository.GetTotalPaidByInvoiceIdAsync(invoiceId, cancellationToken);
            var outstanding = invoice.TotalAmount - totalPaid;

            if (outstanding <= 0)
                throw new InvalidOperationException("Invoice is already fully paid.");

            if (amount > outstanding)
                throw new InvalidOperationException($"Payment amount ({amount:C}) exceeds outstanding balance ({outstanding:C}).");

            // Generate receipt number
            string receiptNumber;
            try
            {
                receiptNumber = await _numberSequenceService.GenerateSequenceAsync("RCPT", cancellationToken);
            }
            catch
            {
                // Fallback if RCPT sequence is not seeded yet, though it should be
                receiptNumber = $"RCPT-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}";
            }

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                InvoiceId = invoiceId,
                ReceiptNumber = receiptNumber,
                PaymentDate = DateTime.UtcNow,
                Amount = amount,
                PaymentMethod = methodRef.Code,
                ReferenceNumber = referenceNumber,
                Notes = notes,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = userId,
                IsDeleted = false
            };

            await _paymentRepository.AddAsync(payment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return payment;
        }

        public async Task<IEnumerable<Payment>> GetPaymentsByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
        {
            return await _paymentRepository.GetPaymentsByInvoiceIdAsync(invoiceId, cancellationToken);
        }

        public async Task<decimal> GetTotalPaidByInvoiceIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
        {
            return await _paymentRepository.GetTotalPaidByInvoiceIdAsync(invoiceId, cancellationToken);
        }
    }
}
