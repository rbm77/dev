using System.Data;
using Buslogix.Interfaces;
using Buslogix.Models;
using Buslogix.Models.DTO;
using Buslogix.Utilities;

namespace Buslogix.Repositories
{
    public class PaymentRequestRepository(IDataAccess dataAccess) : IPaymentRequestRepository
    {

        public async Task<PaymentRequest?> GetPaymentRequest(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            List<PaymentRequest> rows = await dataAccess.ExecuteReader("get_payment_request", CommandType.StoredProcedure,
                static reader => new PaymentRequest
                {
                    Id = reader.GetInt32OrDefault(0),
                    Date = reader.GetDateTimeOrDefault(1),
                    Amount = reader.GetDecimalOrDefault(2),
                    StudentId = reader.GetInt32OrDefault(3),
                    ReceiptReference = reader.GetStringOrDefault(4, "") ?? "",
                    RequestedAt = reader.GetDateTimeOrDefault(5),
                    IsValidated = reader.GetBooleanOrDefault(6)
                }, parameters);

            return rows.Count > 0 ? rows[0] : null;
        }

        public async Task<PagedResult<PaymentRequest>> GetPaymentRequests(int companyId, DateTime? requestedAt = null, bool? isValidated = null, string? receiptReference = null, int page = 1, int pageSize = 20)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_requested_at"] = requestedAt,
                ["p_is_validated"] = isValidated,
                ["p_receipt_reference"] = receiptReference,
                ["p_page"] = page,
                ["p_page_size"] = pageSize
            };

            (List<PaymentRequest> items, long totalCount) = await dataAccess.ExecuteReaderPaged("get_payment_requests", CommandType.StoredProcedure,
                static reader => new PaymentRequest
                {
                    Id = reader.GetInt32OrDefault(0),
                    Date = reader.GetDateTimeOrDefault(1),
                    Amount = reader.GetDecimalOrDefault(2),
                    StudentId = reader.GetInt32OrDefault(3),
                    ReceiptReference = reader.GetStringOrDefault(4, "") ?? "",
                    RequestedAt = reader.GetDateTimeOrDefault(5),
                    IsValidated = reader.GetBooleanOrDefault(6)
                }, parameters);

            return new PagedResult<PaymentRequest>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<int> InsertPaymentRequest(int companyId, PaymentRequest paymentRequest)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_date"] = paymentRequest.Date,
                ["p_amount"] = paymentRequest.Amount,
                ["p_student_id"] = paymentRequest.StudentId,
                ["p_receipt_reference"] = paymentRequest.ReceiptReference
            };

            object? result = await dataAccess.ExecuteScalar("insert_payment_request", CommandType.StoredProcedure, parameters);
            return result != null ? (int)result : 0;
        }

        public async Task<long> ApprovePaymentRequest(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            Dictionary<string, DbType> outputParameters = new()
            {
                ["p_new_payment_id"] = DbType.Int64
            };

            (_, IDictionary<string, object?> output) = await dataAccess.ExecuteNonQuery("approve_payment_request", CommandType.StoredProcedure, parameters, outputParameters);
            return Convert.ToInt64(output["p_new_payment_id"] ?? 0L);
        }

        public async Task<AutoApprovalResult> AutoApprovePaymentRequests()
        {
            List<AutoApprovalResult> rows = await dataAccess.ExecuteReader("auto_approve_payment_requests", CommandType.StoredProcedure,
                static reader => new AutoApprovalResult
                {
                    ProcessedCount = reader.GetInt32OrDefault(0),
                    ApprovedCount = reader.GetInt32OrDefault(1),
                    FailedCount = reader.GetInt32OrDefault(2)
                }, null);

            return rows.Count > 0 ? rows[0] : new AutoApprovalResult();
        }

        public async Task<bool> RejectPaymentRequest(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            int affected = await dataAccess.ExecuteNonQuery("reject_payment_request", CommandType.StoredProcedure, parameters);
            return affected > 0;
        }

        public async Task<bool> ValidatePaymentRequest(int companyId, int id)
        {
            Dictionary<string, object?> parameters = new()
            {
                ["p_company_id"] = companyId,
                ["p_id"] = id
            };

            int affected = await dataAccess.ExecuteNonQuery("validate_payment_request", CommandType.StoredProcedure, parameters);
            return affected > 0;
        }
    }
}
