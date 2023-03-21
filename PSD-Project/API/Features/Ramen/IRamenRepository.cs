using System;
using System.Collections.Generic;
using Util.Option;
using Util.Try;

namespace PSD_Project.API.Features.Ramen
{
    public interface IRamenRepository
    {
        Try<List<Ramen>, Exception> GetRamen();
        Try<Ramen, Exception> GetRamen(int ramenId);
        Try<Ramen, Exception> CreateRamen(string name, string borth, string price, int meatId);
        Try<Ramen, Exception> UpdateRamen(int ramenId, string name, string borth, string price, int meatId);
        Option<Exception> DeleteRamen(int ramenId);
    }
}