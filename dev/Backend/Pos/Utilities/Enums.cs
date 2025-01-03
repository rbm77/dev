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

        public enum AccountType
        {
            Receivable,
            Payable
        }

        public enum TransactionType
        {
            Sale,
            Refund
        }

        public enum Currency
        {
            CRC,
            USD
        }
    }
}
