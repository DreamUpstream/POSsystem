using MediatR;
using Microsoft.Extensions.Logging;
using POSsystem.Contracts.Data;
using POSsystem.Contracts.DTO;
using POSsystem.Contracts.Services;
using POSsystem.Core.Exceptions;
using AutoMapper;

namespace POSsystem.Core.Handlers.Queries
{
    public class GetBranchByIdQuery :IRequest<BranchDTO>
    {
        public int BranchId { get; }
        public GetBranchByIdQuery(int id)
        {
            BranchId = id;
        }
    }

    public class GetBranchByIdQueryHandler : IRequestHandler<GetBranchByIdQuery, BranchDTO>
    {
        private readonly IUnitOfWork _repository;
        private readonly IMapper _mapper;
        private readonly ICachingService _cache;
        private readonly ILogger<GetBranchByIdQueryHandler> _logger;

        public GetBranchByIdQueryHandler(ILogger<GetBranchByIdQueryHandler> logger, IUnitOfWork repository, IMapper mapper, ICachingService cache)
        {
            _repository = repository;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        public async Task<BranchDTO> Handle(GetBranchByIdQuery request, CancellationToken cancellationToken)
        {
            var cachedItem = _cache.GetItem<BranchDTO>($"branch_{request.BranchId}");
            if (cachedItem != null)
            {
                _logger.LogInformation($"Branch Exists in Cache. Return CachedItem.");
                return cachedItem;
            }

            _logger.LogInformation($"Branch doesn't exist in Cache.");

            var branch = await Task.FromResult(_repository.Branches.Get(request.BranchId));
            if (branch == null)
            {
                throw new EntityNotFoundException($"No Branch found for Id {request.BranchId}");
            }

            var result = _mapper.Map<BranchDTO>(branch);

            _logger.LogInformation($"Add Branch to Cache and return.");
            var _ = _cache.SetItem($"branch_{request.BranchId}", result);
            return result;
        }
    }
}