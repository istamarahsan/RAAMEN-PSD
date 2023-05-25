using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using PSD_Project.API.Features.Authentication;
using PSD_Project.App.Common;
using PSD_Project.App.Services.Transactions;
using Util.Option;
using IRamenService = PSD_Project.App.Services.RamenService.IRamenService;

namespace PSD_Project.App.Pages
{
    public partial class Transaction : Page
    {
        protected class TransactionDetailViewModel
        {
            public int RamenId;
            public string RamenName;
            public double Price;
            public int Quantity;
        }

        protected int TransactionId;
        protected List<TransactionDetailViewModel> TransactionDetails;

        private readonly ITransactionService transactionService = AppServices.Singletons.TransactionService;
        private readonly IRamenService ramenService = AppServices.Singletons.RamenService;

        protected void Page_Load(object sender, EventArgs e)
        {
            var canSeePage = Session.GetUserSession()
                .Map(session => session.Role.Name == "Customer" || session.Role.Name == "Admin")
                .OrElse(false);
            if (!canSeePage || Request.GetTokenFromCookie().IsNone()) Response.Redirect("Login.aspx");
            var token = Request.GetTokenFromCookie().Unwrap();
            var getTransactionId = Request.QueryString["transaction"].ToOption().Bind(s => s.TryParseInt().Ok());
            if (getTransactionId.IsNone()) Response.Redirect("History.aspx");
            TransactionId = getTransactionId.Unwrap();
            InitPage(Session.GetUserSession().Unwrap(), token, TransactionId);
        }

        protected void InitPage(UserSessionDetails user, int token, int transactionId)
        {
            var getTransactions = user.Role.Name == "Admin"
                ? transactionService.GetAllTransactions(token)
                : transactionService.GetTransactions(token);

            var retrievedTransactions = getTransactions.Unwrap("Failed to retrieve transactions");

            var targetTransaction = retrievedTransactions
                .FirstOrDefault(t => t.Id == transactionId)
                .ToOption()
                .Unwrap("Transaction not found");

            TransactionDetails = targetTransaction.Details
                .Select(detail => (detail, ramen: ramenService.GetRamen(detail.RamenId).Ok().Unwrap($"Failed to retrieve ramen with ID: {detail.RamenId}")))
                .Select(pair => new TransactionDetailViewModel
                {
                    RamenId = pair.detail.RamenId,
                    RamenName = pair.ramen.Name,
                    Price = pair.ramen.Price.TryParseDouble().Ok().OrElse(0),
                    Quantity = pair.detail.Quantity
                })
                .ToList();
        }
    }
}