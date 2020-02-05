using AuctionHouse.Common.Interfaces;
using AuctionHouse.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouse.Common.Services
{
    public class ParallelWowItemPopulator : IPopulator<WowItem>
    {
        public readonly IWowheadService[] _services;
        private IWowItemRepository _itemRepo;

        private const int maxItemId = 300000;

        public ParallelWowItemPopulator(IWowheadService[] services, IWowItemRepository itemRepo)
        {
            _services = services;
            _itemRepo = itemRepo;
        }

        public Task CleanAsync()
        {
            throw new NotImplementedException();
        }

        public async Task PopulateAsync()
        {
            var interval = maxItemId / _services.Count();
            var tasks = new List<Task>();
            for (var i = 0; i < _services.Count(); i++)
            {
                var service = _services[i];
                var start = i * interval;
                var end = (i + 1) * interval;
                var task = Task.Run(() => GetItemsInRangeAsync(service, start, end));
                tasks.Add(task);
            }

            await Task.WhenAll(tasks.ToArray());
        }

        private async Task GetItemsInRangeAsync(IWowheadService service, int start, int end)
        {
            for (var i = start; i < end; i++)
            {
                var item = await _itemRepo.GetItemAsync(i);
                if (item == null)
                {
                    item = await service.GetItemAsync(i);
                    Console.WriteLine($"{item.Id}: {item.Name}");
                    await _itemRepo.AddItemAsync(item);
                }
            }
        }
    }

    public class WowItemPopulator : IPopulator<WowItem>
    {
        private readonly IWowheadService _wowheadService;
        private IWowItemRepository _itemRepo;

        public WowItemPopulator(IWowheadService wowheadService, IWowItemRepository itemRepository)
        {
            _wowheadService = wowheadService;
            _itemRepo = itemRepository;
        }

        public async Task CleanAsync()
        {
            throw new NotImplementedException();
        }

        public async Task PopulateAsync()
        {
            for (var i = 0; i < 400000; i++)
            {

                var item = await _itemRepo.GetItemAsync(i);
                if (item == null)
                {
                    item = await _wowheadService.GetItemAsync(i);
                    if (item != null)
                    {
                        Console.WriteLine($"{item.Id}: {item.Name}");
                        await _itemRepo.AddItemAsync(item);
                    }
                }
                else
                {
                    Console.WriteLine($"{i} already exists");
                }
            }

            //await Task.Run(() =>
            //{
            //    var options = new ParallelOptions
            //    {
            //        MaxDegreeOfParallelism = 4
            //    };
            //    Parallel.For(0, 999999, options, async i =>
            //    {
            //        Console.WriteLine("searching for " + i);
            //        var item = await _itemRepo.GetItemAsync(i);
            //        if (item == null)
            //        {
            //            item = await _wowheadService.GetItemAsync(i);
            //            if (item != null)
            //            {
            //                Console.WriteLine($"{item.Id}: {item.Name}");
            //                await _itemRepo.AddItemAsync(item);
            //            }
            //        }

            //    });
            //});
        }
    }
}
