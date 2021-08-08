using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moonpig.PostOffice.Api.Model;

namespace Moonpig.PostOffice.Api.Services
{
    public interface IDespatchService
    {
        Task<DespatchDate> GetDespatchDateAsync(List<int> productIds, DateTime orderDate);
    }
}