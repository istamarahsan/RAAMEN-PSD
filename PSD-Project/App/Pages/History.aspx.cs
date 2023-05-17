using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using PSD_Project.API.Features.Authentication;
using PSD_Project.API.Features.Users;
using PSD_Project.App.Common;
using PSD_Project.App.Services.RamenService;
using PSD_Project.App.Services.Transactions;
using PSD_Project.App.Services.Users;
using Util.Collections;
using Util.Option;

namespace PSD_Project.App.Pages
{
    public partial class History : Page
    {
        private readonly ITransactionService transactionService = AppServices.Singletons.TransactionService;
        private readonly IUserService userService = AppServices.Singletons.UserService;
        private readonly IRamenService ramenService = AppServices.Singletons.RamenService;
        protected List<TransactionViewModel> Transactions = new List<TransactionViewModel>();
        
        protected class TransactionViewModel
        {
            public DateTime? Date;
            public string StaffName;
            public string CustomerName;
            public double Total;
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            var canSeePage = Session.GetUserSession()
                .Map(session => session.Role.Name == "Customer" || session.Role.Name == "Admin")
                .OrElse(false);
            if (!canSeePage || Request.GetTokenFromCookie().IsNone())
            {
                Response.Redirect("Login.aspx");
            }
            if (IsPostBack) return;
            InitPage(Session.GetUserSession().Unwrap(), Request.GetTokenFromCookie().Unwrap());
        }

        private void InitPage(UserSessionDetails user, int token)
        {
            var transactionsFetch = user.Role.Name == "Admin"
                ? transactionService.GetAllTransactions(token).Unwrap()
                : transactionService.GetTransactions(token).Unwrap();
            var usersFetch = transactionsFetch
                .SelectMany(t => new List<int> { t.CustomerId, t.StaffId })
                .Distinct()
                .Select(id => userService.GetUser(token, id))
                .Select(result => result.Ok().UnwrapOrNull())
                .Where(result => result != null)
                .ToDictionary(u => u.Id);
            var ramenFetch = transactionsFetch
                .SelectMany(t => t.Details.Select(d => d.RamenId))
                .Distinct()
                .Select(id => ramenService.GetRamen(id))
                .Select(result => result.Ok().UnwrapOrNull())
                .Where(result => result != null)
                .ToDictionary(r => r.Id);
            Transactions = transactionsFetch
                .Select(t => new TransactionViewModel
                {
                    Date = t.Date,
                    StaffName = usersFetch.Get(t.StaffId).Map(u => u.Username).UnwrapOrNull(),
                    CustomerName = usersFetch.Get(t.CustomerId).Map(u => u.Username).UnwrapOrNull(),
                    Total = t.Details
                        .Select(d => ramenFetch.Get(d.RamenId).Unwrap().Price.TryParseDouble().Ok().OrElse(0) * d.Quantity)
                        .Sum()
                }).ToList();
        }
    }
}