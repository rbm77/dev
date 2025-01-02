namespace Pos.Utilities
{
    public static class Enums
    {
        public enum LogType
        {
            Info,
            Error
        }

        public enum PaymentMethod
        {
            Cash,
            Card,
            Transfer,
            Check
        }

        public enum TransactionType
        {
            Receivable,
            Payable
        }

        public enum DocumentType
        {
            Sale
        }
    }
}
