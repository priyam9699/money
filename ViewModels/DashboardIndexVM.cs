namespace FinanceManagement.ViewModels
{
    public class DashboardIndexVM
    {
        public string Company { get; set; }
        public decimal? AccountBalance { get; set; }
        public decimal? FlipkartOutstandingPayment { get; set; }
        public decimal? AmazonOutstandingPayment { get; set; }
        public decimal? MeeshoOutstandingPayment { get; set; }
        public decimal? OtherOutstandingPayment { get; set; }
        public decimal? PendingPayment { get; set; }
        public decimal? CashBalance { get; set; }
        public decimal? TotalUpad { get; set; }

        
        public Dictionary<string, decimal> TotalAmountMonthly {  get; set; }
        public Dictionary<string, decimal> FirmAccountBalances { get; set; }

    }
}
