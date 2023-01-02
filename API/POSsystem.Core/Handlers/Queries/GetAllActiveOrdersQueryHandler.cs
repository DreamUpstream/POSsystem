using AutoMapper;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Enum;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace POSsystem.Core.Handlers.Queries
{
    public class GetAllActiveOrdersQuery : IRequest<IEnumerable<OrderDTO>>
    {
    }

    public class GetAllActiveOrdersQueryHandler : IRequestHandler<GetAllActiveOrdersQuery, IEnumerable<OrderDTO>>
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly IDistributedCache _cache;

        public GetAllActiveOrdersQueryHandler(IUnitOfWork repository, IMapper mapper, IDistributedCache cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<IEnumerable<OrderDTO>> Handle(GetAllActiveOrdersQuery request, CancellationToken cancellationToken)
        {
            var cachedEntitiesString = await _cache.GetStringAsync("all_active_orders");
            
            if (cachedEntitiesString == null)
            {
                var dbEntities = await Task.FromResult(_repository.Orders.GetAll().Where(x => x.Status == OrderStatus.Created));
                var entities = dbEntities.Select(x => new OrderDTO
                {
                    SubmissionDate = x.SubmissionDate,
                    Tip = x.Tip,
                    DeliveryRequired = x.DeliveryRequired,
                    Comment = x.Comment,
                    Status = x.Status,
                    CustomerId = x.CustomerId,
                    EmployeeId = x.EmployeeId,
                    DiscountId = x.DiscountId,
                    Delivery = x.Delivery,
                    Products = x.Products.ToList(),
                    Services = x.Services.ToList()
                }).ToList();

                await _cache.SetStringAsync("all_active_orders", JsonConvert.SerializeObject(entities));
                return entities;
            }
            else
            {
                var cachedEntities = JsonConvert.DeserializeObject<IEnumerable<OrderDTO>>(cachedEntitiesString);
                return cachedEntities;
            }

        }
    }
}